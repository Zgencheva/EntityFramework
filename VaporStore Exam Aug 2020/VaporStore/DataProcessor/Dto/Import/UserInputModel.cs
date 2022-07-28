using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class UserInputModel
	{
        [Required]
        [RegularExpression(@"^[A-Z]{1}[a-z]+\s{1}[A-Z]{1}[a-z]+$")]
        public string FullName { get; set; }
        [StringLength(30, MinimumLength = 3)]
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Range(3,103)]
        [Required]
        public int Age { get; set; }
        public List<CardInputModel> Cards { get; set; }
       
    }
}
