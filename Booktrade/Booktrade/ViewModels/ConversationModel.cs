using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class ConversationModel
    {
        public ICollection<Message> messages { get; set; }
        public NewMessageModel NewMessage { get; set; }

        public string Email { get; set; }
    }
}