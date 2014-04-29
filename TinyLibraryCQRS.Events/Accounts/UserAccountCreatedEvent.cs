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
    public class UserAccountCreatedEvent : DomainEvent
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddressCountry { get; set; }
        public string ContactAddressState { get; set; }
        public string ContactAddressCity { get; set; }
        public string ContactAddressStreet { get; set; }
        public string ContactAddressZip { get; set; }
    }
}
