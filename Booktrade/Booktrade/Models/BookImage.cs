using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Booktrade.Models
{
    public class BookImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public byte[] Image { get; set; }

        [ForeignKey("BookImg")]
        public int BookImgId { get; set; }

        public virtual Book BookImg { get; set; }
    }
}