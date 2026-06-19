using Lesson3_CNLTWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson3_CNLTWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RoomType_BIT242520> RoomTypes => Set<RoomType_BIT242520>();
        public DbSet<Room_BIT242520> Rooms => Set<Room_BIT242520>();
        public DbSet<RoomImage_BIT242520> RoomImages => Set<RoomImage_BIT242520>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomType_BIT242520>(entity =>
            {
                entity.ToTable("RoomTypes_BIT242520", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_RoomTypes_BIT242520_Name_NotEmpty",
                    "LEN(LTRIM(RTRIM([Name]))) > 0"));
            });

            modelBuilder.Entity<Room_BIT242520>(entity =>
            {
                entity.ToTable("Rooms_BIT242520", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10,0)");
                entity.Property(e => e.Area).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.HasIndex(e => new { e.RoomTypeId, e.Name }).IsUnique();

                entity.HasOne(e => e.RoomType)
                    .WithMany(e => e.Rooms)
                    .HasForeignKey(e => e.RoomTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Rooms_BIT242520_Name_NotEmpty", "LEN(LTRIM(RTRIM([Name]))) > 0");
                    t.HasCheckConstraint("CK_Rooms_BIT242520_Price_Positive", "[Price] > 0");
                    t.HasCheckConstraint("CK_Rooms_BIT242520_Area_Positive", "[Area] > 0");
                });
            });

            modelBuilder.Entity<RoomImage_BIT242520>(entity =>
            {
                entity.ToTable("RoomImages_BIT242520", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.IsThumbnail).HasDefaultValue(false);

                entity.HasOne(e => e.Room)
                    .WithMany(e => e.RoomImages)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.RoomId)
                    .HasFilter("[IsThumbnail] = 1")
                    .IsUnique();

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_RoomImages_BIT242520_ImageUrl_NotEmpty",
                    "LEN(LTRIM(RTRIM([ImageUrl]))) > 0"));
            });
        }
    }
}
