using System;
using System.Collections.Generic;
using MessApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CommonBill> CommonBills { get; set; }

    public virtual DbSet<MarketCost> MarketCosts { get; set; }

    public virtual DbSet<Meal> Meals { get; set; }

    public virtual DbSet<Mess> Messes { get; set; }

    public virtual DbSet<MessMember> MessMembers { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommonBill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__CommonBi__11F2FC6ADCB34CF6");

            entity.HasOne(d => d.Mess).WithMany(p => p.CommonBills).HasConstraintName("FK__CommonBil__MessI__6E01572D");
        });

        modelBuilder.Entity<MarketCost>(entity =>
        {
            entity.HasKey(e => e.CostId).HasName("PK__MarketCo__8285233EDABEC44E");

            entity.HasOne(d => d.Mess).WithMany(p => p.MarketCosts).HasConstraintName("FK_MarketCosts_Mess");

            entity.HasOne(d => d.MessMember).WithMany(p => p.MarketCosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MarketCos__MessMemberId__6B24EA82");

            entity.HasOne(d => d.UnitNavigation).WithMany(p => p.MarketCosts).HasConstraintName("FK__MarketCos__UnitId__6A30C649");
        });

        modelBuilder.Entity<Meal>(entity =>
        {
            entity.HasKey(e => e.MealId).HasName("PK__Meals__ACF6A63D4EEB18A2");

            entity.HasOne(d => d.Mess).WithMany(p => p.Meals).HasConstraintName("FK__Meals__MessId__66603565");

            entity.HasOne(d => d.MessMember).WithMany(p => p.Meals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meals__UserId__6754599E");
        });

        modelBuilder.Entity<Mess>(entity =>
        {
            entity.HasKey(e => e.MessId).HasName("PK__Mess__9CC50CDD06C6E9E2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Mess__CreatedAt__5812160E");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Messes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mess__CreatedBy__59063A47");
        });

        modelBuilder.Entity<MessMember>(entity =>
        {
            entity.HasKey(e => e.MessMemberId).HasName("PK__MessMemb__2F8CE6B8620BB8A1");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())", "DF__MessMembe__Joine__5EBF139D");
            entity.Property(e => e.Role).HasDefaultValue("Member", "DF__MessMember__Role__5DCAEF64");

            entity.HasOne(d => d.Mess).WithMany(p => p.MessMembers).HasConstraintName("FK__MessMembe__MessI__5FB337D6");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07B8D56923");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())", "DF__RefreshTo__Creat__3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens).HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07FA976C93");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Unit__3214EC07F0A552C5");

            entity.Property(e => e.ShortName).IsFixedLength();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07148AD525");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Users__CreatedAt__38996AB5");

            entity.HasOne(d => d.CurrentMess).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Users_Mess");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF2760AD4B7A5E2E");

            entity.Property(e => e.AssignedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRoles__RoleI__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRoles__UserI__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
