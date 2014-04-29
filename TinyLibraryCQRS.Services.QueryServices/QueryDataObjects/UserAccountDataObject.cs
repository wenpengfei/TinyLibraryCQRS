/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TinyLibraryCQRS.Services.QueryServices.QueryDataObjects
{
    [DataContract]
    public class UserAccountDataObject
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string ContactPhone { get; set; }
        [DataMember]
        public string AddressCountry { get; set; }
        [DataMember]
        public string AddressState { get; set; }
        [DataMember]
        public string AddressCity { get; set; }
        [DataMember]
        public string AddressStreet { get; set; }
        [DataMember]
        public string AddressZip { get; set; }
        [DataMember]
        public long AggregateRootId { get; set; }
        [DataMember]
        public List<BookTransferHistoryDataObject> BorrowedBooks { get; set; }
    }
}