/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Collections.Generic;
using System.ServiceModel;
using TinyLibraryCQRS.Services.QueryServices.QueryDataObjects;

namespace TinyLibraryCQRS.Services.QueryServices
{
    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        UserAccountDataObject GetUserByUserName(string userName);

        [OperationContract]
        UserAccountDataObject GetUserByEmail(string email);

        [OperationContract]
        bool ValidateUser(string userName, string password);

        [OperationContract]
        AccountType? GetAccountType(string userName);

        [OperationContract]
        List<BookDataObject> GetBooks();

        [OperationContract]
        BookDataObject GetBookByGuid(Guid guid);

        [OperationContract]
        UserAccountDataObject GetUserByGuid(Guid guid);
    }
}
