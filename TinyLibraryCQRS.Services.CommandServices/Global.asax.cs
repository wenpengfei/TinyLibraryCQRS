/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Configuration;
using System.Diagnostics;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Bus.DirectBus;
using Apworks.Bus.MSMQ;
using Apworks.Config;
using Apworks.Events.Storage;
using Apworks.Events.Storage.General;
using Apworks.Repositories;
using Apworks.Snapshots.Providers;
using Apworks.Storage;
using Apworks.Storage.General;
using Microsoft.Practices.Unity;
using TinyLibraryCQRS.Infrastructure;
using TinyLibraryCQRS.Model.Services;

namespace TinyLibraryCQRS.Services.CommandServices
{
    public class Global : System.Web.HttpApplication
    {
        private const string EventDBConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=TinyLibraryCQRS_EventDB;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;";
        private const string EventBusMessageQueue = @".\private$\TinyLibraryCQRS_EventBus";

        protected void Application_Start(object sender, EventArgs e)
        {
            IConfigSource appConfigSource = new AppConfigSource();
            IApp application = AppRuntime.Create(appConfigSource);
            application.Initialize += new EventHandler<AppInitEventArgs>(application_Initialize);
            application.Start();
        }

        void application_Initialize(object sender, AppInitEventArgs e)
        {
            string eventDBConnectionString = ConfigurationManager.ConnectionStrings["EventDBConnectionString"].ConnectionString;
            if (string.IsNullOrEmpty(eventDBConnectionString))
                eventDBConnectionString = EventDBConnectionString;

            string eventBusMessageQueue = ConfigurationManager.AppSettings["EventBusMessageQueue"];
            if (string.IsNullOrEmpty(eventBusMessageQueue))
                eventBusMessageQueue = EventBusMessageQueue;

            UnityContainer c = e.ObjectContainer.GetWrappedContainer<UnityContainer>();
            IMessageDispatcher commandDispatcher = MessageDispatcher.CreateAndRegister(e.ConfigSource, typeof(MessageDispatcher));
            c.RegisterInstance<IMessageDispatcher>("commandDispatcher", commandDispatcher)
                .RegisterType<ICommandBus, DirectCommandBus>(new InjectionConstructor(new ResolvedParameter<IMessageDispatcher>("commandDispatcher")))
                .RegisterType<IStorageMappingResolver, XmlStorageMappingResolver>(new InjectionConstructor("EventDBXmlMappingSchema.xml"))
                .RegisterType<IDomainEventStorage, SqlDomainEventStorage>(new InjectionConstructor(eventDBConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IEventBus, MSMQEventBus>(new ContainerControlledLifetimeManager(), new InjectionConstructor(eventBusMessageQueue, false))
                .RegisterType<IStorage, SqlStorage>("eventStore", new InjectionConstructor(eventDBConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<IStorage, SqlStorage>("snapshotStore", new InjectionConstructor(eventDBConnectionString, new ResolvedParameter<IStorageMappingResolver>()))
                .RegisterType<ISnapshotProvider, EventNumberSnapshotProvider>(new InjectionConstructor(new ResolvedParameter<IStorage>("eventStore"), new ResolvedParameter<IStorage>("snapshotStore"), new InjectionParameter<SnapshotProviderOption>(SnapshotProviderOption.Immediate), new InjectionParameter<int>(100)))
                .RegisterType<IDomainRepository, TinyLibraryCQRSDomainRepository>(new InjectionConstructor(new ResolvedParameter<IDomainEventStorage>(), new ResolvedParameter<IEventBus>(), new ResolvedParameter<ISnapshotProvider>()))
                .RegisterType<IUserAccountBookTransferService, UserAccountBookTransferService>();

            if (!EventLog.SourceExists(Utils.EventLogApplication))
                EventLog.CreateEventSource(Utils.EventLogApplication, Utils.EventLog);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}