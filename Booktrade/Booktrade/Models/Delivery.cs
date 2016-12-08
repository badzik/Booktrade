using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.Models
{
    public class Delivery
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }


        [ForeignKey("DeliveryPrices")]
        public int DeliveryPriceId { get; set; }

        public virtual Book DeliveryPrices { get; set; }
    }
}