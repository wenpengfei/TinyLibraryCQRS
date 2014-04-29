/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Runtime.Serialization;

namespace TinyLibraryCQRS.Services.QueryServices.QueryDataObjects
{
    [DataContract]
    public class BookDataObject
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ISBN { get; set; }
        [DataMember]
        public int Pages { get; set; }
        [DataMember]
        public int Inventory { get; set; }
        [DataMember]
        public long AggregateRootId { get; set; }
    }
}