/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using Apworks;
using Apworks.Snapshots;
using TinyLibraryCQRS.Events.Accounts;
using TinyLibraryCQRS.Model.Snapshots;

namespace TinyLibraryCQRS.Model
{
    public partial class AdminAccount : Account
    {
        #region Ctor
        internal AdminAccount() : base() { }
        internal AdminAccount(long id) : base(id) { }
        #endregion

        #region Factory Methods
        public static AdminAccount Create(long? id, string username, string password,
            string displayName, string email)
        {
            AdminAccount adminAccount = null;
            if (id != null && id.HasValue)
                adminAccount = new AdminAccount(id.Value);
            else
                adminAccount = new AdminAccount();
            adminAccount.RaiseEvent<AdminAccountCreatedEvent>(new AdminAccountCreatedEvent
                {
                    UserName = username,
                    Password = password,
                    DisplayName = displayName,
                    Email = email
                });
            return adminAccount;
        }

        public static AdminAccount Create(string username, string password, string displayName, string email)
        {
            return Create(null, username, password, displayName, email);
        }
        #endregion

        #region Domain Event Handlers
        [Handles(typeof(AdminAccountCreatedEvent))]
        private void HandleAdminAccountCreatedEvent(AdminAccountCreatedEvent e)
        {
            this.UserName = e.UserName;
            this.Password = e.Password;
            this.DisplayName = e.DisplayName;
            this.Email = e.Email;
        }
        #endregion

        protected override void DoBuildFromSnapshot(ISnapshot snapshot)
        {
            AdminAccountSnapshot s = (AdminAccountSnapshot)snapshot;
            this.DisplayName = s.DisplayName;
            this.Email = s.Email;
            this.Password = s.Password;
            this.UserName = s.UserName;
        }

        protected override ISnapshot DoCreateSnapshot()
        {
            return new AdminAccountSnapshot
            {
                DisplayName = this.DisplayName,
                Email = this.Email,
                Password = this.Password,
                UserName = this.UserName
            };
        }
    }
}
