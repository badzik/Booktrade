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
    public class MessageController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public MessageController() : this(Startup.UserManagerFactory.Invoke())
        {

        }

        public MessageController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Message
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewMessage(string receiverId)
        {
            if (receiverId == null)
            {
                return View();
            }
            AppUser receiver = userManager.FindById(receiverId);
            NewMessageModel model = new NewMessageModel()
            {
                Receiver = receiver,
                Sender = userManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()),
                Text = null
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult NewMessage(NewMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Information", "Info", new { text = "MessageSendFail" });
            }
            string receiverId = Request.QueryString["receiverId"];
            //TODO: Return to book view after sending a message
            //string bookId= Request.QueryString["bookId"];
            var context = new AppDbContext();
            Message message = new Message()
            {
                isRead = false,
                Receiver = context.Users.Find(receiverId),
                ReceiverId = receiverId,
                SendDate = DateTime.Now,
                Text = model.Text,
                Sender = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),
                SenderId = System.Web.HttpContext.Current.User.Identity.GetUserId()
            };
            context.Messages.Add(message);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Messages()
        {
            var context = new AppDbContext();
            Message lastMessage = null;
            List<Message> tempList = new List<Message>();
            AppUser currentUser = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            ConversationPreviewsModel model = new ConversationPreviewsModel()
            {
                Conversations = new List<ConversationPreviewModel>()
            };
            HashSet<string> interlocutorsIds = new HashSet<string>();
            //create a list of interlocutors
            foreach (Message message in currentUser.SentMessages.Concat(currentUser.ReceivedMessages))
            {
                if (message.ReceiverId == currentUser.Id) interlocutorsIds.Add(message.SenderId);
                else interlocutorsIds.Add(message.ReceiverId);
            }
            //find last sent or received message in conversation
            foreach (string interlocutorId in interlocutorsIds)
            {
                tempList = currentUser.SentMessages.Concat(currentUser.ReceivedMessages).Where(x => x.ReceiverId == interlocutorId || x.SenderId == interlocutorId).OrderBy(x => x.SendDate).ToList();
                lastMessage = tempList.Last();
                model.Conversations.Add(new ConversationPreviewModel()
                {
                    Interlocutor = context.Users.Find(interlocutorId),
                    LastMessage = lastMessage
                });
            }
            return View(model);
        }

        public ActionResult Conversation(string interlocutorEmail)
        {
            AppUser currentUser = userManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            AppUser interlocutor = userManager.FindByEmail(interlocutorEmail);
            ConversationModel model = new ConversationModel();
            model.Email = interlocutorEmail;
            model.NewMessage = new NewMessageModel()
            {
                Receiver = null,
                Sender = currentUser,
                Text = null
            };
            model.messages = currentUser.SentMessages.Concat(currentUser.ReceivedMessages).Where(x => x.Receiver.Id == interlocutor.Id || x.Sender.Id == interlocutor.Id).OrderBy(x => x.SendDate).ToList();
            if (model.messages.Last().Receiver.Id == currentUser.Id && !model.messages.Last().isRead)
            {
                var context = new AppDbContext();
                Message message = context.Messages.Find(model.messages.Last().MessageId);
                message.isRead = true;
                context.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Conversation(ConversationModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Information", "Info", new { text = "MessageSendFail" });
            }
            string receiverEmail = Server.UrlDecode(Request.QueryString["interlocutorEmail"]);
            var context = new AppDbContext();
            AppUser receiver = (AppUser)context.Users.Where(x => x.Email == receiverEmail).FirstOrDefault();
            Message message = new Message()
            {
                isRead = false,
                Receiver = receiver,
                ReceiverId = receiver.Id,
                SendDate = DateTime.Now,
                Text = model.NewMessage.Text,
                Sender = context.Users.Find(System.Web.HttpContext.Current.User.Identity.GetUserId()),
                SenderId = System.Web.HttpContext.Current.User.Identity.GetUserId()
            };
            context.Messages.Add(message);
            context.SaveChanges();
            return RedirectToAction("Conversation", "Message", new {interlocutorEmail=receiverEmail });
        }
    }
}