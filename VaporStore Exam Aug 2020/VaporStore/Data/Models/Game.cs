using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.GameTags = new HashSet<GameTag>();
            this.Purchases = new HashSet<Purchase>();

        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Required]
        public int DeveloperId { get; set; }
        [Required]
        public Developer Developer { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public Genre Genre { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<GameTag> GameTags { get; set; }
       
    }
}
