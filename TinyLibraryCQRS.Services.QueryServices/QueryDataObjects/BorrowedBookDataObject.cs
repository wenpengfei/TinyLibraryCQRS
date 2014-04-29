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
    public class BookTransferHistoryDataObject
    {
        [DataMember]
        public Guid BookID { get; set; }
        [DataMember]
        public long BookAggregateRootId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string ISBN { get; set; }
        [DataMember]
        public DateTime ReturnedDate { get; set; }
        [DataMember]
        public bool Returned { get; set; }
        [DataMember]
        public DateTime BorrowedDate { get; set; }
    }
}