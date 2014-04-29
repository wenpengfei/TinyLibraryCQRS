/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.ServiceProcess;
using System.Timers;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Config;
using Apworks.Events.Serialization;
using Microsoft.Practices.Unity;

namespace TinyLibraryCQRS.Services.SynchronizationService
{
    public sealed class SynchronizationServiceProc : ServiceBase
    {
        private string eventMessageQueue = null;
        private string eventMessageReceiveTimeoutString = null;
        private string intervalString = null;

        private readonly IMessageDispatcher messageDispatcher = null;
        private readonly Timer timer = new Timer();
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<MessageProcessingEventArgs> Processing;
        public event EventHandler<MessageProcessFailedEventArgs> ProcessFailed;

        public SynchronizationServiceProc()
        {
            this.ServiceName = "TinyLibraryCQRS Synchronization Service";
            this.EventLog.Log = "Application";

            this.CanShutdown = true;
            this.CanStop = true;

            AppConfigSource configSource = new AppConfigSource();
            IApp application = AppRuntime.Create(configSource);
            application.Initialize += (s, e) =>
            {
                UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
                IMessageDispatcher eventDispatcher = MessageDispatcher.CreateAndRegister(configSource, typeof(MessageDispatcher));
                c.RegisterInstance<IMessageDispatcher>(eventDispatcher);
            };
            application.Start();

            messageDispatcher = AppRuntime.Instance.CurrentApplication.ObjectContainer.GetService<IMessageDispatcher>();

            timer.Interval = this.Interval;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = false;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker localWorker = sender as BackgroundWorker;
            if (localWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            List<string> allMessageIds = e.Argument as List<string>;
            var messageCount = allMessageIds.Count;
            List<DomainEventMessageContent> messageContents = new List<DomainEventMessageContent>();
            using (MessageQueue messageQueue = new MessageQueue(this.EventMessageQueue))
            {
                messageQueue.MessageReadPropertyFilter.SentTime = true;
                for (int i = 0; i < messageCount; i++)
                {
                    Message message = messageQueue.PeekById(allMessageIds[i], this.EventMessageReceiveTimeout);
                    var messageContent = new DomainEventMessageContent(message);
                    messageContents.Add(messageContent);
                }
                messageQueue.Close();
            }
            var sortedMessageContents = messageContents.OrderBy(mc => mc.SentTime);
            foreach (var mc in sortedMessageContents)
            {
                bool canRemove = true;
                try
                {
                    if (!mc.IsValidMessage)
                        throw new Exception("Invalid Message Content.");
                    OnProcessing(mc);
                    Type eventType = Type.GetType(mc.Type);
                    if (eventType != null)
                    {
                        DomainEventXmlSerializer xmlSerializer = new DomainEventXmlSerializer();
                        var domainEvent = xmlSerializer.Deserialize(eventType, mc.Bytes);
                        messageDispatcher.DispatchMessage(domainEvent);
                    }
                    else
                        canRemove = false;
                }
                catch (Exception ex)
                {
                    OnProcessFailed(mc.MessageId, mc, ex);
                    canRemove = false;
                }
                finally
                {
                    if (canRemove)
                    {
                        using (MessageQueue messageQueue = new MessageQueue(this.EventMessageQueue))
                        {
                            try
                            {
                                messageQueue.ReceiveById(mc.MessageId, this.EventMessageReceiveTimeout);
                            }
                            finally
                            {
                                messageQueue.Close();
                            }
                        }
                    }
                }
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                int messageCount = 0;
                List<string> messageIds = new List<string>();
                using (MessageQueue messageQueue = new MessageQueue(this.EventMessageQueue))
                {
                    var messages = messageQueue.GetAllMessages();
                    messageCount = messages.Length;
                    messageIds = messages.Select(p => p.Id).ToList();
                    messageQueue.Close();
                }
                if (messageCount > 0)
                {
                    worker.RunWorkerAsync(messageIds);
                }
            }
        }

        private string EventMessageQueue
        {
            get
            {
                if (this.eventMessageQueue == null)
                    this.eventMessageQueue = ConfigurationManager.AppSettings["EventMessageQueue"];
                return this.eventMessageQueue;
            }
        }

        private TimeSpan EventMessageReceiveTimeout
        {
            get
            {
                if (this.eventMessageReceiveTimeoutString == null)
                    this.eventMessageReceiveTimeoutString = ConfigurationManager.AppSettings["EventMessageReceiveTimeout"];
                return TimeSpan.Parse(this.eventMessageReceiveTimeoutString);
            }
        }

        private double Interval
        {
            get
            {
                if (string.IsNullOrEmpty(this.intervalString))
                    this.intervalString = ConfigurationManager.AppSettings["Interval"];
                double intVal = double.MinValue;
                if (double.TryParse(this.intervalString, out intVal))
                    return intVal;
                return 5000;
            }
        }

        private void OnStartedProcessing()
        {
            EventHandler eh = this.Started;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        private void OnStoppedProcessing()
        {
            EventHandler eh = this.Stopped;
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        private void OnProcessing(DomainEventMessageContent content)
        {
            EventHandler<MessageProcessingEventArgs> eh = this.Processing;
            if (eh != null)
            {
                MessageProcessingEventArgs e = new MessageProcessingEventArgs(content);
                eh(this, e);
            }
        }

        private void OnProcessFailed(string messageId, DomainEventMessageContent content, Exception exception)
        {
            EventHandler<MessageProcessFailedEventArgs> eh = this.ProcessFailed;
            if (eh != null)
            {
                MessageProcessFailedEventArgs e = new MessageProcessFailedEventArgs(messageId, content, exception);
                eh(this, e);
            }
        }

#if !CONSOLE
        static void Main()
        {
            ServiceBase.Run(new SynchronizationServiceProc());
        }
#endif

        public void StartProc()
        {
            timer.Start();
            this.OnStartedProcessing();
        }

        public void StopProc()
        {
            timer.Stop();
            if (worker.WorkerSupportsCancellation)
                worker.CancelAsync();
            while (true)
            {
                if (!worker.IsBusy)
                    break;
            }
            this.OnStoppedProcessing();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            this.StartProc();
        }

        protected override void OnStop()
        {
            this.StopProc();
        }
    }
}
