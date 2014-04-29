/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Services.SynchronizationService
{
    public class MessageProcessingEventArgs : EventArgs
    {
        public DomainEventMessageContent MessageContent { get; set; }

        public MessageProcessingEventArgs() { }

        public MessageProcessingEventArgs(DomainEventMessageContent messageContent)
        {
            this.MessageContent = messageContent;
        }
    }
}
