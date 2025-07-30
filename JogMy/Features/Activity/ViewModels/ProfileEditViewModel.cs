using System.ComponentModel.DataAnnotations;

namespace JogMy.Features.Activity.ViewModels
{
    public class ProfileEditViewModel
    {
        [Display(Name = "Full Name")]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Display(Name = "Bio")]
        [MaxLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Location")]
        [MaxLength(100)]
        public string? Location { get; set; }

        [Display(Name = "Website")]
        [MaxLength(200)]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? Website { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Favorite Running Time")]
        [MaxLength(50)]
        public string? FavoriteRunningTime { get; set; }

        [Display(Name = "Weekly Distance Goal (km)")]
        [Range(0, 1000, ErrorMessage = "Weekly distance must be between 0 and 1000 km")]
        public double? WeeklyDistance { get; set; }

        [Display(Name = "Favorite Route")]
        [MaxLength(100)]
        public string? FavoriteRoute { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }

        [Display(Name = "Cover Photo")]
        public IFormFile? CoverPhoto { get; set; }

        // Read-only display properties
        public string? CurrentProfilePicturePath { get; set; }
        public string? CurrentCoverPhotoPath { get; set; }
        public string? Email { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}