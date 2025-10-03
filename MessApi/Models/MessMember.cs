using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

[Index("MessId", "Email", Name = "UQ__MessMemb__D6581C8FCF77A519", IsUnique = true)]
public partial class MessMember
{
    [Key]
    public int MessMemberId { get; set; }

    public int MessId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Role { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? JoinedAt { get; set; }

    [InverseProperty("MessMember")]
    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();

    [ForeignKey("MessId")]
    [InverseProperty("MessMembers")]
    public virtual Mess Mess { get; set; } = null!;
}
