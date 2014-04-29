/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using Apworks.Commands;

namespace TinyLibraryCQRS.Commands
{
    [Serializable]
    public class ReturnBookCommand : Command
    {
        public long UserAccountAggregateRootId { get; set; }
        public long BookAggregateRootId { get; set; }
    }
}
