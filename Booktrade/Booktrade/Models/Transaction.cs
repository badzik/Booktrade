using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public bool Exchanged { get; set; }
        public bool SellerCommented { get; set; }
        public bool BuyerCommented { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; }
        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }
        [ForeignKey("SoldBook")]
        public int BookId { get; set; }
        [ForeignKey("ExMessage")]
        public int? ExchangeMessageId { get; set; }
        [ForeignKey("SelectedDelivery")]
        public int? DeliveryId { get; set; }


        public virtual AppUser Seller { get; set; }
        public virtual AppUser Buyer { get; set; }
        public virtual Book SoldBook { get; set; }
        public virtual ExchangeMessage ExMessage { get; set; }
        public virtual Delivery SelectedDelivery { get; set; }
        public virtual Comment FromSellerComment { get; set; }
        public virtual Comment FromBuyerComment { get; set; }

    }
}