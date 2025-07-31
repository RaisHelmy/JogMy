using JogMy.Data;
using JogMy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JogMy.Features.JoggingTracks
{
    public class JoggingTracksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JoggingTracksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tracks = await _context.JoggingTracks
                .Include(t => t.MediaFiles.OrderBy(m => m.OrderIndex))
                .ToListAsync();
            return View(tracks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var track = await _context.JoggingTracks.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            return View(track);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaypointTooltip(int trackId, int waypointIndex, double latitude, double longitude)
        {
            var track = await _context.JoggingTracks.FindAsync(trackId);
            if (track == null)
            {
                return NotFound();
            }

            ViewBag.WaypointNumber = waypointIndex + 1;
            ViewBag.TrackName = track.Name;
            ViewBag.Latitude = latitude.ToString("F6");
            ViewBag.Longitude = longitude.ToString("F6");

            return PartialView("_WaypointTooltip");
        }
    }
}