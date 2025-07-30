using System.ComponentModel.DataAnnotations;
using JogMy.Models;

namespace JogMy.Features.Admin.ViewModels
{
    public class EditTrackViewModel
    {
        public int Id { get; set; }

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

        // Tooltip customization fields
        [Display(Name = "Custom Difficulty Level")]
        public string? CustomDifficulty { get; set; }

        [Display(Name = "Surface Type")]
        [MaxLength(100)]
        public string? SurfaceType { get; set; }

        [Display(Name = "Special Features")]
        [MaxLength(200)]
        public string? SpecialFeatures { get; set; }

        [Display(Name = "Best Time to Jog")]
        [MaxLength(100)]
        public string? BestTimeToJog { get; set; }

        [Display(Name = "Parking Available")]
        public bool HasParking { get; set; }

        [Display(Name = "Water Fountains Available")]
        public bool HasWaterFountains { get; set; }

        [Display(Name = "Restrooms Available")]
        public bool HasRestrooms { get; set; }

        [Display(Name = "Well Lit")]
        public bool IsWellLit { get; set; }

        [Display(Name = "Safety Notes")]
        [MaxLength(300)]
        public string? SafetyNotes { get; set; }

        [Display(Name = "Track Photos/Videos")]
        public List<IFormFile>? MediaFiles { get; set; }

        // Existing media files
        public List<TrackMediaViewModel> ExistingMedia { get; set; } = new();
    }

    public class TrackMediaViewModel
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public int OrderIndex { get; set; }
    }
}