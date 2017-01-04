using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class TransactionsModel
    {
        public ICollection<Transaction> TransactionsAsSeller { get; set; }
        public ICollection<Transaction> TransactionsAsBuyer { get; set; }
    }
}