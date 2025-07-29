using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class JoggingTrack
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Coordinates { get; set; } = string.Empty; // JSON string of lat/lng points

        [Required]
        [Range(0.1, 100)]
        public double Distance { get; set; } // in kilometers

        [Required]
        [MaxLength(50)]
        public string Region { get; set; } = string.Empty; // KL or Selangor

        public string? Description { get; set; }

        // Tooltip customization fields
        public string? CustomDifficulty { get; set; }

        [MaxLength(100)]
        public string? SurfaceType { get; set; }

        [MaxLength(200)]
        public string? SpecialFeatures { get; set; }

        [MaxLength(100)]
        public string? BestTimeToJog { get; set; }

        public bool HasParking { get; set; }

        public bool HasWaterFountains { get; set; }

        public bool HasRestrooms { get; set; }

        public bool IsWellLit { get; set; }

        [MaxLength(300)]
        public string? SafetyNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}