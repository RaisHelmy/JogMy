using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JogMy.Data;
using JogMy.Models;
using JogMy.Features.Activity.ViewModels;
using System.Security.Claims;

namespace JogMy.Features.Activity
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActivityController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> MyActivity()
        {
            var viewModel = await GetMyActivityViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyActivity(CreatePostViewModel createPost, bool isPostSubmission = false)
        {
            // If it's not a post submission, just return the regular view
            if (!isPostSubmission)
            {
                var regularViewModel = await GetMyActivityViewModel();
                return View(regularViewModel);
            }

            // Debug: Log the model content
            System.Diagnostics.Debug.WriteLine($"Content received: '{createPost.Content}'");
            System.Diagnostics.Debug.WriteLine($"Privacy: {createPost.Privacy}");
            System.Diagnostics.Debug.WriteLine($"Distance: {createPost.Distance}");
            
            if (!ModelState.IsValid)
            {
                // Get specific validation errors for debugging
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                
                var errorDetails = string.Join("; ", errors.Select(x => $"{x.Field}: {string.Join(", ", x.Errors)}"));
                System.Diagnostics.Debug.WriteLine($"Validation errors: {errorDetails}");
                
                TempData["Error"] = $"Validation failed: {errorDetails}";
                
                // Return to the same page with the model to preserve form data
                var viewModel = await GetMyActivityViewModel();
                viewModel.CreatePost = createPost;
                return View(viewModel);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string? imagePath = null;
            string? videoPath = null;

            // Custom validation for optional distance
            if (createPost.Distance.HasValue && createPost.Distance.Value <= 0)
            {
                createPost.Distance = null; // Reset invalid distance to null
            }

            // Handle file uploads
            if (createPost.Image != null)
            {
                imagePath = await SaveFileAsync(createPost.Image, "images");
            }

            if (createPost.Video != null)
            {
                videoPath = await SaveFileAsync(createPost.Video, "videos");
            }

            // Parse duration
            TimeSpan? duration = null;
            if (!string.IsNullOrEmpty(createPost.DurationInput) && TimeSpan.TryParse(createPost.DurationInput, out var parsedDuration))
            {
                duration = parsedDuration;
            }

            // Get route name and distance from selected track if provided
            string? routeName = createPost.Route;
            double? trackDistance = createPost.Distance;
            
            if (createPost.SelectedTrackId.HasValue)
            {
                var selectedTrack = await _context.JoggingTracks
                    .FirstOrDefaultAsync(t => t.Id == createPost.SelectedTrackId.Value);
                if (selectedTrack != null)
                {
                    routeName = selectedTrack.Name;
                    trackDistance = selectedTrack.Distance;
                }
            }

            var post = new ActivityPost
            {
                UserId = userId,
                Content = createPost.Content,
                ImagePath = imagePath,
                VideoPath = videoPath,
                Privacy = createPost.Privacy,
                Distance = trackDistance,
                Duration = duration,
                Route = routeName,
                ActivityDate = createPost.ActivityDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityPosts.Add(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your activity has been posted successfully!";
            TempData["NewPostId"] = post.Id; // Pass the new post ID to highlight it
            return RedirectToAction(nameof(MyActivity));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Comment cannot be empty." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);

            var comment = new ActivityComment
            {
                ActivityPostId = postId,
                UserId = userId,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityComments.Add(comment);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                comment = new {
                    id = comment.Id,
                    userName = user?.UserName ?? user?.Email ?? "Unknown User",
                    content = comment.Content,
                    timeAgo = "Just now"
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var existingLike = await _context.ActivityLikes
                .FirstOrDefaultAsync(l => l.ActivityPostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.ActivityLikes.Remove(existingLike);
            }
            else
            {
                var like = new ActivityLike
                {
                    ActivityPostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ActivityLikes.Add(like);
            }

            await _context.SaveChangesAsync();

            var likesCount = await _context.ActivityLikes
                .CountAsync(l => l.ActivityPostId == postId);

            return Json(new { 
                success = true, 
                isLiked = existingLike == null,
                likesCount = likesCount
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileEditViewModel
            {
                FullName = user.FullName,
                Bio = user.Bio,
                Location = user.Location,
                Website = user.Website,
                DateOfBirth = user.DateOfBirth,
                FavoriteRunningTime = user.FavoriteRunningTime,
                WeeklyDistance = user.WeeklyDistance,
                FavoriteRoute = user.FavoriteRoute,
                CurrentProfilePicturePath = user.ProfilePicturePath,
                CurrentCoverPhotoPath = user.CoverPhotoPath,
                Email = user.Email,
                JoinedAt = user.JoinedAt
            };

            return PartialView("_EditProfileModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditProfileModal", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.Location = model.Location;
            user.Website = model.Website;
            user.DateOfBirth = model.DateOfBirth;
            user.FavoriteRunningTime = model.FavoriteRunningTime;
            user.WeeklyDistance = model.WeeklyDistance;
            user.FavoriteRoute = model.FavoriteRoute;

            // Handle profile picture upload
            if (model.ProfilePicture != null)
            {
                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    DeleteFile(user.ProfilePicturePath);
                }

                // Save new profile picture
                user.ProfilePicturePath = await SaveFileAsync(model.ProfilePicture, "profiles");
            }

            // Handle cover photo upload
            if (model.CoverPhoto != null)
            {
                // Delete old cover photo if exists
                if (!string.IsNullOrEmpty(user.CoverPhotoPath))
                {
                    DeleteFile(user.CoverPhotoPath);
                }

                // Save new cover photo
                user.CoverPhotoPath = await SaveFileAsync(model.CoverPhoto, "covers");
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return Json(new { success = true });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return PartialView("_EditProfileModal", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var post = await _context.ActivityPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return Json(new { success = false, message = "Post not found or access denied." });
            }

            // Delete associated files
            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                DeleteFile(post.ImagePath);
            }
            if (!string.IsNullOrEmpty(post.VideoPath))
            {
                DeleteFile(post.VideoPath);
            }

            _context.ActivityPosts.Remove(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
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

        private async Task<MyActivityViewModel> GetMyActivityViewModel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userId);

            var totalPosts = await _context.ActivityPosts
                .Where(p => p.UserId == userId)
                .CountAsync();

            // Get available tracks for dropdown
            var availableTracks = await _context.JoggingTracks
                .OrderBy(t => t.Name)
                .ToListAsync();

            // Get only the latest 5 posts for the timeline preview
            var posts = await _context.ActivityPosts
                .Include(p => p.User)
                .Include(p => p.Comments.OrderBy(c => c.CreatedAt))
                    .ThenInclude(c => c.User)
                .Include(p => p.Likes)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new MyActivityViewModel
            {
                CurrentUserId = userId,  
                TotalPosts = totalPosts,
                AvailableTracks = availableTracks,
                Profile = new UserProfileViewModel
                {
                    UserId = userId,
                    FullName = user?.FullName,
                    Email = user?.Email,
                    Bio = user?.Bio,
                    Location = user?.Location,
                    ProfilePicturePath = user?.ProfilePicturePath,
                    CoverPhotoPath = user?.CoverPhotoPath,
                    Website = user?.Website,
                    DateOfBirth = user?.DateOfBirth,
                    FavoriteRunningTime = user?.FavoriteRunningTime,
                    WeeklyDistance = user?.WeeklyDistance,
                    FavoriteRoute = user?.FavoriteRoute,
                    JoinedAt = user?.JoinedAt ?? DateTime.UtcNow
                },
                Posts = posts.Select(p => new ActivityPostViewModel
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? p.User.Email ?? "Unknown User",
                    UserEmail = p.User.Email ?? "",
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    VideoPath = p.VideoPath,
                    Privacy = p.Privacy,
                    CreatedAt = p.CreatedAt,
                    Distance = p.Distance,
                    Duration = p.Duration,
                    Route = p.Route,
                    ActivityDate = p.ActivityDate,
                    LikesCount = p.Likes.Count,
                    IsLikedByCurrentUser = p.Likes.Any(l => l.UserId == userId),
                    CanEdit = p.UserId == userId,
                    CanDelete = p.UserId == userId,
                    Comments = p.Comments.Select(c => new ActivityCommentViewModel
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User.UserName ?? c.User.Email ?? "Unknown User",
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        CanEdit = c.UserId == userId,
                        CanDelete = c.UserId == userId
                    }).ToList()
                }).ToList()
            };
        }
    }
}