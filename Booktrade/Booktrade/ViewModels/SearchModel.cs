using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class SearchModel
    {
        public string Phrase { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public bool ForExchange { get; set; }
        public bool ForSell { get; set; }
        public float PriceFrom { get; set; }
        public float PriceTo { get; set; }
        public string Publisher { get; set; }
        public int PublicationYear { get; set; }
        public string SortBy { get; set; }
        public string Category { get; set; }
        public int CurrentPage { get; set; }
        public ICollection<Book> Results { get; set; }
    }
}