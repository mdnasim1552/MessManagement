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

    public int MessMemberId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    [StringLength(250)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Quantity { get; set; }

    public int? Unit { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Cost { get; set; }

    [ForeignKey("MessId")]
    [InverseProperty("MarketCosts")]
    public virtual Mess Mess { get; set; } = null!;

    [ForeignKey("MessMemberId")]
    [InverseProperty("MarketCosts")]
    public virtual MessMember MessMember { get; set; } = null!;

    [ForeignKey("Unit")]
    [InverseProperty("MarketCosts")]
    public virtual Unit? UnitNavigation { get; set; }
}
