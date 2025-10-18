using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

[Table("Unit")]
public partial class Unit
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(10)]
    public string? ShortName { get; set; }

    [InverseProperty("UnitNavigation")]
    public virtual ICollection<MarketCost> MarketCosts { get; set; } = new List<MarketCost>();
}
