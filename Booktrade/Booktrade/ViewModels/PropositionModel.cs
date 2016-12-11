using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class PropositionModel
    {
        public Book Interested { get; set; }
        public ICollection<Book> YourBooks { get; set; }
        public IEnumerable<string> SelectedBooks { get; set; }
        public string Text { get; set; }
    }
}