using JogMy.Data;
using JogMy.Models;
using JogMy.Features.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JogMy.Features.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardViewModel
            {
                TotalTracks = await _context.JoggingTracks.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                RecentTracks = await _context.JoggingTracks
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Tracks()
        {
            var tracks = await _context.JoggingTracks.ToListAsync();
            return View(tracks);
        }

        [HttpGet]
        public IActionResult CreateTrack()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrack(CreateTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var track = new JoggingTrack
                {
                    Name = model.Name,
                    Coordinates = model.Coordinates,
                    Distance = model.Distance,
                    Region = model.Region,
                    Description = model.Description
                };

                _context.JoggingTracks.Add(track);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Jogging track created successfully!";
                return RedirectToAction("Tracks");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditTrack(int id)
        {
            var track = await _context.JoggingTracks.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }

            var model = new EditTrackViewModel
            {
                Id = track.Id,
                Name = track.Name,
                Coordinates = track.Coordinates,
                Distance = track.Distance,
                Region = track.Region,
                Description = track.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrack(EditTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var track = await _context.JoggingTracks.FindAsync(model.Id);
                if (track == null)
                {
                    return NotFound();
                }

                track.Name = model.Name;
                track.Coordinates = model.Coordinates;
                track.Distance = model.Distance;
                track.Region = model.Region;
                track.Description = model.Description;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Jogging track updated successfully!";
                return RedirectToAction("Tracks");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            var track = await _context.JoggingTracks.FindAsync(id);
            if (track != null)
            {
                _context.JoggingTracks.Remove(track);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Jogging track deleted successfully!";
            }

            return RedirectToAction("Tracks");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            return View(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);

                TempData["Success"] = $"User role updated to {role} successfully!";
            }

            return RedirectToAction("Users");
        }
    }
}