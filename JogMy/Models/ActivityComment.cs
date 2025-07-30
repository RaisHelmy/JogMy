using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class ActivityComment
    {
        public int Id { get; set; }

        [Required]
        public int ActivityPostId { get; set; }
        public ActivityPost ActivityPost { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // For nested replies (optional)
        public int? ParentCommentId { get; set; }
        public ActivityComment? ParentComment { get; set; }
        public ICollection<ActivityComment> Replies { get; set; } = new List<ActivityComment>();
    }
}