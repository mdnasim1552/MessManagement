using CommunityToolkit.Mvvm.Messaging;
using MessManagement.Helpers;
using MessManagement.MVVM.ViewModels;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using System.Net.Http.Headers;

namespace MessManagement
{
    public partial class AppShell : Shell
    {
        private readonly UserSessionService _userSession;
        private readonly JwtHelper _jwtHelper;
        private readonly AppShellViewModel _appShellViewModel;
        public AppShell()
        {
            InitializeComponent();
            _userSession = App.Current.Handler.MauiContext.Services.GetService<UserSessionService>();
            _jwtHelper = App.Current.Handler.MauiContext.Services.GetService<JwtHelper>();
            _appShellViewModel = App.Current.Handler.MauiContext.Services.GetService<AppShellViewModel>();
            BindingContext = _appShellViewModel;

            MainThread.BeginInvokeOnMainThread(async () => await SetDefaultPage());
            this.Navigating += AppShell_Navigating;
            //Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        }
        private async Task SetDefaultPage()
        {
            if (_userSession.CurrentUser.CurrentMessId == null)
            {
                await GoToAsync($"//{nameof(MessWizardPage)}");
            }
            else
            {
                //await GoToAsync("//MessDetailsTabBar/MessMembersPage");
                await GoToAsync($"//MessDetailsTabBar/MessMembersPage?messId={_userSession.CurrentUser.CurrentMessId}");

            }
        }
        private async void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Target.Location.OriginalString.Contains("MessDetailsTabBar") || e.Target.Location.OriginalString.Contains("MessDetailsTabBarList"))
            {
                if (e.Target.Location.OriginalString.Contains("MessMembersPage"))
                {
                    Preferences.Set("MessDetailsTabBarUrl", e.Target.Location.OriginalString);
                }   
            }
        }

        private async void LogoutButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Color = Colors.Black,
                    WidthRequest = 50,
                    HeightRequest = 50
                }
            };
            await Task.Delay(2000);
            _jwtHelper.ClearCurrentUser();
            Application.Current.MainPage = new LRAppShell();
        }
    }
}
