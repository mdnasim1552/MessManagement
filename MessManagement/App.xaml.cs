using MessManagement.Helpers;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using MessManagement.Shared.DTOs;

namespace MessManagement
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        public App(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
            MainPage = new AppShell();
            CheckLoginStatusAsync();

        }
        private async void CheckLoginStatusAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");

                if (string.IsNullOrEmpty(token))
                {
                    // No token → navigate to LoginPage
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    // Token exists → optional: check expiry
                    bool isExpired = JwtHelper.IsTokenExpired(token);
                    if (isExpired)
                    {
                        var refreshToken = await SecureStorage.GetAsync("refresh_token");

                        var result =await _authService.RefreshTokenAsync(refreshToken);
                        if (result == null)
                        {
                            // Refresh failed → go to login
                            await SecureStorage.SetAsync("auth_token", "");
                            await SecureStorage.SetAsync("refresh_token", "");
                            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                        }
                        else
                        {
                            await SecureStorage.SetAsync("auth_token", result.Data.Token);
                            await SecureStorage.SetAsync("refresh_token", result.Data.RefreshToken);
                            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                        }
                        
                    }
                    else
                    {
                        // Token valid → go to Dashboard/MainPage
                        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                    }
                }
            }
            catch
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}