using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theatre.Data.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public sbyte RowNumber { get; set; }
        [ForeignKey(nameof(Play))]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        [ForeignKey(nameof(Data.Models.Theatre))]
        public int TheatreId { get; set; }
        public Theatre Theatre { get; set; }

    }
}