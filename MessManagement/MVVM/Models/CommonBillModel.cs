using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.Models
{
    public partial class CommonBillModel: ObservableObject
    {
        public int BillId { get; set; }
        public int MessId { get; set; }
        public string BillType { get; set; }
        public decimal Amount { get; set; }
        [ObservableProperty]
        public string heading = string.Empty;
    }
}
