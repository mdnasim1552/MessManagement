using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public ObservableCollection<MessDto> Messes { get; set; }= new ObservableCollection<MessDto>();
        [ObservableProperty]
        private bool isBusy;
        public MessListViewModel(MessService messService)
        {
            _messService= messService;
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
                        Messes.Add(mess);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Mess load failed", response.Message, "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }           
        }

        [RelayCommand]
        private async Task ViewMessAsync(MessDto selectedMess)
        {
            if (selectedMess == null) return;
            await Shell.Current.GoToAsync($"///MessDetailsTabBar");
            //await Shell.Current.GoToAsync($"///MessDetailsTabBar/MessMembersPage");
            Preferences.Set("CurrentMessId", selectedMess.MessId);
        }

        [RelayCommand]
        private async Task RemoveMessAsync(MessDto selectedMess)
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
