using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booktrade.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }
  
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string Publisher { get; set; }
        public bool Changeable { get; set; }
        public float? Price { get; set; }
        public DateTime AddDate { get; set; }
        public bool isSold { get; set; }
        public bool isChanged { get; set; }
        [ForeignKey("Seller")]
        public string SellerId { get; set; }
        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }

        public virtual AppUser Seller { get; set; }     
        public virtual AppUser Buyer { get; set; }

        [InverseProperty("BookImg")]
        public virtual ICollection<BookImage> ImagesForBook { get; set; }

        [InverseProperty("DeliveryPrices")]
        public virtual ICollection<Delivery> DeliveryforBook { get; set; }

        public ICollection<ExchangeMessage> ExchangeMessages { get; set; }
    }
}