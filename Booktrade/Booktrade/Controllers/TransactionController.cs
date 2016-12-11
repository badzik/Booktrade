using Booktrade.Models;
using Booktrade.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booktrade.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Proposition(string bookId)
        {
            PropositionModel model = GetPropositionModel(bookId);
            if (model.Interested == null)
            {
                return RedirectToAction("Information", "Info", new { text = "AccessDenied" });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Proposition(PropositionModel model)
        {
            if (model.SelectedBooks == null)
            {
                ModelState.AddModelError("", "Musisz zaproponować co najmniej jedną książkę");
                model = GetPropositionModel(Request.QueryString["bookId"]);
                return View(model);
            }
            AppUser currentUser;
            int bookId = 0;
            Int32.TryParse(Request.QueryString["bookId"], out bookId);
            Book interested;
            ICollection<Book> proposedBooks = new List<Book>();
            using (var context = new AppDbContext())
            {
                currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
                interested = context.Books.Find(bookId);
                foreach (string id in model.SelectedBooks)
                {
                    proposedBooks.Add(context.Books.Find(Int32.Parse(id)));
                }
                if (interested != null)
                {
                    ExchangeMessage eMessage = new ExchangeMessage()
                    {
                        Text = model.Text,
                        SendDate = DateTime.Now,
                        SenderId = currentUser.Id,
                        Sender = currentUser,
                        ReceiverId = interested.SellerId,
                        Receiver = interested.Seller,
                        ForBook = interested,
                        BookId = interested.BookId,
                        Accepted=false,
                        ProposedBooks = proposedBooks
                    };
                    context.ExchangeMessages.Add(eMessage);
                    context.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Information", "Info", new { text = "AccessDenied" });
                }

            }
            //TODO: Return to book view after sending a propostion
            return RedirectToAction("Index", "Home");
        }

        private PropositionModel GetPropositionModel(string bookId)
        {
            PropositionModel model = new PropositionModel();
            var context = new AppDbContext();
            model.YourBooks = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).SellingBooks;
            int id = 0;
            Int32.TryParse(bookId, out id);
            model.Interested = context.Books.Find(id);
            return model;
        }

        public ActionResult Propositions()
        {
            PropositionsModel model = new PropositionsModel();
            var context = new AppDbContext();

            model.ReceivedPropositions = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).ReceivedExchangeMessages.Where(x => x.Accepted == false).ToList();
            model.SentPropositions = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()).SentExchangeMessages.Where(x=>x.Accepted==false).ToList();

            return View(model);
        }

        public ActionResult AcceptProposition(string propositionId)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            int id = 0;
            Int32.TryParse(propositionId, out id);
            ExchangeMessage eMessage = context.ExchangeMessages.Find(id);
            Transaction transaction;
            Book book = context.Books.Find(eMessage.BookId);
            if (currentUser != null && eMessage != null && eMessage.ReceiverId == currentUser.Id)
            {
                eMessage.Accepted = true;
                book.isChanged = true;
                foreach (Book b in eMessage.ProposedBooks) b.isChanged = true;
                transaction = new Transaction()
                {
                    Exchanged = true,
                    SellerCommented=false,
                    BuyerCommented=false,
                    SellerId=eMessage.SenderId,
                    Seller=eMessage.Sender,
                    BuyerId=currentUser.Id,
                    Buyer=currentUser,
                    BookId=eMessage.BookId,
                    SoldBook=eMessage.ForBook,
                    ExchangeMessageId=eMessage.ExchangeMessageId,
                    ExMessage=eMessage
                };
                context.Transactions.Add(transaction);
                context.SaveChanges();
                return RedirectToAction("Propositions", "Transaction");
            }
            else
                return RedirectToAction("Information", "Info", new { text = "Error" });
        }

        public ActionResult DeclineProposition(string propositionId)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            int id = 0;
            Int32.TryParse(propositionId, out id);
            ExchangeMessage eMessage = context.ExchangeMessages.Find(id);
            if (currentUser != null && eMessage != null && eMessage.ReceiverId == currentUser.Id)
            {
                context.ExchangeMessages.Remove(eMessage);
                context.SaveChanges();
                return RedirectToAction("Propositions", "Transaction");
            }
            else
                return RedirectToAction("Information", "Info", new { text = "Error" });
        }

        public ActionResult CancelProposition(string propositionId)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            int id = 0;
            Int32.TryParse(propositionId, out id);
            ExchangeMessage eMessage = context.ExchangeMessages.Find(id);
            if (currentUser != null && eMessage != null && eMessage.SenderId == currentUser.Id)
            {
                context.ExchangeMessages.Remove(eMessage);
                context.SaveChanges();
                return RedirectToAction("Propositions", "Transaction");
            }
            else
                return RedirectToAction("Information", "Info", new { text = "Error" });

        }
    }
}