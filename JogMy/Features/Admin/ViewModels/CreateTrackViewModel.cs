using System.ComponentModel.DataAnnotations;

namespace JogMy.Features.Admin.ViewModels
{
    public class CreateTrackViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Coordinates (JSON)")]
        public string Coordinates { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 100)]
        [Display(Name = "Distance (km)")]
        public double Distance { get; set; }

        [Required]
        [MaxLength(50)]
        public string Region { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}