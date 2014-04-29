/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.ServiceModel;
using Apworks;
using Apworks.Application;
using Apworks.Bus;
using Apworks.Commands;
using TinyLibraryCQRS.Commands;
using TinyLibraryCQRS.Infrastructure;
using TinyLibraryCQRS.Services.CommandServices.CommandDataObjects;

namespace TinyLibraryCQRS.Services.CommandServices
{
    public class CommandService : ICommandService
    {
        private void CommitCommand(ICommand command)
        {
            try
            {
                using (ICommandBus commandBus = AppRuntime
                    .Instance
                    .CurrentApplication
                    .ObjectContainer
                    .GetService<ICommandBus>())
                {
                    commandBus.Publish(command);
                    commandBus.Commit();
                }
            }
            catch (Exception ex)
            {
                WCFServiceFault sf = new WCFServiceFault
                    {
                        Message=ex.Message,
                        StackTrace=ex.StackTrace
                    };
                throw new FaultException<WCFServiceFault>(sf, sf.Message);
            }
        }

        #region ICommandService Members

        public void RegisterUserAccount(string userName, string password, string displayName, string email,
            string contactPhone, AddressDataObject contactAddress)
        {
            RegisterUserAccountCommand command = new RegisterUserAccountCommand
            {
                UserName = userName,
                Password = password,
                DisplayName = displayName,
                Email = email,
                ContactPhone = contactPhone,
                ContactAddressCity = contactAddress.City,
                ContactAddressCountry = contactAddress.Country,
                ContactAddressState = contactAddress.State,
                ContactAddressStreet = contactAddress.Street,
                ContactAddressZip = contactAddress.Zip
            };
            this.CommitCommand(command);
        }

        public void RegisterAdminAccount(string userName, string password, string displayName, string email)
        {
            RegisterAdminAccountCommand command = new RegisterAdminAccountCommand
            {
                UserName = userName,
                Password = password,
                DisplayName = displayName,
                Email = email
            };
            this.CommitCommand(command);
        }

        public void AddBook(string title, string author, string description, string isbn, int pages, int inventory)
        {
            CreateBookCommand command = new CreateBookCommand
            {
                Title = title,
                Author = author,
                Description = description,
                ISBN = isbn,
                Pages = pages,
                Inventory = inventory
            };
            this.CommitCommand(command);
        }

        public void UpdateBook(long id, string title, string author, string description, string isbn, int pages, int inventory)
        {
            UpdateBookCommand command = new UpdateBookCommand
            {
                Title = title,
                Author = author,
                Description = description,
                AggregateRootId = id,
                ISBN = isbn,
                Pages = pages,
                Inventory = inventory
            };
            this.CommitCommand(command);
        }

        public void UpdateUserAccount(long id, string displayName, string email,
            string contactPhone, AddressDataObject contactAddress)
        {
            UpdateUserAccountCommand command = new UpdateUserAccountCommand
            {
                AggregateRootId = id,
                DisplayName = displayName,
                Email = email,
                ContactPhone = contactPhone,
                ContactAddressCountry = contactAddress.Country,
                ContactAddressState = contactAddress.State,
                ContactAddressCity = contactAddress.City,
                ContactAddressStreet = contactAddress.Street,
                ContactAddressZip = contactAddress.Zip
            };
            this.CommitCommand(command);
        }

        public void BorrowBookToUser(long bookId, long userAccountId)
        {
            try
            {
                BorrowBookCommand command = new BorrowBookCommand
                {
                    BookAggregateRootId = bookId,
                    UserAccountAggregateRootId = userAccountId
                };
                this.CommitCommand(command);
            }
            catch (DomainException ex)
            {
                WCFServiceFault sf = new WCFServiceFault
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
                throw new FaultException<WCFServiceFault>(sf, new FaultReason(sf.Message));
            }
        }

        public void ReturnBookFromUser(long bookId, long userAccountId)
        {
            ReturnBookCommand command = new ReturnBookCommand
            {
                BookAggregateRootId = bookId,
                UserAccountAggregateRootId = userAccountId
            };
            this.CommitCommand(command);
        }

        #endregion
    }
}
