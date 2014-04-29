/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using Apworks.Commands;
using Apworks.Repositories;
using TinyLibraryCQRS.Commands;
using TinyLibraryCQRS.Model;
using TinyLibraryCQRS.Model.Services;

namespace TinyLibraryCQRS.CommandHandlers
{
    public class TinyLibraryCQRSCommandHandler : CommandHandlerBase, 
        ICommandHandler<RegisterUserAccountCommand>,
        ICommandHandler<RegisterAdminAccountCommand>,
        ICommandHandler<CreateBookCommand>,
        ICommandHandler<UpdateBookCommand>,
        ICommandHandler<UpdateUserAccountCommand>,
        ICommandHandler<BorrowBookCommand>,
        ICommandHandler<ReturnBookCommand>
    {
        #region IHandler<RegisterUserAccountCommand> Members

        public bool Handle(RegisterUserAccountCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                Address address = new Address();
                address.City = message.ContactAddressCity;
                address.Country = message.ContactAddressCountry;
                address.State = message.ContactAddressState;
                address.Street = message.ContactAddressStreet;
                address.Zip = message.ContactAddressZip;
                UserAccount userAccount = UserAccount.Create(message.UserName,
                    message.Password, message.DisplayName, message.Email, message.ContactPhone, address);
                domainRepository.Save<UserAccount>(userAccount);
                domainRepository.Commit();
                return true;
            }
        }

        #endregion

        #region IHandler<CreateBookCommand> Members

        public bool Handle(CreateBookCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                Book book = Book.Create(message.Title, message.Author, message.Description, message.ISBN, message.Pages, message.Inventory);
                domainRepository.Save<Book>(book);
                domainRepository.Commit();
                return true;
            }
        }

        #endregion


        #region IHandler<RegisterAdminAccountCommand> Members

        public bool Handle(RegisterAdminAccountCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                AdminAccount adminAccount = AdminAccount.Create(message.UserName,
                    message.Password, message.DisplayName, message.Email);
                domainRepository.Save<AdminAccount>(adminAccount);
                domainRepository.Commit();
                return true;
            }
        }

        #endregion

        #region IHandler<UpdateBookCommand> Members

        public bool Handle(UpdateBookCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                Book book = domainRepository.Get<Book>(message.AggregateRootId);
                book.UpdateBasicInformation(message.Title, message.Author, message.Description, message.ISBN, message.Pages, message.Inventory);
                domainRepository.Save<Book>(book);
                domainRepository.Commit();
                return domainRepository.Committed;
            }
        }

        #endregion

        #region IHandler<UpdateUserAccountCommand> Members

        public bool Handle(UpdateUserAccountCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                UserAccount userAccount = domainRepository.Get<UserAccount>(message.AggregateRootId);
                userAccount.UpdateBasicInformation(message.DisplayName, message.Email, message.ContactPhone,
                    message.ContactAddressCountry, message.ContactAddressState, message.ContactAddressCity,
                    message.ContactAddressStreet, message.ContactAddressZip);
                domainRepository.Save(userAccount);
                domainRepository.Commit();
                return domainRepository.Committed;
            }
        }

        #endregion

        #region IHandler<BorrowBookCommand> Members

        public bool Handle(BorrowBookCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                UserAccount userAccount = domainRepository.Get<UserAccount>(message.UserAccountAggregateRootId);
                Book book = domainRepository.Get<Book>(message.BookAggregateRootId);
                IUserAccountBookTransferService bookTransService = this.GetBookTransferService();
                bookTransService.BorrowBookToUser(userAccount, book);
                domainRepository.Save(userAccount);
                domainRepository.Save(book);
                domainRepository.Commit();
                return domainRepository.Committed;
            }
        }

        #endregion

        #region IHandler<ReturnBookCommand> Members

        public bool Handle(ReturnBookCommand message)
        {
            using (IDomainRepository domainRepository = this.GetDomainRepository())
            {
                UserAccount user = domainRepository.Get<UserAccount>(message.UserAccountAggregateRootId);
                Book book = domainRepository.Get<Book>(message.BookAggregateRootId);
                IUserAccountBookTransferService bookTransService = this.GetBookTransferService();
                bookTransService.ReturnBookFromUser(user, book);
                domainRepository.Save(user);
                domainRepository.Save(book);
                domainRepository.Commit();
                return domainRepository.Committed;
            }
        }

        #endregion
    }
}
