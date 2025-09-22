using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

public partial class MarketCost
{
    [Key]
    public int CostId { get; set; }

    public int MessId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    public DateOnly ExpenseDate { get; set; }

    [StringLength(250)]
    public string? Description { get; set; }

    [ForeignKey("MessId")]
    [InverseProperty("MarketCosts")]
    public virtual Mess Mess { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("MarketCosts")]
    public virtual User User { get; set; } = null!;
}
