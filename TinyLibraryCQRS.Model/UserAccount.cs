/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Collections.Generic;
using Apworks;
using Apworks.Snapshots;
using TinyLibraryCQRS.Events.Accounts;
using TinyLibraryCQRS.Model.Snapshots;

namespace TinyLibraryCQRS.Model
{
    public partial class UserAccount : Account
    {
        private List<long> bookIds = new List<long>();

        #region Ctor
        internal UserAccount() : base() { }
        internal UserAccount(long id) : base(id) { }
        #endregion

        #region Factory Methods
        public static UserAccount Create(long? id, 
            string userName, 
            string password, 
            string displayName, 
            string email, 
            string contactPhone,
            Address contactAddress)
        {
            UserAccount userAccount = null;
            if (id != null && id.HasValue)
                userAccount = new UserAccount(id.Value);
            else
                userAccount = new UserAccount();
            userAccount.RaiseEvent<UserAccountCreatedEvent>(new UserAccountCreatedEvent
                {
                    ContactAddressCity = contactAddress.City,
                    ContactAddressCountry = contactAddress.Country,
                    ContactAddressState = contactAddress.State,
                    ContactAddressStreet = contactAddress.Street,
                    ContactAddressZip = contactAddress.Zip,
                    ContactPhone = contactPhone,
                    UserName = userName,
                    Password = password,
                    DisplayName = displayName,
                    Email = email
                });
            return userAccount;
        }

        public static UserAccount Create(string userName, 
            string password, 
            string displayName, 
            string email,
            string contactPhone, 
            Address contactAddress)
        {
            return Create(null, userName, password, displayName, email, contactPhone, contactAddress);
        }
        #endregion

        #region Public Properties
        public string ContactPhone { get; private set; }
        public Address ContactAddress { get; private set; }
        #endregion

        #region Internal Methods
        internal void BorrowBook(Book book)
        {
            if (this.bookIds.Contains(book.Id))
                throw new DomainException("You've already borrowed this book.");
            this.RaiseEvent<UserBorrowBookEvent>(new UserBorrowBookEvent
                {
                    UserAccountId = this.Id,
                    BookId = book.Id,
                    TransferDateTime = DateTime.Now
                });
        }

        internal void ReturnBook(Book book)
        {
            if (!this.bookIds.Contains(book.Id))
                throw new DomainException("I have not borrowed this book yet.");
            this.RaiseEvent<UserReturnBookEvent>(new UserReturnBookEvent
                {
                    UserAccountId = this.Id,
                    BookId = book.Id,
                    TransferDateTime = DateTime.Now
                });
        }
        #endregion

        #region Public Methods
        public void UpdateBasicInformation(string displayName, string email, string contactPhone,
            string country, string state, string city, string street, string zip)
        {
            this.RaiseEvent<UserAccountUpdatedEvent>(new UserAccountUpdatedEvent
                {
                    DisplayName = displayName,
                    Email = email,
                    ContactPhone = contactPhone,
                    ContactAddressCountry = country,
                    ContactAddressState = state,
                    ContactAddressCity = city,
                    ContactAddressStreet = street,
                    ContactAddressZip = zip
                });
        }
        #endregion

        #region Domain Event Handlers
        [Handles(typeof(UserAccountCreatedEvent))]
        private void HandleUserAccountCreatedEvent(UserAccountCreatedEvent e)
        {
            this.UserName = e.UserName;
            this.Password = e.Password;
            this.DisplayName = e.DisplayName;
            this.Email = e.Email;
            this.ContactPhone = e.ContactPhone;
            Address address = new Address
            {
                Country = e.ContactAddressCountry,
                State = e.ContactAddressState,
                City = e.ContactAddressCity,
                Street = e.ContactAddressStreet,
                Zip = e.ContactAddressZip
            };
            this.ContactAddress = address;
        }

        [Handles(typeof(UserBorrowBookEvent))]
        private void HandleBorrowBookEvent(UserBorrowBookEvent e)
        {
            this.bookIds.Add(e.BookId);
        }

        [Handles(typeof(UserReturnBookEvent))]
        private void HandleReturnBookEvent(UserReturnBookEvent e)
        {
            this.bookIds.Remove(e.BookId);
        }

        [Handles(typeof(UserAccountUpdatedEvent))]
        private void HandleUserAccountUpdatedEvent(UserAccountUpdatedEvent e)
        {
            this.DisplayName = e.DisplayName;
            this.Email = e.Email;
            this.ContactPhone = e.ContactPhone;
            if (this.ContactAddress == null)
                this.ContactAddress = new Address();
            this.ContactAddress.City = e.ContactAddressCity;
            this.ContactAddress.Country = e.ContactAddressCountry;
            this.ContactAddress.State = e.ContactAddressState;
            this.ContactAddress.Street = e.ContactAddressStreet;
            this.ContactAddress.Zip = e.ContactAddressZip;
        }
        #endregion

        protected override void DoBuildFromSnapshot(ISnapshot snapshot)
        {
            UserAccountSnapshot s = (UserAccountSnapshot)snapshot;
            this.UserName = s.UserName;
            this.Password = s.Password;
            this.Email = s.Email;
            this.DisplayName = s.DisplayName;
            this.ContactPhone = s.ContactPhone;
            if (this.ContactAddress == null)
                this.ContactAddress = new Address();
            this.ContactAddress.City = s.ContactAddressCity;
            this.ContactAddress.Country = s.ContactAddressCountry;
            this.ContactAddress.State = s.ContactAddressState;
            this.ContactAddress.Street = s.ContactAddressStreet;
            this.ContactAddress.Zip = s.ContactAddressZip;
            this.bookIds.Clear();
            s.BookIds.ForEach(p => this.bookIds.Add(p));
        }

        protected override ISnapshot DoCreateSnapshot()
        {
            UserAccountSnapshot snapshot = new UserAccountSnapshot
            {
                UserName = this.UserName,
                Password = this.Password,
                Email = this.Email,
                DisplayName = this.DisplayName,
                ContactPhone = this.ContactPhone,
                ContactAddressCity = this.ContactAddress.City,
                ContactAddressState = this.ContactAddress.State,
                ContactAddressCountry = this.ContactAddress.Country,
                ContactAddressStreet = this.ContactAddress.Street,
                ContactAddressZip = this.ContactAddress.Zip
            };
            this.bookIds.ForEach(p => snapshot.BookIds.Add(p));
            return snapshot;
        }
    }
}
