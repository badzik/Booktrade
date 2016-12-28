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
            if(model.Interested.Seller.Id== System.Web.HttpContext.Current.User.Identity.GetUserId())
            {
                return RedirectToAction("Information", "Info", new { text = "Error" });
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
                if (interested != null && interested.Changeable)
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
                    return RedirectToAction("Information", "Info", new { text = "Error" });
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
            ICollection<ExchangeMessage> allReceivedExMessages = eMessage.Sender.ReceivedExchangeMessages;
            ICollection<ExchangeMessage> allSentExMesssages = eMessage.Sender.SentExchangeMessages;
            if (currentUser != null && eMessage != null && eMessage.ReceiverId == currentUser.Id)
            {
                eMessage.Accepted = true;
                book.isChanged = true;
                book.Buyer = eMessage.Sender;
                book.BuyerId = eMessage.SenderId;
                foreach (Book b in eMessage.ProposedBooks) b.isChanged = true;


                //deleting propositions related to changed book
                foreach(ExchangeMessage e in currentUser.ReceivedExchangeMessages.ToList())
                {
                    if (e.BookId == book.BookId && e.ExchangeMessageId!=eMessage.ExchangeMessageId)
                    {
                        context.ExchangeMessages.Remove(e);
                    }
                }
                foreach (ExchangeMessage e in currentUser.SentExchangeMessages.ToList())
                {
                    if (e.ProposedBooks.Contains(book))
                    {
                        context.ExchangeMessages.Remove(e);
                    }
                }


                //deleting propositions related to proposed books by other user
                foreach (ExchangeMessage e in allReceivedExMessages.ToList())
                {
                    if (eMessage.ProposedBooks.Any(x => x.BookId == e.BookId))
                    {
                        context.ExchangeMessages.Remove(e);
                    }
                }
                foreach(ExchangeMessage e in allSentExMesssages.ToList())
                {
                    foreach(Book b in eMessage.ProposedBooks)
                    {
                        if (e.ProposedBooks.Contains(b) && e.ExchangeMessageId!=eMessage.ExchangeMessageId)
                        {
                            context.ExchangeMessages.Remove(e);
                        }
                    }
                }


                transaction = new Transaction()
                {
                    Exchanged = true,
                    SellerCommented = false,
                    BuyerCommented = false,
                    SellerId = eMessage.SenderId,
                    Seller = eMessage.Sender,
                    BuyerId = currentUser.Id,
                    Buyer = currentUser,
                    BookId = eMessage.BookId,
                    SoldBook = eMessage.ForBook,
                    ExchangeMessageId = eMessage.ExchangeMessageId,
                    ExMessage = eMessage,
                    SelectedDelivery = null,
                    DeliveryId = null
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

        public ActionResult NewComment(string transactionId)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            NewCommentModel model = new NewCommentModel();
            int id = 0;
            Int32.TryParse(transactionId, out id);
            Transaction transaction = context.Transactions.Find(id);
            if(transaction==null || (transaction.Buyer.Id!=currentUser.Id && transaction.Seller.Id != currentUser.Id))
            {
                return RedirectToAction("Information", "Info", new { text = "Error" });
            }
            if(transaction.Buyer.Id == currentUser.Id)
            {
                model.CommentFor = transaction.Seller.UserName;
            }else
            {
                model.CommentFor = transaction.Buyer.UserName;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult NewComment(NewCommentModel model)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Comment comment = new Comment();
            int transactionId = 0;
            Int32.TryParse(Request.QueryString["transactionId"], out transactionId);
            Transaction tr = context.Transactions.Find(transactionId);
            comment.Description = model.Text;
            comment.Rating = model.Rating;
            comment.CommentDate = DateTime.Now;
            if (tr.SellerId == currentUser.Id)
            {
                if(tr.SellerCommented==true) return RedirectToAction("Information", "Info", new { text = "Error" }); //protect against multiple comments
                comment.ReceiverId = tr.BuyerId;
                comment.Receiver = tr.Buyer;
                comment.Sender = currentUser;
                comment.SenderId = currentUser.Id;
                comment.SellerSideTransaction = tr;
                context.Comments.Add(comment);
                tr.SellerCommented = true;
                tr.FromSellerComment = comment;
                context.SaveChanges();
            }
            if (tr.BuyerId == currentUser.Id)
            {
                if (tr.BuyerCommented == true) return RedirectToAction("Information", "Info", new { text = "Error" });
                comment.ReceiverId = tr.SellerId;
                comment.Receiver = tr.Seller;
                comment.Sender = currentUser;
                comment.SenderId = currentUser.Id;
                comment.BuyerSideTransaction = tr;
                context.Comments.Add(comment);
                tr.BuyerCommented = true;
                tr.FromBuyerComment = comment;
                context.SaveChanges();
            }
            return RedirectToAction("Transactions", "Transaction");
        }

        public ActionResult Transactions()
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            TransactionsModel model = new TransactionsModel()
            {
                TransactionsAsBuyer = currentUser.BuyerTransactions,
                TransactionsAsSeller = currentUser.SellerTransactions
            };
            return View(model);
        }

        public ActionResult ExchangeCard(string transactionId)
        {
            return GetTransactionById(transactionId);
        }

        public ActionResult PayCard(string transactionId)
        {
            return GetTransactionById(transactionId);
        }

        private ActionResult GetTransactionById(string transactionId)
        {
            var context = new AppDbContext();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            int id = 0;
            Int32.TryParse(transactionId, out id);
            Transaction model = context.Transactions.Find(id);
            if (model.Buyer.Id == currentUser.Id && model.Exchanged == true)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Information", "Info", new { text = "Error" });
            }
        }
    }
}