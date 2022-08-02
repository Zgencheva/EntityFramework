using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeesJsonImportModel
    {
        [MaxLength(40)]
        [MinLength(3)]
        [Required]
        [RegularExpression(@"^[a-zA-z\d]+$")]
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
        public string Phone { get; set; }
        public int[] Tasks { get; set; }
    }
}
