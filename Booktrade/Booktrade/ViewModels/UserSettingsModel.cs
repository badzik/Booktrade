using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class UserSettingsModel : UserAttributes
    {
        [Required]
        [Display(Name = "Numer konta")]
        public string BankAccount { get; set; }
    }
}