using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class ConversationPreviewModel
    {
        public Message LastMessage { get; set; }
        public AppUser Interlocutor { get; set; }

    }
}