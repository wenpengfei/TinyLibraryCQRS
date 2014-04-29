/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Services.SynchronizationService
{
    public class MessageProcessFailedEventArgs : EventArgs
    {
        public string MessageId { get; set; }
        public DomainEventMessageContent MessageContent { get; set; }
        public Exception Exception { get; set; }

        public MessageProcessFailedEventArgs() { }

        public MessageProcessFailedEventArgs(string messageId, DomainEventMessageContent messageContent, Exception exception)
        {
            this.MessageId = messageId;
            this.MessageContent = messageContent;
            this.Exception = exception;
        }
    }
}
