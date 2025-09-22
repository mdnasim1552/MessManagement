using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessManagement.MVVM.ViewModels
{
    public partial class RegisterViewModel: ObservableObject
    {
        private readonly AuthService _authService;
        [ObservableProperty]
        private string fullname;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
        }
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Fullname) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Full Name, Email and Password are required.", "OK");
                return;
            }
            var request = new RegisterUserDto
            {
                FullName= Fullname,
                Email = Email,
                Password = Password
            };
            var result = await _authService.RegisterAsync(request);
            if (result != null && result.Success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", result.Data, "OK");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Register Failed", result.Message, "OK");
            }
        }
    }
}
