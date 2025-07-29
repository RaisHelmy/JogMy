using JogMy.Models;

namespace JogMy.Features.Admin.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalTracks { get; set; }
        public int TotalUsers { get; set; }
        public List<JoggingTrack> RecentTracks { get; set; } = new();
    }
}