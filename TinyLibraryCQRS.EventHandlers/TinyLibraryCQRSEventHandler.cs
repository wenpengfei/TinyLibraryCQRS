/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Apworks.Events;
using Microsoft.ApplicationBlocks.Data;
using TinyLibraryCQRS.Events.Accounts;
using TinyLibraryCQRS.Events.Books;

namespace TinyLibraryCQRS.EventHandlers
{
    public class TinyLibraryCQRSEventHandler : IEventHandler<UserAccountCreatedEvent>,
        IEventHandler<AdminAccountCreatedEvent>,
        IEventHandler<BookCreatedEvent>,
        IEventHandler<BookUpdatedEvent>,
        IEventHandler<UserAccountUpdatedEvent>,
        IEventHandler<UserBorrowBookEvent>,
        IEventHandler<BookIssueStockEvent>,
        IEventHandler<UserReturnBookEvent>,
        IEventHandler<BookReceiveStockEvent>
    {
        private string queryDBConnectionString = null;
        
        private string QueryDBConnectionString
        {
            get
            {
                if (queryDBConnectionString == null)
                    queryDBConnectionString = ConfigurationManager.ConnectionStrings["QueryDBConnectionString"].ConnectionString;
                return queryDBConnectionString;
            }
        }

        private SqlParameter NullableSqlParameter(string paraName, object paraValue)
        {
            SqlParameter p = new SqlParameter(paraName, paraValue ?? DBNull.Value);
            p.IsNullable = true;
            return p;
        }

        private Guid? GetIDByAggregateRootId(string tableName, long aggregateRootId)
        {
            string sql = string.Format("SELECT [ID] FROM [{0}] WHERE [AggregateRootId]=@aggregateRootId", tableName);
            object result = SqlHelper.ExecuteScalar(this.QueryDBConnectionString, CommandType.Text,
                sql,
                new SqlParameter("@aggregateRootId", aggregateRootId));
            if (result != null)
                return (Guid)result;
            return null;
        }

        #region IHandler<UserAccountCreatedEvent> Members

        public bool Handle(UserAccountCreatedEvent message)
        {
            string insertUserAccoutSql = @"INSERT INTO [UserAccounts] 
([UserName], [Password], [DisplayName], [Email], [ContactPhone], [Address_Country], [Address_State], [Address_Street], [Address_City], [Address_Zip], [AggregateRootId])
 VALUES 
(@userName, @password, @displayName, @email, @contactPhone, @country, @state, @street, @city, @zip, @aggregateRootId)";

            var rowsAffected = SqlHelper.ExecuteNonQuery(QueryDBConnectionString, CommandType.Text, insertUserAccoutSql,
                new SqlParameter("@userName", message.UserName),
                new SqlParameter("@password", message.Password),
                new SqlParameter("@displayName", message.DisplayName),
                new SqlParameter("@email", message.Email),
                NullableSqlParameter("@contactPhone", message.ContactPhone),
                NullableSqlParameter("@country", message.ContactAddressCountry),
                NullableSqlParameter("@state", message.ContactAddressState),
                NullableSqlParameter("@street", message.ContactAddressStreet),
                NullableSqlParameter("@city", message.ContactAddressCity),
                NullableSqlParameter("@zip", message.ContactAddressZip),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));

            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<AdminAccountCreatedEvent> Members

        public bool Handle(AdminAccountCreatedEvent message)
        {
            string insertAdminAccountSql = @"INSERT INTO [AdminAccounts] 
([UserName], [Password], [DisplayName], [Email], [AggregateRootId])
 VALUES 
(@userName, @password, @displayName, @email, @aggregateRootId)";
            var rowsAffected = SqlHelper.ExecuteNonQuery(QueryDBConnectionString, CommandType.Text, insertAdminAccountSql,
                new SqlParameter("@userName", message.UserName),
                new SqlParameter("@password", message.Password),
                new SqlParameter("@displayName", message.DisplayName),
                new SqlParameter("@email", message.Email),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<BookCreatedEvent> Members

        public bool Handle(BookCreatedEvent message)
        {
            string insertBookSql = @"INSERT INTO [Books] 
([Title], [Author], [Description], [ISBN], [Pages], [Inventory], [AggregateRootId])
 VALUES 
(@title, @author, @description, @isbn, @pages, @inventory, @aggregateRootId)";
            var rowsAffected = SqlHelper.ExecuteNonQuery(QueryDBConnectionString, CommandType.Text, insertBookSql,
                new SqlParameter("@title", message.Title),
                new SqlParameter("@author", message.Author),
                new SqlParameter("@description", message.Description),
                new SqlParameter("@isbn", message.ISBN),
                new SqlParameter("@pages", message.Pages),
                new SqlParameter("@inventory", message.Inventory),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<BookUpdatedEvent> Members

        public bool Handle(BookUpdatedEvent message)
        {
            string updateBookSql = @"UPDATE [Books] SET 
[Title]=@title, [Author]=@author, [Description]=@description, [ISBN]=@isbn, [Pages]=@pages, [Inventory]=@inventory
 WHERE [AggregateRootId]=@aggregateRootId";
            var rowsAffected = SqlHelper.ExecuteNonQuery(QueryDBConnectionString, CommandType.Text, updateBookSql,
                new SqlParameter("@title", message.Title),
                new SqlParameter("@author", message.Author),
                new SqlParameter("@description", message.Description),
                new SqlParameter("@isbn", message.ISBN),
                new SqlParameter("@pages", message.Pages),
                new SqlParameter("@inventory", message.Inventory),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<UserAccountUpdatedEvent> Members

        public bool Handle(UserAccountUpdatedEvent message)
        {
            string updateUserAccountSql = @"UPDATE [UserAccounts] SET 
[DisplayName]=@displayName, [Email]=@email, [ContactPhone]=@contactPhone, [Address_Country]=@country, [Address_State]=@state, [Address_Street]=@street, [Address_City]=@city, [Address_Zip]=@zip
 WHERE [AggregateRootId]=@aggregateRootId";
            var rowsAffected = SqlHelper.ExecuteNonQuery(QueryDBConnectionString, CommandType.Text, updateUserAccountSql,
                new SqlParameter("@displayName", message.DisplayName),
                new SqlParameter("@email", message.Email),
                NullableSqlParameter("@contactPhone", message.ContactPhone),
                NullableSqlParameter("@country", message.ContactAddressCountry),
                NullableSqlParameter("@state", message.ContactAddressState),
                NullableSqlParameter("@street", message.ContactAddressStreet),
                NullableSqlParameter("@city", message.ContactAddressCity),
                NullableSqlParameter("@zip", message.ContactAddressZip),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<UserBorrowBookEvent> Members

        public bool Handle(UserBorrowBookEvent message)
        {
            string insertHistorySql = @"INSERT INTO [BookTransferHistory] 
 ([UserID], [BookID], [Returned], [ReturnedDate], [BorrowedDate]) VALUES
 (@userId, @bookId, @returned, @returnedDate, @borrowedDate)";
            Guid? userGUID = this.GetIDByAggregateRootId("UserAccounts", message.UserAccountId);
            if (userGUID == null)
                throw new InvalidOperationException(string.Format("Cannot find the UserAccount with the AggregateRootId={0}", message.UserAccountId));
            Guid? bookGUID = this.GetIDByAggregateRootId("Books", message.BookId);
            if (bookGUID==null)
                throw new InvalidOperationException(string.Format("Cannot find the Book with the AggregateRootId={0}", message.BookId));
            var rowsAffected = SqlHelper.ExecuteNonQuery(this.QueryDBConnectionString,
                CommandType.Text,
                insertHistorySql,
                new SqlParameter("@userId", userGUID.Value),
                new SqlParameter("@bookId", bookGUID.Value),
                new SqlParameter("@returned", false),
                new SqlParameter("@returnedDate", SqlDateTime.MinValue),
                new SqlParameter("@borrowedDate", new SqlDateTime(message.TransferDateTime)));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<BookIssueStockEvent> Members

        public bool Handle(BookIssueStockEvent message)
        {
            string updateBookStockSql = @"UPDATE [Books] SET [Inventory]=[Inventory]-@quantity WHERE [AggregateRootId]=@aggregateRootId";
            var rowsAffected = SqlHelper.ExecuteNonQuery(this.QueryDBConnectionString,
                CommandType.Text,
                updateBookStockSql,
                new SqlParameter("@quantity", message.Quantity),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<UserReturnBookEvent> Members

        public bool Handle(UserReturnBookEvent message)
        {
            string updateHistorySql = @"UPDATE [BookTransferHistory] SET 
[Returned]=@returned, [ReturnedDate]=@returnedDate WHERE [UserID]=@userId AND [BookID]=@bookId";
            Guid? userId = this.GetIDByAggregateRootId("UserAccounts", message.UserAccountId);
            if (userId == null)
                throw new InvalidOperationException(string.Format("Cannot find the UserAccount with the AggregateRootId={0}", message.UserAccountId));
            Guid? bookId = this.GetIDByAggregateRootId("Books", message.BookId);
            if (bookId == null)
                throw new InvalidOperationException(string.Format("Cannot find the Book with the AggregateRootId={0}", message.BookId));
            var rowsAffected = SqlHelper.ExecuteNonQuery(this.QueryDBConnectionString,
                CommandType.Text,
                updateHistorySql,
                new SqlParameter("@returned", true),
                new SqlParameter("@returnedDate", new SqlDateTime(message.TransferDateTime)),
                new SqlParameter("@userId", userId.Value),
                new SqlParameter("@bookId", bookId.Value));
            return rowsAffected > 0;
        }

        #endregion

        #region IHandler<BookReceiveStockEvent> Members

        public bool Handle(BookReceiveStockEvent message)
        {
            string updateBookStockSql = @"UPDATE [Books] SET [Inventory]=[Inventory]+@quantity WHERE [AggregateRootId]=@aggregateRootId";
            var rowsAffected = SqlHelper.ExecuteNonQuery(this.QueryDBConnectionString,
                CommandType.Text,
                updateBookStockSql,
                new SqlParameter("@quantity", message.Quantity),
                new SqlParameter("@aggregateRootId", message.AggregateRootId));
            return rowsAffected > 0;
        }

        #endregion
    }
}
