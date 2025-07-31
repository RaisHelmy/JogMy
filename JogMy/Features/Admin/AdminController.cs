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

        public IActionResult Terminal()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteCommand([FromBody] TerminalCommandRequest request)
        {
            try
            {
                // Check if command contains multiple commands separated by semicolons
                if (request.Command.Contains(';'))
                {
                    var commands = SplitMultipleCommands(request.Command);
                    var results = new List<string>();
                    
                    foreach (var cmd in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(cmd))
                        {
                            var result = await ProcessCommand(cmd.Trim());
                            results.Add($"$ {cmd.Trim()}\n{result}");
                        }
                    }
                    
                    return Json(new { success = true, output = string.Join("\n\n", results) });
                }
                else
                {
                    var response = await ProcessCommand(request.Command);
                    return Json(new { success = true, output = response });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, output = $"Error: {ex.Message}" });
            }
        }

        private List<string> SplitMultipleCommands(string commandLine)
        {
            var commands = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;
            bool escapeNext = false;

            for (int i = 0; i < commandLine.Length; i++)
            {
                char c = commandLine[i];

                if (escapeNext)
                {
                    current.Append(c);
                    escapeNext = false;
                }
                else if (c == '\\')
                {
                    current.Append(c);
                    escapeNext = true;
                }
                else if (c == '"')
                {
                    current.Append(c);
                    inQuotes = !inQuotes;
                }
                else if (c == ';' && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        commands.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                commands.Add(current.ToString());
            }

            return commands;
        }

        private async Task<string> ProcessCommand(string command)
        {
            var parts = ParseCommandArguments(command.Trim());
            if (parts.Length == 0)
            {
                return "No command entered.";
            }

            var mainCommand = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Command: {mainCommand}, Args: [{string.Join(", ", args)}]");

            return mainCommand switch
            {
                "help" => GetHelpText(),
                "adduser" => await AddUserCommand(args),
                "addtrack" => await AddTrackCommand(args),
                "addtracks" => await AddMultipleTracksCommand(args),
                "addsampletracks" => await AddSampleTracksCommand(),
                "listusers" => await ListUsersCommand(),
                "listtracks" => await ListTracksCommand(),
                "clear" => "CLEAR", // Special command to clear terminal
                _ => $"Unknown command: {mainCommand}. Type 'help' for available commands."
            };
        }

        private string[] ParseCommandArguments(string command)
        {
            var args = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;
            bool escapeNext = false;

            for (int i = 0; i < command.Length; i++)
            {
                char c = command[i];

                if (escapeNext)
                {
                    current.Append(c);
                    escapeNext = false;
                }
                else if (c == '\\')
                {
                    escapeNext = true;
                }
                else if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                args.Add(current.ToString());
            }

            return args.ToArray();
        }

        private string GetHelpText()
        {
            return @"Available Commands:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

User Management:
  adduser <email> <password> <fullname> [role]
    - Create a new user account
    - role: optional (default: User, options: Admin, User)
    - Example: adduser john@example.com password123 ""John Doe"" Admin

  listusers
    - Display all registered users

Track Management:
  addtrack <name> <region> <distance> <coordinates>
    - Create a new jogging track
    - coordinates: JSON array format [[lat,lng],[lat,lng],...]
    - Example: addtrack ""Central Park"" ""KL"" 5.2 ""[[3.1319,101.6841],[3.1320,101.6842]]""

  addtracks <count>
    - Generate multiple track commands template
    - Example: addtracks 3 (generates 3 track commands)

  addsampletracks
    - Add 5 predefined sample tracks to database
    - Example: addsampletracks

  listtracks
    - Display all available tracks

General:
  help      - Show this help message
  clear     - Clear terminal screen

Multiple Commands:
  Use semicolons (;) to run multiple commands in one line:
  
  Multiple Track Creation Examples:
  addtrack ""Taman Tasik Perdana"" ""KL"" 3.5 ""[[3.1390,101.6869],[3.1391,101.6880]]""; addtrack ""KLCC Park"" ""KL"" 2.8 ""[[3.1579,101.7123],[3.1580,101.7124]]""
  
  Multiple User Creation Examples:
  adduser user1@example.com pass123 ""User One"" User; adduser admin@example.com admin123 ""Admin User"" Admin
  
  Mixed Commands Examples:
  adduser test@example.com test123 ""Test User"" User; addtrack ""Test Track"" ""KL"" 2.0 ""[[3.15,101.68],[3.16,101.69]]""; listtracks

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";
        }

        private async Task<string> AddUserCommand(string[] args)
        {
            if (args.Length < 3)
            {
                return "Usage: adduser <email> <password> <fullname> [role]\nExample: adduser john@example.com password123 \"John Doe\" Admin";
            }

            var email = args[0];
            var password = args[1];
            var fullName = args[2];
            var role = args.Length > 3 ? args[3] : "User";

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return $"❌ User with email '{email}' already exists.";
            }

            // Create new user
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return $"❌ Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            // Assign role
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || role.Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return $"✅ User '{fullName}' ({email}) created successfully with role '{role}'.";
        }

        private async Task<string> AddTrackCommand(string[] args)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"AddTrackCommand called with {args.Length} args: [{string.Join(", ", args)}]");
                
                if (args.Length < 4)
                {
                    return "Usage: addtrack <name> <region> <distance> <coordinates>\nExample: addtrack \"Central Park\" \"KL\" 5.2 \"[[3.1319,101.6841],[3.1320,101.6842]]\"";
                }

                var name = args[0];
                var region = args[1];
                
                System.Diagnostics.Debug.WriteLine($"Parsing distance: '{args[2]}'");
                if (!double.TryParse(args[2], out double distance))
                {
                    return $"❌ Invalid distance: '{args[2]}'. Please provide a valid number.";
                }

                var coordinatesJson = args[3];
                System.Diagnostics.Debug.WriteLine($"Coordinates JSON: '{coordinatesJson}'");

                // Validate JSON format
                System.Text.Json.JsonDocument.Parse(coordinatesJson);

                var track = new JoggingTrack
                {
                    Name = name,
                    Region = region,
                    Distance = distance,
                    Coordinates = coordinatesJson,
                    Description = $"Track created via terminal on {DateTime.Now:yyyy-MM-dd HH:mm}"
                };

                System.Diagnostics.Debug.WriteLine($"Adding track: {name} in {region}");
                _context.JoggingTracks.Add(track);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"Track saved successfully: {track.Id}");

                return $"✅ Track '{name}' in {region} ({distance} km) created successfully.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddTrackCommand error: {ex}");
                return $"❌ Failed to create track: {ex.Message}";
            }
        }

        private Task<string> AddMultipleTracksCommand(string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int count) || count <= 0 || count > 10)
            {
                return Task.FromResult("Usage: addtracks <count>\nExample: addtracks 3\nNote: Maximum 10 tracks at once.");
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine($"🚀 Batch Track Creation Mode - Adding {count} tracks");
            output.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            output.AppendLine();
            output.AppendLine("📝 Copy and paste this multi-command line:");
            output.AppendLine();

            var commands = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                commands.Add($"addtrack \"Track {i}\" \"KL\" {2.0 + i * 0.5} \"[[3.{1300 + i},{101.6800 + i * 0.001}],[3.{1305 + i},{101.6805 + i * 0.001}]]\"");
            }

            output.AppendLine(string.Join("; ", commands));
            output.AppendLine();
            output.AppendLine("🎯 Or try the sample tracks command: addsampletracks");

            return Task.FromResult(output.ToString());
        }

        private async Task<string> AddSampleTracksCommand()
        {
            var sampleTracks = new List<(string Name, string Region, double Distance, string Coordinates)>
            {
                ("KLCC Park Circuit", "KL", 2.8, "[[3.1579,101.7123],[3.1580,101.7124],[3.1585,101.7127],[3.1590,101.7130]]"),
                ("Lake Gardens Main Loop", "KL", 4.2, "[[3.1390,101.6869],[3.1391,101.6880],[3.1395,101.6885],[3.1400,101.6890]]"),
                ("Titiwangsa Park Trail", "KL", 3.6, "[[3.1710,101.7030],[3.1715,101.7035],[3.1720,101.7040],[3.1725,101.7045]]"),
                ("Bukit Jalil Park Loop", "KL", 5.1, "[[3.0470,101.6800],[3.0475,101.6805],[3.0480,101.6810],[3.0485,101.6815]]"),
                ("Shah Alam Lake Gardens", "Selangor", 3.9, "[[3.0667,101.5000],[3.0672,101.5005],[3.0677,101.5010],[3.0682,101.5015]]")
            };

            var results = new List<string>();
            int successCount = 0;

            foreach (var (name, region, distance, coordinates) in sampleTracks)
            {
                try
                {
                    // Check if track already exists
                    var existingTrack = await _context.JoggingTracks.FirstOrDefaultAsync(t => t.Name == name);
                    if (existingTrack != null)
                    {
                        results.Add($"⚠️  Track '{name}' already exists, skipping.");
                        continue;
                    }

                    // Validate JSON format
                    System.Text.Json.JsonDocument.Parse(coordinates);

                    var track = new JoggingTrack
                    {
                        Name = name,
                        Region = region,
                        Distance = distance,
                        Coordinates = coordinates,
                        Description = $"Sample track created via terminal on {DateTime.Now:yyyy-MM-dd HH:mm}"
                    };

                    _context.JoggingTracks.Add(track);
                    await _context.SaveChangesAsync();
                    
                    results.Add($"✅ Track '{name}' in {region} ({distance} km) created successfully.");
                    successCount++;
                }
                catch (Exception ex)
                {
                    results.Add($"❌ Failed to create track '{name}': {ex.Message}");
                }
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine("🌟 Sample Tracks Creation Results:");
            output.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            output.AppendLine();
            foreach (var result in results)
            {
                output.AppendLine(result);
            }
            output.AppendLine();
            output.AppendLine($"📊 Summary: {successCount} out of {sampleTracks.Count} tracks created successfully.");

            return output.ToString();
        }

        private async Task<string> ListUsersCommand()
        {
            var users = await _userManager.Users.ToListAsync();
            if (!users.Any())
            {
                return "No users found.";
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine("📋 Registered Users:");
            output.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleText = roles.Any() ? string.Join(", ", roles) : "No Role";
                output.AppendLine($"👤 {user.FullName ?? "N/A"} ({user.Email}) - Role: {roleText}");
            }

            return output.ToString();
        }

        private async Task<string> ListTracksCommand()
        {
            var tracks = await _context.JoggingTracks.ToListAsync();
            if (!tracks.Any())
            {
                return "No tracks found.";
            }

            var output = new System.Text.StringBuilder();
            output.AppendLine("🗺️  Available Tracks:");
            output.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            foreach (var track in tracks)
            {
                output.AppendLine($"📍 {track.Name} - {track.Region} ({track.Distance} km)");
                if (!string.IsNullOrEmpty(track.Description))
                {
                    output.AppendLine($"   Description: {track.Description}");
                }
            }

            return output.ToString();
        }

        public class TerminalCommandRequest
        {
            public string Command { get; set; } = string.Empty;
        }
    }
}