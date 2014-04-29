/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using Apworks.Application;
using Apworks.Repositories;
using TinyLibraryCQRS.Model.Services;

namespace TinyLibraryCQRS.CommandHandlers
{
    public abstract class CommandHandlerBase
    {
        protected IDomainRepository GetDomainRepository()
        {
            return AppRuntime.Instance.CurrentApplication.ObjectContainer.GetService<IDomainRepository>();
        }

        protected IUserAccountBookTransferService GetBookTransferService()
        {
            return AppRuntime.Instance.CurrentApplication.ObjectContainer.GetService<IUserAccountBookTransferService>();
        }
    }
}
