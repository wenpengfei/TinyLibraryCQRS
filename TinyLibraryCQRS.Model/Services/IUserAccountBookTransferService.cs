/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */


namespace TinyLibraryCQRS.Model.Services
{
    public interface IUserAccountBookTransferService
    {
        void BorrowBookToUser(UserAccount userAccount, Book book);
        void ReturnBookFromUser(UserAccount userAccount, Book book);
    }
}
