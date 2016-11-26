using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class SellBookModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Gatunek")]
        public string Genre { get; set; }
        [Required]
        [Display(Name = "Autor")]
        public string Author { get; set; }
        [Display(Name = "Wydawca")]
        public string Publisher { get; set; }
        [Display(Name = "Data wydania")]
        public DateTime? PublicationDate { get; set; }
        [Display(Name = "Chęć wymiany")]
        public bool Changeable { get; set; }
        [Range(0, 5000)]
        [Display(Name = "Cena")]
        public float? Price { get; set; }
        public HttpPostedFileBase BookImage { get; set; }
        public AppUser Seller { get; set; }

    }
}