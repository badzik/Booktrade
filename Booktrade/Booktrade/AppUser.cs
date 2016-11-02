using Booktrade.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
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

        public virtual ICollection<Book> Books { get; set; }
    }
}