/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Model.Services
{
    public class UserAccountBookTransferService : IUserAccountBookTransferService
    {
        #region IUserAccountBookTransferService Members

        public void BorrowBookToUser(UserAccount userAccount, Book book)
        {
            if (userAccount == null)
                throw new ArgumentNullException("userAccount");
            if (book == null)
                throw new ArgumentNullException("book");

            userAccount.BorrowBook(book);
            book.IssueStock(1);
        }

        public void ReturnBookFromUser(UserAccount userAccount, Book book)
        {
            if (userAccount == null)
                throw new ArgumentNullException("userAccount");
            if (book == null)
                throw new ArgumentNullException("book");

            userAccount.ReturnBook(book);
            book.ReceiveStock(1);
        }

        #endregion
    }
}
