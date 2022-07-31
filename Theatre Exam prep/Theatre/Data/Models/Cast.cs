using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Theatre.Data.Models
{
    public class Cast
    {
        public int Id { get; set; }
        [MaxLength(30)]
        [Required]
        public string FullName { get; set; }
        [Required]
        public bool IsMainCharacter { get; set; }
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }
        [ForeignKey(nameof(Play))]
        public int PlayId { get; set; }
        public Play Play { get; set; }

    }
}