/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using Apworks.Events;

namespace TinyLibraryCQRS.Events.Accounts
{
    [Serializable]
    public abstract class UserAccountBookTransferEvent : DomainEvent
    {
        public long UserAccountId { get; set; }
        public long BookId { get; set; }
        public DateTime TransferDateTime { get; set; }
    }
}
