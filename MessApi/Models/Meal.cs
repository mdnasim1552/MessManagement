using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

[Index("MessId", "UserId", "MealDate", Name = "UQ__Meals__8A69AE4C9E41CEAE", IsUnique = true)]
public partial class Meal
{
    [Key]
    public int MealId { get; set; }

    public int MessId { get; set; }

    public int UserId { get; set; }

    public DateOnly MealDate { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Breakfast { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Lunch { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal Dinner { get; set; }

    [ForeignKey("MessId")]
    [InverseProperty("Meals")]
    public virtual Mess Mess { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Meals")]
    public virtual User User { get; set; } = null!;
}
