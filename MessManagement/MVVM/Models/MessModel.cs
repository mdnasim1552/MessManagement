using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.Models
{
    public partial class MessModel: ObservableObject
    {
        private readonly ObservableCollection<MessModel> _parentCollection;

        public MessModel(ObservableCollection<MessModel> parentCollection)
        {
            _parentCollection = parentCollection;
        }
        public int MessId { get; set; }
        public string MessName { get; set; }
        public string? Description { get; set; }
        public DateTime Month { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        [ObservableProperty]
        private bool currentMess;
        public bool IsCreatedByCurrentUser { get; set; }
        public decimal TotalMarketCost { get; set; }
        public decimal TotalMeals { get; set; }
        public decimal MealRate { get; set; }
        public decimal CommonBillPerMember { get; set; }
        public event Func<MessModel, Task>? CurrentMessChanged;

        partial void OnCurrentMessChanged(bool value)
        {
            if (value)
            {
                // Uncheck all other messes
                foreach (var m in _parentCollection)
                {
                    if (m != this)
                        m.CurrentMess = false;
                }

                // Trigger event to notify VM to update backend
                CurrentMessChanged?.Invoke(this);
            }
        }
    }
}
