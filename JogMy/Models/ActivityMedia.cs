using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class ActivityMedia
    {
        public int Id { get; set; }
        
        [Required]
        public int ActivityPostId { get; set; }
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        [Required]
        public MediaType MediaType { get; set; }
        
        public int OrderIndex { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ActivityPost ActivityPost { get; set; } = null!;
    }
    
    public enum MediaType
    {
        Image = 0,
        Video = 1
    }
}