using JogMy.Models;

namespace JogMy.Features.Activity.ViewModels
{
    public class MyActivityViewModel
    {
        public CreatePostViewModel CreatePost { get; set; } = new();
        public List<ActivityPostViewModel> Posts { get; set; } = new();
        public List<JoggingTrack> AvailableTracks { get; set; } = new();
        public string CurrentUserId { get; set; } = string.Empty;
        public int TotalPosts { get; set; }
        
        // User profile information
        public UserProfileViewModel Profile { get; set; } = new();
        
        // Quick stats for dashboard
        public double TotalDistance => Posts.Sum(p => p.Distance ?? 0);
        public int TotalLikes => Posts.Sum(p => p.LikesCount);
        public int TotalComments => Posts.Sum(p => p.Comments.Count);
    }

    public class PublicActivityViewModel
    {
        public List<ActivityPostViewModel> Posts { get; set; } = new();
        public string CurrentUserId { get; set; } = string.Empty;
    }

    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? CoverPhotoPath { get; set; }
        public string? Website { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FavoriteRunningTime { get; set; }
        public double? WeeklyDistance { get; set; }
        public string? FavoriteRoute { get; set; }
        public DateTime JoinedAt { get; set; }
        
        // Calculated properties
        public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Email?.Split('@')[0] ?? "User";
        public string ProfileInitials => DisplayName.Split(' ').Take(2).Select(n => n[0]).DefaultIfEmpty('U').Aggregate("", (a, b) => a + b).ToUpper();
        public string FormattedJoinDate => JoinedAt.ToString("MMMM yyyy");
        public int Age => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : 0;
    }

    public class ActivityPostViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? VideoPath { get; set; }
        public PostPrivacy Privacy { get; set; }
        public List<ActivityMediaViewModel> MediaFiles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        
        // Activity details
        public double? Distance { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? Route { get; set; }
        public DateTime? ActivityDate { get; set; }
        
        // Interaction data
        public List<ActivityCommentViewModel> Comments { get; set; } = new();
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        
        // Formatted properties
        public string TimeAgo => GetTimeAgo(CreatedAt);
        public string FormattedDistance => Distance?.ToString("F1") + " km" ?? "";
        public string FormattedDuration => Duration?.ToString(@"hh\:mm") ?? "";
        
        private string GetTimeAgo(DateTime createdAt)
        {
            var timeSpan = DateTime.UtcNow - createdAt;
            
            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}d ago";
            
            return createdAt.ToString("MMM dd, yyyy");
        }
    }

    public class ActivityCommentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes}m ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours}h ago";
                return CreatedAt.ToString("MMM dd");
            }
        }
    }

    public class ActivityMediaViewModel
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public int OrderIndex { get; set; }
    }
}