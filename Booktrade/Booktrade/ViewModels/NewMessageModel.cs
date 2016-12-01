using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class NewMessageModel
    {
        [Required]
        public string Text { get; set; }

        public AppUser Receiver { get; set; }
        public AppUser Sender { get; set; }
    }
}