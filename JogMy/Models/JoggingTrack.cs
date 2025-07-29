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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}