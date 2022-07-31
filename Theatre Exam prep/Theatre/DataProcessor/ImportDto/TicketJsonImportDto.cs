using System.ComponentModel.DataAnnotations;

namespace Theatre.DataProcessor.ImportDto
{
    public class TicketJsonImportDto
    {
        [Range(1.00, 100.00)]
        [Required]
        public decimal Price { get; set; }
        [Range(1, 10)]
        [Required]
        public sbyte RowNumber { get; set; }
        public int PlayId { get; set; }
       
    }
}