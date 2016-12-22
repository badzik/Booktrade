using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }

        public int Rating { get; set; }
        public string Description { get; set; }
        public DateTime CommentDate { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        [ForeignKey("Sender")]
        public string SenderId { get; set; }

        public virtual AppUser Receiver { get; set; }
        public virtual AppUser Sender { get; set; }
        public virtual Transaction SellerSideTransaction { get; set; }
        public virtual Transaction BuyerSideTransaction { get; set; }
    }
}