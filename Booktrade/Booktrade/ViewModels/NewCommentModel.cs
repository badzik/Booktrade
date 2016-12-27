using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Booktrade.ViewModels
{
    public class NewCommentModel
    {
        public int Rating { get; set; }
        [Required]
        public string Text { get; set; }
        public string CommentFor { get; set; }
    }
}