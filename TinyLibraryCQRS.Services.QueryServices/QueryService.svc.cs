/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using TinyLibraryCQRS.Services.QueryServices.QueryDataObjects;

namespace TinyLibraryCQRS.Services.QueryServices
{
    public class QueryService : IQueryService
    {
        private string queryDBConnectionString;

        private string QueryDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(queryDBConnectionString))
                {
                    queryDBConnectionString = ConfigurationManager
                        .ConnectionStrings["QueryDBConnectionString"]
                        .ConnectionString;
                }
                return queryDBConnectionString;
            }
        }

        private bool ValidateAccount(string tableName, string userName, string password)
        {
            bool ret = false;
            string sql = string.Format(@"SELECT [Password] FROM [{0}] WHERE [UserName]=@userName", tableName);
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@userName", userName)))
            {
                while (reader.Read())
                {
                    string passwordFromDB = Convert.ToString(reader["Password"]);
                    if (password == passwordFromDB)
                        ret = true;
                    break;
                }
                reader.Close();
            }
            return ret;
        }

        private int GetAccountTypeIndicator(string userName)
        {
            int ret = -1;
            string sqlUserAccounts = @"SELECT COUNT(*) FROM [UserAccounts] WHERE [UserName]=@userName";
            int userAccountsCount = Convert.ToInt32(SqlHelper.ExecuteScalar(this.QueryDBConnectionString,
                CommandType.Text,
                sqlUserAccounts, new SqlParameter("@userName", userName)));
            if (userAccountsCount > 0)
            {
                ret = 1;
            }
            else
            {
                string sqlAdminAccounts = @"SELECT COUNT(*) FROM [AdminAccounts] WHERE [UserName]=@userName";
                int adminAccountsCount = Convert.ToInt32(SqlHelper.ExecuteScalar(this.QueryDBConnectionString,
                    CommandType.Text,
                    sqlAdminAccounts, new SqlParameter("@userName", userName)));
                if (adminAccountsCount > 0)
                    ret = 0;
            }
            return ret;
        }

        private void ConvertFromReader(SqlDataReader reader, UserAccountDataObject obj)
        {
            obj.AddressCity = Convert.ToString(reader["Address_City"]);
            obj.AddressCountry = Convert.ToString(reader["Address_Country"]);
            obj.AddressState = Convert.ToString(reader["Address_State"]);
            obj.AddressStreet = Convert.ToString(reader["Address_Street"]);
            obj.AddressZip = Convert.ToString(reader["Address_Zip"]);
            obj.AggregateRootId = Convert.ToInt64(reader["AggregateRootId"]);
            obj.ContactPhone = Convert.ToString(reader["ContactPhone"]);
            obj.DisplayName = Convert.ToString(reader["DisplayName"]);
            obj.Email = Convert.ToString(reader["Email"]);
            obj.ID = Guid.Parse(Convert.ToString(reader["ID"]));
            obj.Password = Convert.ToString(reader["Password"]);
            obj.UserName = Convert.ToString(reader["UserName"]);
        }

        private void ConvertFromReader(SqlDataReader reader, BookDataObject obj)
        {
            obj.Author = Convert.ToString(reader["Author"]);
            obj.Description = Convert.ToString(reader["Description"]);
            obj.Inventory = Convert.ToInt32(reader["Inventory"]);
            obj.ISBN = Convert.ToString(reader["ISBN"]);
            obj.Pages = Convert.ToInt32(reader["Pages"]);
            obj.Title = Convert.ToString(reader["Title"]);
            obj.ID = Guid.Parse(Convert.ToString(reader["ID"]));
            obj.AggregateRootId = Convert.ToInt64(reader["AggregateRootId"]);
        }

        private void ConvertFromReader(SqlDataReader reader, BookTransferHistoryDataObject obj)
        {
            obj.BookAggregateRootId = Convert.ToInt64(reader["AggregateRootId"]);
            obj.BookID = Guid.Parse(Convert.ToString(reader["ID"]));
            obj.Returned = Convert.ToBoolean(reader["Returned"]);
            obj.ISBN = Convert.ToString(reader["ISBN"]);
            obj.Title = Convert.ToString(reader["Title"]);
            obj.ReturnedDate = Convert.ToDateTime(reader["ReturnedDate"]);
            obj.BorrowedDate = Convert.ToDateTime(reader["BorrowedDate"]);
        }

        private List<BookTransferHistoryDataObject> PopulateBookTransferHistory(Guid userID)
        {
            List<BookTransferHistoryDataObject> objs = new List<BookTransferHistoryDataObject>();
            string sql = @"SELECT     b.ID AS ID, b.AggregateRootId AS AggregateRootId, b.Title as Title, b.ISBN as ISBN, t.ReturnedDate as ReturnedDate, t.Returned as Returned, t.BorrowedDate as BorrowedDate 
FROM         Books AS b INNER JOIN
                      BookTransferHistory AS t ON t.BookID = b.ID
WHERE     (t.UserID = @userId)
ORDER BY t.BorrowedDate DESC";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@userId", userID)))
            {
                while (reader.Read())
                {
                    BookTransferHistoryDataObject obj = new BookTransferHistoryDataObject();
                    this.ConvertFromReader(reader, obj);
                    objs.Add(obj);
                }
                reader.Close();
            }
            return objs;
        }

        #region IQueryService Members

        public UserAccountDataObject GetUserByUserName(string userName)
        {
            UserAccountDataObject obj = null;
            string sql = @"SELECT * FROM [UserAccounts] WHERE [UserName]=@userName";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@userName", userName)))
            {
                while (reader.Read())
                {
                    obj = new UserAccountDataObject();
                    this.ConvertFromReader(reader, obj);
                    break;
                }
                reader.Close();
            }
            if (obj != null)
            {
                obj.BorrowedBooks = this.PopulateBookTransferHistory(obj.ID);
            }
            return obj;
        }

        public UserAccountDataObject GetUserByEmail(string email)
        {
            UserAccountDataObject obj = null;
            string sql = @"SELECT * FROM [UserAccounts] WHERE [Email]=@email";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@email", email)))
            {
                while (reader.Read())
                {
                    obj = new UserAccountDataObject();
                    this.ConvertFromReader(reader, obj);
                    break;
                }
                reader.Close();
            }
            if (obj != null)
            {
                obj.BorrowedBooks = this.PopulateBookTransferHistory(obj.ID);
            }
            return obj;
        }

        public bool ValidateUser(string userName, string password)
        {
            return ValidateAccount("UserAccounts", userName, password) ||
                ValidateAccount("AdminAccounts", userName, password);
        }

        public AccountType? GetAccountType(string userName)
        {
            int accountType = this.GetAccountTypeIndicator(userName);
            if (accountType == -1)
                return null;
            switch(accountType)
            {
                case 0:
                    return AccountType.Admin;
                case 1:
                    return AccountType.User;
                default:
                    break;
            }
            return null;
        }

        public List<BookDataObject> GetBooks()
        {
            BookDataObject book = null;
            List<BookDataObject> books = new List<BookDataObject>();
            string sql = "SELECT * FROM [Books]";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql))
            {
                while (reader.Read())
                {
                    book = new BookDataObject();
                    this.ConvertFromReader(reader, book);
                    books.Add(book);
                }
                reader.Close();
            }
            return books;
        }

        public BookDataObject GetBookByGuid(Guid guid)
        {
            BookDataObject book = null;
            string sql = @"SELECT * FROM [Books] WHERE [ID]=@id";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@id", guid)))
            {
                while (reader.Read())
                {
                    book = new BookDataObject();
                    this.ConvertFromReader(reader, book);
                    break;
                }
                reader.Close();
            }
            return book;
        }

        public UserAccountDataObject GetUserByGuid(Guid guid)
        {
            UserAccountDataObject userAccount = null;
            string sql = @"SELECT * FROM [UserAccounts] WHERE [ID]=@id";
            using (SqlDataReader reader = SqlHelper.ExecuteReader(this.QueryDBConnectionString,
                CommandType.Text,
                sql,
                new SqlParameter("@id", guid)))
            {
                while (reader.Read())
                {
                    userAccount = new UserAccountDataObject();
                    this.ConvertFromReader(reader, userAccount);
                    break;
                }
                reader.Close();
            }
            if (userAccount != null)
            {
                userAccount.BorrowedBooks = this.PopulateBookTransferHistory(userAccount.ID);
            }
            return userAccount;
        }

        #endregion
    }
}
