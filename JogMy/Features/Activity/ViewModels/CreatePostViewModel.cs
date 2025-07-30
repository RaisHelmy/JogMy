using System.ComponentModel.DataAnnotations;
using JogMy.Models;

namespace JogMy.Features.Activity.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        [MaxLength(1000)]
        [Display(Name = "What's on your mind?")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Privacy")]
        public PostPrivacy Privacy { get; set; } = PostPrivacy.Public;

        [Display(Name = "Upload Image")]
        public IFormFile? Image { get; set; }

        [Display(Name = "Upload Video")]
        public IFormFile? Video { get; set; }

        // Activity details (optional)
        [Display(Name = "Distance (km)")]
        public double? Distance { get; set; }

        [Display(Name = "Duration")]
        public string? DurationInput { get; set; } // Format: HH:mm

        [Display(Name = "Track/Route")]
        public int? SelectedTrackId { get; set; }

        [Display(Name = "Route/Location")]
        [MaxLength(200)]
        public string? Route { get; set; }

        [Display(Name = "Activity Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ActivityDate { get; set; }
    }
}