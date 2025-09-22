using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

[Table("Mess")]
[Index("Month", Name = "UQ__Mess__FF7C6BA029C450D6", IsUnique = true)]
public partial class Mess
{
    [Key]
    public int MessId { get; set; }

    [StringLength(150)]
    public string MessName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Month { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Mess")]
    public virtual ICollection<CommonBill> CommonBills { get; set; } = new List<CommonBill>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("Messes")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Mess")]
    public virtual ICollection<MarketCost> MarketCosts { get; set; } = new List<MarketCost>();

    [InverseProperty("Mess")]
    public virtual ICollection<Meal> Meals { get; set; } = new List<Meal>();

    [InverseProperty("Mess")]
    public virtual ICollection<MessMember> MessMembers { get; set; } = new List<MessMember>();
}
