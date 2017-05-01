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
            return View(GetConversationPreviews());
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
            model.messages = currentUser.SentMessages.Concat(currentUser.ReceivedMessages).Where(x => x.Receiver.Id == interlocutor.Id || x.Sender.Id == interlocutor.Id).OrderByDescending(x => x.SendDate).ToList();
            if (model.messages.First().Receiver.Id == currentUser.Id && !model.messages.First().isRead)
            {
                var context = new AppDbContext();
                Message message = context.Messages.Find(model.messages.First().MessageId);
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
            return RedirectToAction("Conversation", "Message", new { interlocutorEmail = receiverEmail });
        }

        public static ConversationPreviewsModel GetConversationPreviews()
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
            return model;
        }

        public static string CalculateMessageTime(DateTime time)
        {
            TimeSpan t;
            string text = "";
            t = DateTime.Now - time;
            if (t.TotalMinutes < 60)
            {
                text = Math.Ceiling(t.TotalMinutes) + " min. temu";
            }
            else if (t.TotalHours < 24)
            {
                text = Math.Ceiling(t.TotalHours) + " godz. temu";
            }
            else if (t.TotalDays <= 31)
            {
                text = Math.Ceiling(t.TotalDays) + " dni temu";
            }
            else if (t.TotalDays > 31)
            {
                text = Math.Ceiling(t.TotalDays / 31) + "mies. temu";
            }
            return text;
        }

        public static int CountUnreadMessages()
        {
            ConversationPreviewsModel model = MessageController.GetConversationPreviews();
            int unreadMessages = 0;
            foreach (ConversationPreviewModel m in model.Conversations)
            {
                if (m.LastMessage.Receiver.Id == System.Web.HttpContext.Current.User.Identity.GetUserId() && !m.LastMessage.isRead)
                {
                    unreadMessages++;
                }
            }
            return unreadMessages;
        }
    }
}