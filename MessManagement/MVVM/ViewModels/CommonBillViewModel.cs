using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.MVVM.Models;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.ViewModels
{
    public partial class CommonBillViewModel: ObservableObject
    {
        private readonly MessService _messService;
        public ObservableCollection<CommonBillModel> CommonBills { get; set; } = new ObservableCollection<CommonBillModel>();
        [ObservableProperty]
        private bool isListEmpty;
        [ObservableProperty]
        private bool isBusy;
        public CommonBillViewModel(MessService messService)
        {
            _messService = messService;
        }
        private void CheckIfEmpty()
        {
            IsListEmpty = CommonBills.Count == 0;
        }
        [RelayCommand]
        private async Task LoadCommonBillAsync(int messId)
        {
            try
            {
                IsBusy = true;
                var commonbills = await _messService.GetCommonBillAsync(messId);
                if (commonbills.Success && commonbills.Data?.Any() == true)
                {
                    CommonBills.Clear();
                    foreach (var c in commonbills.Data)
                    {
                        CommonBills.Add(new CommonBillModel()
                        {
                            BillId = c.BillId,
                            MessId = c.MessId,
                            BillType = c.BillType,
                            Amount = c.Amount
                        });
                    }
                }
                else
                {
                    CommonBills.Clear();
                }
                UpdateIndexes();
                CheckIfEmpty();
            }
            finally
            {
                IsBusy = false;
            }          
        }
        [RelayCommand]
        private async Task AddCommonbillAsync(CommonBillModel currentBill)
        {
            try
            {
                // You can insert after the clicked item
                if (currentBill != null)
                {
                    var index = CommonBills.IndexOf(currentBill);
                    if (index >= 0 && index < CommonBills.Count)
                    {
                        CommonBills.Insert(index + 1, new CommonBillModel
                        {
                            BillType = string.Empty,
                            Amount = 0.00M
                        });
                    }
                    else
                    {
                        CommonBills.Add(new CommonBillModel());
                    }
                }
                else
                {
                    // Add to end if no reference bill
                    CommonBills.Add(new CommonBillModel());
                }
                UpdateIndexes();
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding bill: {ex.Message}");
            }
        }
        [RelayCommand]
        private async Task RemoveCommonbillAsync(CommonBillModel currentBill)
        {
            try
            {
                if (currentBill == null)
                    return;

                // Confirm it's in the collection before removing
                if (CommonBills.Contains(currentBill))
                {
                    CommonBills.Remove(currentBill);
                    if (currentBill.BillId > 0) // only remove from DB if already saved
                        await _messService.DeleteCommonBillAsync(currentBill.BillId);
                }
                UpdateIndexes();
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing bill: {ex.Message}");
            }
        }
        private void UpdateIndexes()
        {
            for (int i = 0; i < CommonBills.Count; i++)
            {
                CommonBills[i].Index = i + 1; // 1-based serial
            }
        }

        public async Task SaveBillAsync(CommonBillModel bill)
        {
            if (bill == null || string.IsNullOrWhiteSpace(bill.BillType) || bill.Amount <= 0)
            {
                return;
            }
            var dto = new CommonBillDto
            {
                BillId = bill.BillId,
                MessId = bill.MessId,
                BillType = bill.BillType,
                Amount = bill.Amount ?? 0
            };

            var result = await _messService.UpdateAndSaveCommonBillAsync(dto);
            if (!result.Success)
            {
                // Optional: show error toast
                Console.WriteLine("Failed to save bill in database");
            }
            else
            {
                bill.BillId = result.Data.BillId;
                //var index = CommonBills.IndexOf(bill);
                //if (index >= 0)
                //{
                //    // Replace the old object with the updated one
                //    CommonBills[index] = bill;
                //}
            }
        }

    }
}
