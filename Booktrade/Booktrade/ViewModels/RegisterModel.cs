using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class RegisterModel : UserAttributes
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //[Required] test do facebooka
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}