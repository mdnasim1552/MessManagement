using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System.Text.Json;

namespace MessManagement.MVVM.Views;

public partial class WindowsGoogleLoginPage : ContentPage
{
    //private readonly string _authUrl = "https://guileless-launa-unrealizable.ngrok-free.dev/api/Auth/start-google-login-windows";
    //private readonly string WindowsRedirectUrl = "https://guileless-launa-unrealizable.ngrok-free.dev/api/Auth/windows-return";
    private readonly string _authUrl = "https://mdnasim.bsite.net/api/Auth/start-google-login-windows";
    private readonly string WindowsRedirectUrl = "https://mdnasim.bsite.net/api/Auth/windows-return";
    private readonly UserSessionService _userSession;
    public WindowsGoogleLoginPage()
    {
        InitializeComponent();
        LoginWebView.Source = _authUrl;
        _userSession = App.Current.Handler.MauiContext.Services.GetService<UserSessionService>();
    }

    private async void LoginWebView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        var url = e.Url;

        // Our API will redirect to this
        if (url.StartsWith(WindowsRedirectUrl))
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            var userdto = new UserDto
            {
                Id = int.Parse(query["id"] ?? "0"),
                FullName = query["fullName"] ?? "",
                Email = query["email"] ?? "",
                GoogleId = query["googleId"],
                CreatedAt = DateTime.TryParse(query["createdAt"], out var c) ? c : null,
                UpdatedAt = DateTime.TryParse(query["updatedAt"], out var u) ? u : null,
                CurrentMessId = int.TryParse(query["currentMessId"], out var m) ? m : null,
                ProfilePicture = !string.IsNullOrEmpty(query["profilePicture"])
                    ? Convert.FromBase64String(query["profilePicture"])
                    : null
            };

            var token = query["token"] ?? "";
            var refreshToken = query["refreshToken"] ?? "";

            await SecureStorage.SetAsync("auth_token", token);
            await SecureStorage.SetAsync("refresh_token", refreshToken);

            Preferences.Set("current_user", JsonSerializer.Serialize(userdto));
            _userSession.SetUser(userdto);
            Application.Current.MainPage = new AppShell();
        }
    }
}