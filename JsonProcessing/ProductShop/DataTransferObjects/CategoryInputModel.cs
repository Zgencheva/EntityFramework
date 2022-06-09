using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductShop.DataTransferObjects
{
    public class CategoryInputModel
    {
        [Required]
        public string Name { get; set; }
    }
}
