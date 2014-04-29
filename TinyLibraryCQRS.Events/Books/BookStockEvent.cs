/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using Apworks.Events;

namespace TinyLibraryCQRS.Events.Books
{
    [Serializable]
    public abstract class BookStockEvent : DomainEvent
    {
        public int Quantity { get; set; }
    }
}
