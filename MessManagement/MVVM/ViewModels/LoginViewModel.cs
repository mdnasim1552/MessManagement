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
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;

        //public ICommand LoginCommand { get; }
        //public ICommand GoogleLoginCommand { get; }
        //public ICommand RegisterCommand { get; }

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            //LoginCommand = new Command(async () => await LoginAsync());
            //GoogleLoginCommand = new Command(async () => await GoogleLoginAsync());
            //RegisterCommand = new Command(async () => await RegisterAsync());
        }
        [RelayCommand]
        private async Task RegisterAsync()
        {
            try
            {
                // Navigate to RegisterPage using Shell routing
                //var registerPage = App.Current.Handler.MauiContext.Services.GetService<RegisterPage>();
                //await Application.Current.MainPage.Navigation.PushAsync(registerPage);

                await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Validation Error", "Email and Password are required.", "OK");
                    return;
                }
                var request = new LoginRequestDto
                {
                    Email = Email,
                    Password = Password
                };

                var result = await _authService.LoginAsync(request);
                if (result == null || result.Data.Token == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                    return;
                }

                var userdto = result.Data.User;
                // Save JWT & Refresh token securely
                await SecureStorage.SetAsync("auth_token", result.Data.Token);
                await SecureStorage.SetAsync("refresh_token", result.Data.RefreshToken);

                // Optionally save user info locally (e.g., for dashboard)
                Preferences.Set("user_name", userdto.FullName);
                Preferences.Set("user_email", userdto.Email);        
                Application.Current.MainPage = new AppShell();

                // Navigate to Dashboard/Main page
                //await Shell.Current.GoToAsync($"//{nameof(MainPage)}"); // make sure MainPage route exists in AppShell
                //await Application.Current.MainPage.DisplayAlert("Welcome", $"Hello {result.FullName}", "OK");
                // Save Token in SecureStorage for later API calls
                //await SecureStorage.SetAsync("auth_token", result.Token);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task GoogleLoginAsync()
        {
            // TODO: Integrate Google login (using external package)
            await Application.Current.MainPage.DisplayAlert("Google Login", "Google login clicked", "OK");
        }
    }
}
