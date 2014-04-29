/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.ServiceModel;
using TinyLibraryCQRS.Infrastructure;
using TinyLibraryCQRS.Services.CommandServices.CommandDataObjects;

namespace TinyLibraryCQRS.Services.CommandServices
{
    [ServiceContract]
    public interface ICommandService
    {
        [OperationContract]
        void RegisterUserAccount(string userName, string password, string displayName, string email,
            string contactPhone, AddressDataObject contactAddress);

        [OperationContract]
        void RegisterAdminAccount(string userName, string password, string displayName, string email);

        [OperationContract]
        void AddBook(string title, string author, string description, string isbn, int pages, int inventory);

        [OperationContract]
        void UpdateBook(long id, string title, string author, string description, string isbn, int pages, int inventory);

        [OperationContract]
        void UpdateUserAccount(long id, string displayName, string email,
            string contactPhone, AddressDataObject contactAddress);

        [OperationContract]
        [FaultContract(typeof(WCFServiceFault))]
        void BorrowBookToUser(long bookId, long userAccountId);

        [OperationContract]
        void ReturnBookFromUser(long bookId, long userAccountId);
    }
}
