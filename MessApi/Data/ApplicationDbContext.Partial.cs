using MessApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Data
{
    public partial class ApplicationDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            //
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            //    // Custom configuration logic here
            //});

            // Add more custom configurations as needed for other entities

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "Editor" }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Super Admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    CreatedAt = new DateTime(2025, 9, 19) // <- fixed value
                },
                new User
                {
                    Id = 2,
                    FullName = "Manager User",
                    Email = "manager@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    CreatedAt = new DateTime(2025, 9, 19)
                },
                new User
                {
                    Id = 3,
                    FullName = "Editor User",
                    Email = "editor@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Editor@123"),
                    CreatedAt = new DateTime(2025, 9, 19)
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1, AssignedDate = new DateTime(2025, 9, 19) },
                new UserRole { UserId = 2, RoleId = 2, AssignedDate = new DateTime(2025, 9, 19) },
                new UserRole { UserId = 3, RoleId = 3, AssignedDate = new DateTime(2025, 9, 19) }
            );
        }
    }
}
