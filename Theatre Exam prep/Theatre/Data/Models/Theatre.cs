using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Theatre.Data.Models
{
    public class Theatre
    {
        public Theatre()
        {
            this.Tickets = new HashSet<Ticket>();
        }
 
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string Name { get; set; }
        [Required]
        public sbyte NumberOfHalls { get; set; }
        [MaxLength(30)]
        [Required]
        public string Director { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

    }
}
