using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinyLibraryCQRS.Presentations.Mvc3.Common;
using TinyLibraryCQRS.Presentations.Mvc3.Common.QueryServices;
using TinyLibraryCQRS.Presentations.Mvc3.Common.CommandServices;
using System.ServiceModel;

namespace TinyLibraryCQRS.Presentations.Mvc3.Controllers
{
    
    public class BooksController : Controller
    {
        //
        // GET: /Books/
        
        public ActionResult Index()
        {
            QueryServiceClient queryService = new QueryServiceClient();
            var books = queryService.GetBooks();
            queryService.Close();
            return View(books.OrderBy(p => p.Title));
        }

        [TinyLibraryCQRSAuthorize(AuthorizationAccountType.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [TinyLibraryCQRSAuthorize(AuthorizationAccountType.Admin)]
        [HttpPost]
        public ActionResult Create(BookDataObject book)
        {
            CommandServiceClient commandService = new CommandServiceClient();
            commandService.AddBook(book.Title, book.Author, book.Description, book.ISBN, book.Pages, book.Inventory);
            commandService.Close();
            return RedirectToAction("Index");
        }

        [TinyLibraryCQRSAuthorize(AuthorizationAccountType.Admin)]
        public ActionResult Edit(Guid id)
        {
            QueryServiceClient queryService = new QueryServiceClient();
            var book = queryService.GetBookByGuid(id);
            queryService.Close();
            return View(book);
        }

        [TinyLibraryCQRSAuthorize(AuthorizationAccountType.Admin)]
        [HttpPost]
        public ActionResult Edit(BookDataObject book)
        {
            CommandServiceClient commandService = new CommandServiceClient();
            commandService.UpdateBook(book.AggregateRootId, book.Title, book.Author, book.Description, book.ISBN, book.Pages, book.Inventory);
            commandService.Close();
            return RedirectToAction("Index", "Books");
        }

        public ActionResult Details(Guid id)
        {
            QueryServiceClient queryService = new QueryServiceClient();
            var book = queryService.GetBookByGuid(id);
            queryService.Close();
            return View(book);
        }

        public ActionResult Borrow(Guid id)
        {
            QueryServiceClient queryService = new QueryServiceClient();
            CommandServiceClient commandService = new CommandServiceClient();

            var book = queryService.GetBookByGuid(id);
            var user = queryService.GetUserByUserName(User.Identity.Name);
            commandService.BorrowBookToUser(book.AggregateRootId, user.AggregateRootId);

            queryService.Close();
            commandService.Close();


            return RedirectToAction("MyAccount", "Account");
        }

        public ActionResult Return(Guid id)
        {
            QueryServiceClient queryService = new QueryServiceClient();
            var book = queryService.GetBookByGuid(id);
            var user = queryService.GetUserByUserName(User.Identity.Name);
            queryService.Close();

            CommandServiceClient commandService = new CommandServiceClient();
            commandService.ReturnBookFromUser(book.AggregateRootId, user.AggregateRootId);
            commandService.Close();

            return RedirectToAction("MyAccount", "Account");
        }

    }
}
