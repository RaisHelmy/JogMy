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
            var tracks = await _context.JoggingTracks.ToListAsync();
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
    }
}