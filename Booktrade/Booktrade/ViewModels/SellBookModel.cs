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
        [Display(Name = "Tytuł")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Krótki opis")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Gatunek")]
        public string Genre { get; set; }
        [Required]
        [Display(Name = "Autor")]
        public string Author { get; set; }
        [Required]
        [Display(Name = "Data wydania")]
        public DateTime PublicationDate { get; set; }
        [Display(Name = "Wydawnictwo")]
        public string Publisher { get; set; }
        [Display(Name = "Zgadzasz się na wymianę?")]
        public bool Changeable { get; set; }
        [Required]
        [Display(Name = "Cena")]
        public float Price { get; set; }
        [Display(Name = "Zdjęcie")]
        public HttpPostedFileBase BookImage { get; set; }


        public AppUser Seller { get; set; }

    }
}