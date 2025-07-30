using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class ActivityLike
    {
        public int Id { get; set; }

        [Required]
        public int ActivityPostId { get; set; }
        public ActivityPost ActivityPost { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}