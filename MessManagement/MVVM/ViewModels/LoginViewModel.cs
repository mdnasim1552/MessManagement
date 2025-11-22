using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Auth;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace MessManagement.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly UserSessionService _userSession;
        private readonly IGoogleAuthService _googleAuthService;

        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private bool isBusy;


        //public ICommand LoginCommand { get; }
        //public ICommand GoogleLoginCommand { get; }
        //public ICommand RegisterCommand { get; }

        public LoginViewModel(AuthService authService, UserSessionService userSession, IGoogleAuthService googleAuthService)
        {
            _authService = authService;
            _userSession = userSession;
            _googleAuthService = googleAuthService;
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
                IsBusy = true;
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

                Preferences.Set("current_user", JsonSerializer.Serialize(userdto));

                // Save in-memory session
                _userSession.SetUser(userdto);

                Application.Current.MainPage = new AppShell();
                //Application.Current.MainPage = new NavigationPage(new AppShell());

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
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task GoogleLoginAsync()
        {
            try
            {
                //var authUrl = "https://guileless-launa-unrealizable.ngrok-free.dev/api/Auth/start-google-login";
                //var authUrl = "https://mdnasim.bsite.net/api/Auth/start-google-login";
#if ANDROID
                var idToken = await _googleAuthService.SignInAsync();
                //await App.Current.MainPage.DisplayAlert("Success", $"ID Token: {idToken}", "OK");
                var result = await _authService.GoogleLoginAsync(idToken);
                if (result == null || result.Data.Token == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                    return;
                }

                var userdto = result.Data.User;
                // Save JWT & Refresh token securely
                await SecureStorage.SetAsync("auth_token", result.Data.Token);
                await SecureStorage.SetAsync("refresh_token", result.Data.RefreshToken);

                Preferences.Set("current_user", JsonSerializer.Serialize(userdto));

                // Save in-memory session
                _userSession.SetUser(userdto);

                Application.Current.MainPage = new AppShell();
#elif IOS
            //await App.Current.MainPage.DisplayAlert("Message", "For ios, google login is not implemented yet.", "OK");
            var webClientId = "966817363123-5itk3nqocncp3e9323vv6boasbnjcnmg.apps.googleusercontent.com";
            var redirectUri = new Uri("messmanagement://auth");

            var authUrl = new Uri(
                $"https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={webClientId}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri.ToString())}" +
                $"&response_type=token" +
                $"&scope=openid%20email%20profile"
            );

            var authresult = await WebAuthenticator.Default.AuthenticateAsync(authUrl, redirectUri);

            var idToken = authresult?.Properties["id_token"];

             var result = await _authService.GoogleLoginAsync(idToken);
                if (result == null || result.Data.Token == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Login Failed", "Invalid credentials.", "OK");
                    return;
                }

                var userdto = result.Data.User;
                // Save JWT & Refresh token securely
                await SecureStorage.SetAsync("auth_token", result.Data.Token);
                await SecureStorage.SetAsync("refresh_token", result.Data.RefreshToken);

                Preferences.Set("current_user", JsonSerializer.Serialize(userdto));

                // Save in-memory session
                _userSession.SetUser(userdto);

                Application.Current.MainPage = new AppShell();
#elif WINDOWS
                    // Windows: just open browser
                //await Launcher.Default.OpenAsync(authUrl);
                await Application.Current.MainPage.Navigation.PushAsync(new WindowsGoogleLoginPage());
#endif
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        
    }
}
