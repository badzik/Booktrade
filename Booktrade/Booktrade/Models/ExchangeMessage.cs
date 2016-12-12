using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.Models
{
    public class ExchangeMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExchangeMessageId { get; set; }
        public string Text { get; set; }
        public DateTime SendDate { get; set; }
        public bool Accepted { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        [ForeignKey("ForBook")]
        public int BookId { get; set; }

        public virtual AppUser Sender { get; set; }
        public virtual AppUser Receiver { get; set; }
        public virtual Book ForBook { get; set; }
        public virtual ICollection<Book> ProposedBooks { get; set; }
    }
}