using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.MVVM.Models;
using MessManagement.MVVM.Views;
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
    public partial class MessListViewModel: ObservableObject
    {
        private readonly MessService _messService;
        public ObservableCollection<MessModel> Messes { get; set; }= new ObservableCollection<MessModel>();
        [ObservableProperty]
        private bool isListEmpty;
        [ObservableProperty]
        private bool isBusy;
        public MessListViewModel(MessService messService)
        {
            _messService= messService;
        }
        private void CheckIfEmpty()
        {
            IsListEmpty = Messes.Count == 0;
        }
        [RelayCommand]
        private async Task AddNewMessAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(MessWizardPage)}");
        }
        public async Task LoadMessesAsync()
        {
            try
            {
                IsBusy = true;
                var response = await _messService.GetUserMessesAsync();
                if (response != null && response.Success && response.Data != null)
                {
                    Messes.Clear();
                    foreach (var mess in response.Data)
                    {
                        var messModel =new MessModel(Messes)
                        {
                            MessId= mess.MessId,
                            MessName= mess.MessName,
                            Description= mess.Description,
                            Month= mess.Month,
                            CreatedBy= mess.CreatedBy,
                            CreatedAt= mess.CreatedAt,
                            CurrentMess= mess.CurrentMess,
                            IsCreatedByCurrentUser= mess.IsCreatedByCurrentUser,
                            TotalMarketCost= mess.TotalMarketCost,
                            TotalMeals= mess.TotalMeals,
                            MealRate= mess.MealRate,
                            CommonBillPerMember= mess.CommonBillPerMember                                
                        };
                        messModel.CurrentMessChanged += OnCurrentMessChangedAsync;

                        Messes.Add(messModel);


                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Mess load failed", response.Message, "OK");
                }
                CheckIfEmpty();
            }
            finally
            {
                IsBusy = false;
            }           
        }
        private async Task OnCurrentMessChangedAsync(MessModel mess)
        {
            try
            {
                // Optionally show loading indicator
                //IsBusy = true;
                var messDto = new MessDto()
                {
                    MessId = mess.MessId,
                    MessName = mess.MessName,
                    Description = mess.Description,
                    Month = mess.Month,
                    CreatedBy = mess.CreatedBy,
                    CreatedAt = mess.CreatedAt,
                    CurrentMess = mess.CurrentMess,
                    IsCreatedByCurrentUser = mess.IsCreatedByCurrentUser,
                    TotalMarketCost = mess.TotalMarketCost,
                    TotalMeals = mess.TotalMeals,
                    MealRate = mess.MealRate,
                    CommonBillPerMember = mess.CommonBillPerMember
                };
                var result = await _messService.UpdateCurrentMessAsync(messDto);
                if (result.Success)
                {
                    //foreach (var m in Messes)
                    //{
                    //    m.CurrentMess = m.MessId == mess.MessId;
                    //}
                    var toast = Toast.Make("Current mess is set successfully.", ToastDuration.Short, 12);
                    await toast.Show();
                }
                else
                {
                    mess.CurrentMess = false;
                    var toast = Toast.Make(result.Message ?? "Failed to set current mess.", ToastDuration.Short, 12);
                    await toast.Show();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task ViewMessAsync(MessModel selectedMess)
        {
            if (selectedMess == null) return;
            await Shell.Current.GoToAsync($"///MessDetailsTabBar");
            //await Shell.Current.GoToAsync($"///MessDetailsTabBar/MessMembersPage");
            Preferences.Set("CurrentMessId", selectedMess.MessId);
        }

        [RelayCommand]
        private async Task RemoveMessAsync(MessModel selectedMess)
        {
            if (selectedMess == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{selectedMess.MessName}'?",
                "Yes", "Cancel");

            if (!confirm)
                return;

            try
            {
                // Optionally show loading indicator
                //IsBusy = true;

                var result = await _messService.DeleteMessAsync(selectedMess.MessId);
                if (result.Success)
                {
                    // Remove from local list to update UI
                    Messes.Remove(selectedMess);

                    await Application.Current.MainPage.DisplayAlert(
                        "Deleted",
                        "Mess deleted successfully.",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        result.Message ?? "Failed to delete mess.",
                        "OK");
                }
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            //finally
            //{
            //    IsBusy = false;
            //}
        }
    }
}
