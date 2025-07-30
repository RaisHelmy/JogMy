using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JogMy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        
        // Profile fields
        [MaxLength(500)]
        public string? Bio { get; set; }
        
        [MaxLength(100)]
        public string? Location { get; set; }
        
        public string? ProfilePicturePath { get; set; }
        
        public string? CoverPhotoPath { get; set; }
        
        [MaxLength(200)]
        public string? Website { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        // Jogging-specific fields
        [MaxLength(50)]
        public string? FavoriteRunningTime { get; set; } // Morning, Evening, etc.
        
        public double? WeeklyDistance { get; set; } // Target weekly distance in km
        
        [MaxLength(100)]
        public string? FavoriteRoute { get; set; }
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}