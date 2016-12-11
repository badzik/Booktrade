using Booktrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class PropositionsModel
    {
        public ICollection<ExchangeMessage> ReceivedPropositions { get; set; } 
        public ICollection<ExchangeMessage> SentPropositions { get; set; }
    }
}