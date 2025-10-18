using MessApi.Models;
using MessManagement.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Data
{
    public partial class ApplicationDbContext
    {
        public DbSet<MessMemberSummaryDto> MessMemberSummaryResults { get; set; } // keyless entity
        public DbSet<MessDto> MessSummaryResults { get; set; } // keyless entity

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessMemberSummaryDto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Optional, because it's not mapped to a real table/view
            });
            modelBuilder.Entity<MessDto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Optional, because it's not mapped to a real table/view
            });
            //
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            //    // Custom configuration logic here
            //});

            // Add more custom configurations as needed for other entities

            // Seed Roles
        }
    }
}
