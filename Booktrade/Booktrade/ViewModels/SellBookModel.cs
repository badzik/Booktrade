using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class SellBookModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Publisher { get; set; }
        public bool Changeable { get; set; }
        [Required]
        public float Price { get; set; }
        public HttpPostedFileBase BookImage { get; set; }


        public AppUser Seller { get; set; }

    }
}