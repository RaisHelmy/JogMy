using JogMy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JogMy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<JoggingTrack> JoggingTracks { get; set; }
        public DbSet<ActivityPost> ActivityPosts { get; set; }
        public DbSet<ActivityComment> ActivityComments { get; set; }
        public DbSet<ActivityLike> ActivityLikes { get; set; }
        public DbSet<ActivityMedia> ActivityMedia { get; set; }
        public DbSet<TrackMedia> TrackMedia { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JoggingTrack>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Coordinates).IsRequired();
                entity.Property(e => e.Distance).IsRequired();
                entity.Property(e => e.Region).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
            });

            // ActivityPost configuration
            builder.Entity<ActivityPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Privacy).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ActivityComment configuration
            builder.Entity<ActivityComment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.ActivityPost)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(e => e.ActivityPostId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Self-referencing for replies
                entity.HasOne(e => e.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(e => e.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ActivityLike configuration
            builder.Entity<ActivityLike>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.ActivityPost)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(e => e.ActivityPostId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint - one like per user per post
                entity.HasIndex(e => new { e.ActivityPostId, e.UserId }).IsUnique();
            });

            // ActivityMedia configuration
            builder.Entity<ActivityMedia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.MediaType).IsRequired();
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.ActivityPost)
                    .WithMany(p => p.MediaFiles)
                    .HasForeignKey(e => e.ActivityPostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TrackMedia configuration
            builder.Entity<TrackMedia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.MediaType).IsRequired();
                entity.Property(e => e.OrderIndex).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");
                
                entity.HasOne(e => e.JoggingTrack)
                    .WithMany(t => t.MediaFiles)
                    .HasForeignKey(e => e.JoggingTrackId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}