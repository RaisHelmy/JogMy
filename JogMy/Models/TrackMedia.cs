using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class TrackMedia
    {
        public int Id { get; set; }
        
        public int JoggingTrackId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        public MediaType MediaType { get; set; }
        
        public int OrderIndex { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public JoggingTrack JoggingTrack { get; set; } = null!;
    }
}