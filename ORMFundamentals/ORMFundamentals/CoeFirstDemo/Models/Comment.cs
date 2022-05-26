using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoeFirstDemo.Models
{
    public  class Comment
    {
        [Key]
        public int Id { get; set; }

        public int NewsId { get; set; }

        public virtual News News { get; set; }

        [MaxLength(50)]
        public string Author { get; set; }
        //Required = not null
        //[Required]
        public string Content { get; set; }
    }
}
