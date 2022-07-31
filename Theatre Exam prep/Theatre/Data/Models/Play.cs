using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Theatre.Data.Models.Enums;

namespace Theatre.Data.Models
{
    public class Play
    {
        public Play()
        {
            this.Casts = new HashSet<Cast>();
            this.Tickets = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public float Rating { get; set; }
        [Required]
        public Genre Genre { get; set; }
        [Required]
        [MaxLength(700)]
        public string Description { get; set; }
        [MaxLength(30)]
        [Required]
        public string Screenwriter { get; set; }
        public ICollection<Cast> Casts { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
      

    }
}
