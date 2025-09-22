using MessApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Data.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db)
        {
            // Ensure database is created
            await db.Database.EnsureCreatedAsync();

            // 1️⃣ Seed Roles
            if (!await db.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "Manager" },
                    new Role { Name = "Editor" }
                };

                db.Roles.AddRange(roles);
                await db.SaveChangesAsync();
            }

            // 2️⃣ Seed Users
            if (!await db.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User
                    {
                        FullName = "Super Admin",
                        Email = "admin@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Manager User",
                        Email = "manager@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Editor User",
                        Email = "editor@example.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Editor@123"),
                        CreatedAt = DateTime.UtcNow
                    }
                };

                db.Users.AddRange(users);
                await db.SaveChangesAsync();
            }

            // 3️⃣ Seed UserRoles
            if (!await db.UserRoles.AnyAsync())
            {
                var admin = await db.Users.FirstAsync(u => u.Email == "admin@example.com");
                var manager = await db.Users.FirstAsync(u => u.Email == "manager@example.com");
                var editor = await db.Users.FirstAsync(u => u.Email == "editor@example.com");

                var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");
                var managerRole = await db.Roles.FirstAsync(r => r.Name == "Manager");
                var editorRole = await db.Roles.FirstAsync(r => r.Name == "Editor");

                var userRoles = new List<UserRole>
                {
                    new UserRole { UserId = admin.Id, RoleId = adminRole.Id, AssignedDate = DateTime.UtcNow },
                    new UserRole { UserId = manager.Id, RoleId = managerRole.Id, AssignedDate = DateTime.UtcNow },
                    new UserRole { UserId = editor.Id, RoleId = editorRole.Id, AssignedDate = DateTime.UtcNow }
                };

                db.UserRoles.AddRange(userRoles);
                await db.SaveChangesAsync();
            }
        }
    }
}
