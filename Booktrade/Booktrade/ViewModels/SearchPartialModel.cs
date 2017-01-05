using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class SearchPartialModel
    {
        public ICollection<Book> Books { get; set; }
        public int MaxPages { get; set; }
    }
}