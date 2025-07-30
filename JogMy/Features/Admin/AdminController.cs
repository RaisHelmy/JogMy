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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
                    Description = model.Description,
                    CustomDifficulty = model.CustomDifficulty,
                    SurfaceType = model.SurfaceType,
                    SpecialFeatures = model.SpecialFeatures,
                    BestTimeToJog = model.BestTimeToJog,
                    HasParking = model.HasParking,
                    HasWaterFountains = model.HasWaterFountains,
                    HasRestrooms = model.HasRestrooms,
                    IsWellLit = model.IsWellLit,
                    SafetyNotes = model.SafetyNotes
                };

                _context.JoggingTracks.Add(track);
                await _context.SaveChangesAsync();

                // Handle media file uploads
                if (model.MediaFiles != null && model.MediaFiles.Any())
                {
                    var mediaFiles = new List<TrackMedia>();
                    for (int i = 0; i < model.MediaFiles.Count; i++)
                    {
                        var file = model.MediaFiles[i];
                        var isVideo = file.ContentType.StartsWith("video/");
                        var folder = isVideo ? "videos" : "images";
                        var filePath = await SaveFileAsync(file, folder);
                        
                        mediaFiles.Add(new TrackMedia
                        {
                            JoggingTrackId = track.Id,
                            FilePath = filePath,
                            MediaType = isVideo ? MediaType.Video : MediaType.Image,
                            OrderIndex = i
                        });
                    }

                    _context.TrackMedia.AddRange(mediaFiles);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Jogging track created successfully!";
                return RedirectToAction("Tracks");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditTrack(int id)
        {
            var track = await _context.JoggingTracks
                .Include(t => t.MediaFiles.OrderBy(m => m.OrderIndex))
                .FirstOrDefaultAsync(t => t.Id == id);
                
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
                Description = track.Description,
                CustomDifficulty = track.CustomDifficulty,
                SurfaceType = track.SurfaceType,
                SpecialFeatures = track.SpecialFeatures,
                BestTimeToJog = track.BestTimeToJog,
                HasParking = track.HasParking,
                HasWaterFountains = track.HasWaterFountains,
                HasRestrooms = track.HasRestrooms,
                IsWellLit = track.IsWellLit,
                SafetyNotes = track.SafetyNotes,
                ExistingMedia = track.MediaFiles.Select(m => new TrackMediaViewModel
                {
                    Id = m.Id,
                    FilePath = m.FilePath,
                    MediaType = m.MediaType,
                    OrderIndex = m.OrderIndex
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrack(EditTrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var track = await _context.JoggingTracks
                    .Include(t => t.MediaFiles)
                    .FirstOrDefaultAsync(t => t.Id == model.Id);
                    
                if (track == null)
                {
                    return NotFound();
                }

                track.Name = model.Name;
                track.Coordinates = model.Coordinates;
                track.Distance = model.Distance;
                track.Region = model.Region;
                track.Description = model.Description;
                track.CustomDifficulty = model.CustomDifficulty;
                track.SurfaceType = model.SurfaceType;
                track.SpecialFeatures = model.SpecialFeatures;
                track.BestTimeToJog = model.BestTimeToJog;
                track.HasParking = model.HasParking;
                track.HasWaterFountains = model.HasWaterFountains;
                track.HasRestrooms = model.HasRestrooms;
                track.IsWellLit = model.IsWellLit;
                track.SafetyNotes = model.SafetyNotes;

                // Handle new media file uploads
                if (model.MediaFiles != null && model.MediaFiles.Any())
                {
                    var currentMaxIndex = track.MediaFiles.Any() ? track.MediaFiles.Max(m => m.OrderIndex) : -1;
                    var mediaFiles = new List<TrackMedia>();
                    
                    for (int i = 0; i < model.MediaFiles.Count; i++)
                    {
                        var file = model.MediaFiles[i];
                        var isVideo = file.ContentType.StartsWith("video/");
                        var folder = isVideo ? "videos" : "images";
                        var filePath = await SaveFileAsync(file, folder);
                        
                        mediaFiles.Add(new TrackMedia
                        {
                            JoggingTrackId = track.Id,
                            FilePath = filePath,
                            MediaType = isVideo ? MediaType.Video : MediaType.Image,
                            OrderIndex = currentMaxIndex + i + 1
                        });
                    }

                    _context.TrackMedia.AddRange(mediaFiles);
                }

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

        [HttpPost]
        public async Task<IActionResult> DeleteTrackMedia(int id)
        {
            var media = await _context.TrackMedia.FindAsync(id);
            if (media != null)
            {
                // Delete the physical file
                DeleteFile(media.FilePath);
                
                // Remove from database
                _context.TrackMedia.Remove(media);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Media not found." });
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName.Replace(" ", "_")}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        private void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}