/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using Apworks;
using Apworks.Snapshots;
using TinyLibraryCQRS.Events.Books;
using TinyLibraryCQRS.Model.Snapshots;

namespace TinyLibraryCQRS.Model
{
    public partial class Book : SourcedAggregateRoot
    {
        #region Ctor
        internal Book() : base() { }
        internal Book(long id) : base(id) { }
        #endregion

        #region Public Properties
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Description { get; private set; }
        public string ISBN { get; private set; }
        public int Pages { get; private set; }
        public int Inventory { get; private set; }
        #endregion

        #region Factory Methods
        public static Book Create(long? id, string title, string author, string description,
            string isbn, int pages, int inventory)
        {
            Book book = null;
            if (id != null && id.HasValue)
                book = new Book(id.Value);
            else
                book = new Book();
            book.RaiseEvent<BookCreatedEvent>(new BookCreatedEvent
                {
                    Title = title,
                    Author = author,
                    Description = description,
                    ISBN = isbn,
                    Pages = pages,
                    Inventory = inventory
                });
            return book;
        }

        public static Book Create(string title, string author, string description,
            string isbn, int pages, int inventory)
        {
            return Create(null, title, author, description, isbn, pages, inventory);
        }
        #endregion

        #region Public Methods
        public void IssueStock(int numOfBooks)
        {
            if (Inventory < numOfBooks)
                throw new DomainException("Cannot issue {0} books since the inventory is not enough. Current inventory: {1}.", 
                    numOfBooks, Inventory);
            this.RaiseEvent<BookIssueStockEvent>(new BookIssueStockEvent
                {
                    Quantity = numOfBooks
                });
        }

        public void ReceiveStock(int numOfBooks)
        {
            this.RaiseEvent<BookReceiveStockEvent>(new BookReceiveStockEvent
                {
                    Quantity = numOfBooks,
                });
        }

        public void UpdateBasicInformation(string title, string author, string description,
            string isbn, int pages, int inventory)
        {
            this.RaiseEvent<BookUpdatedEvent>(new BookUpdatedEvent
                {
                    Title = title,
                    Author = author,
                    Description = description,
                    ISBN = isbn,
                    Pages = pages,
                    Inventory = inventory
                });
        }

        public override string ToString()
        {
            return this.Title;
        }
        #endregion

        #region Domain Event Handlers
        [Handles(typeof(BookCreatedEvent))]
        private void HandleBookCreatedEvent(BookCreatedEvent e)
        {
            this.Title = e.Title;
            this.Author = e.Author;
            this.Description = e.Description;
            this.ISBN = e.ISBN;
            this.Pages = e.Pages;
            this.Inventory = e.Inventory;
        }

        [Handles(typeof(BookIssueStockEvent))]
        private void HandleBookIssueStockEvent(BookIssueStockEvent e)
        {
            this.Inventory -= e.Quantity;
        }

        [Handles(typeof(BookReceiveStockEvent))]
        private void HandleBookReceiveStockEvent(BookReceiveStockEvent e)
        {
            this.Inventory += e.Quantity;
        }

        [Handles(typeof(BookUpdatedEvent))]
        private void HandleBookUpdatedEvent(BookUpdatedEvent e)
        {
            this.Title = e.Title;
            this.Author = e.Author;
            this.Description = e.Description;
            this.ISBN = e.ISBN;
            this.Pages = e.Pages;
            this.Inventory = e.Inventory;
        }

        #endregion

        #region Protected Methods
        protected override void DoBuildFromSnapshot(ISnapshot snapshot)
        {
            BookSnapshot s = (BookSnapshot)snapshot;
            this.Title = s.Title;
            this.Author = s.Author;
            this.Description = s.Description;
            this.Pages = s.Pages;
            this.Inventory = s.Inventory;
            this.ISBN = s.ISBN;
        }

        protected override ISnapshot DoCreateSnapshot()
        {
            return new BookSnapshot
            {
                Title = this.Title,
                Author = this.Author,
                Description = this.Description,
                Pages = this.Pages,
                Inventory = this.Inventory,
                ISBN = this.ISBN
            };
        }
        #endregion
    }
}
