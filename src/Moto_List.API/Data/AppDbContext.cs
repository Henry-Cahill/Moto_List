using Microsoft.EntityFrameworkCore;
using Moto_List.API.Models;

namespace Moto_List.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MotoItem> MotoItems => Set<MotoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100);
            entity.HasData(
                new Category { Id = 1, Name = "Gear", Description = "Riding gear and protective equipment" },
                new Category { Id = 2, Name = "Bike Parts", Description = "Motorcycle parts and accessories" },
                new Category { Id = 3, Name = "Tools", Description = "Tools and maintenance supplies" },
                new Category { Id = 4, Name = "Safety", Description = "Safety equipment and first aid" },
                new Category { Id = 5, Name = "Camping", Description = "Camping and overnight supplies" },
                new Category { Id = 6, Name = "Other", Description = "Miscellaneous items" }
            );
        });

        modelBuilder.Entity<MotoItem>(entity =>
        {
            entity.Property(m => m.Name).HasMaxLength(200);
            entity.HasOne(m => m.Category)
                  .WithMany(c => c.MotoItems)
                  .HasForeignKey(m => m.CategoryId);
            entity.HasOne(m => m.User)
                  .WithMany(u => u.MotoItems)
                  .HasForeignKey(m => m.UserId);
        });
    }
}
