using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MessApi.Models;

public partial class CommonBill
{
    [Key]
    public int BillId { get; set; }

    public int MessId { get; set; }

    [StringLength(100)]
    public string BillType { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    public DateOnly BillDate { get; set; }

    [ForeignKey("MessId")]
    [InverseProperty("CommonBills")]
    public virtual Mess Mess { get; set; } = null!;
}
