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
        
        // Quick stats for dashboard
        public double TotalDistance => Posts.Sum(p => p.Distance ?? 0);
        public int TotalLikes => Posts.Sum(p => p.LikesCount);
        public int TotalComments => Posts.Sum(p => p.Comments.Count);
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
}