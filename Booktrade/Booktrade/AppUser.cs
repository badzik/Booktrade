using Booktrade.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PostalCode { get; set;}
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string BankNumber { get; set; }

        [InverseProperty("Seller")]
        public virtual ICollection<Book> SellingBooks { get; set; }
        [InverseProperty("Buyer")]
        public virtual ICollection<Book> BuyingBooks { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<Message> SentMessages { get; set; }
        [InverseProperty("Receiver")]
        public virtual ICollection<Message> ReceivedMessages { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<ExchangeMessage> SentExchangeMessages { get; set; }
        [InverseProperty("Receiver")]
        public virtual ICollection<ExchangeMessage> ReceivedExchangeMessages { get; set; }
    }
}