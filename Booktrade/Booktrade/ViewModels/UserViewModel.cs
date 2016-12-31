using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class UserViewModel : UserAttributes
    {
       public string userId { get; set; }
       public ICollection<Comment> ReceivedComments { get; set; }
       public int HowManyInOnePage { get; set; }
       public ICollection<Book> Books { get; set; }
        public ICollection<Book> AllBooks { get; set; }
    }
}