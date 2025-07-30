using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class ActivityPost
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public string? ImagePath { get; set; }
        public string? VideoPath { get; set; }

        [Required]
        public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;

        // Activity related fields
        public double? Distance { get; set; } // in km
        public TimeSpan? Duration { get; set; }
        public string? Route { get; set; }
        public DateTime? ActivityDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ActivityComment> Comments { get; set; } = new List<ActivityComment>();
        public ICollection<ActivityLike> Likes { get; set; } = new List<ActivityLike>();
    }

    public enum PostPrivacy
    {
        Public = 0,
        Private = 1,
        FriendsOnly = 2
    }
}