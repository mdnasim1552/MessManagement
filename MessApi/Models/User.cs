using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

[Index("Email", Name = "UQ__Users__A9D10534B23CDD0E", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [StringLength(255)]
    public string? GoogleId { get; set; }

    [StringLength(500)]
    public string? ProfilePictureUrl { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<MarketCost> MarketCosts { get; set; } = new List<MarketCost>();

    [InverseProperty("User")]
    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Mess> Messes { get; set; } = new List<Mess>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
