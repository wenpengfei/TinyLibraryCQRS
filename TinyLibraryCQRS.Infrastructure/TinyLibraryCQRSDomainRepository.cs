/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.Threading;
using Apworks.Bus;
using Apworks.Events.Storage;
using Apworks.Repositories;
using Apworks.Snapshots.Providers;

namespace TinyLibraryCQRS.Infrastructure
{
    public class TinyLibraryCQRSDomainRepository : EventSourcedDomainRepository
    {
        public TinyLibraryCQRSDomainRepository(IDomainEventStorage domainEventStorage, IEventBus eventBus, ISnapshotProvider snapshotProvider)
            : base(domainEventStorage, eventBus, snapshotProvider) { }

        protected override void DoCommit()
        {
            try
            {
                base.DoCommit();
            }
            catch
            {
                EventBus.Clear();
                throw;
            }
            // TODO: Add the commit notification to the sych service
            Thread.Sleep(6000);
        }

    }
}
