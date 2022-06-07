using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Positions
{
    public class PositionsAllViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
