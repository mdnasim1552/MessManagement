using MessManagement.Helpers;
using MessManagement.MVVM.Views;
using MessManagement.Services;

namespace MessManagement
{
    public partial class AppShell : Shell
    {
        private readonly UserSessionService _userSession;
        //private readonly JwtHelper _jwtHelper;
        public AppShell()
        {
            InitializeComponent();
            _userSession = App.Current.Handler.MauiContext.Services.GetService<UserSessionService>();
            //_jwtHelper = jwtHelper;
            // Register your pages with Shell
            //Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            //Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
        
        private void Button_Clicked(object sender, EventArgs e)
        {
            _userSession.ClearUser();
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("refresh_token");
            //Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            Application.Current.MainPage = new LRAppShell();
            //Application.Current.MainPage = new NavigationPage(App.Current.Handler.MauiContext.Services.GetService<LoginPage>());
            //Application.Current.MainPage = App.Current.Handler.MauiContext.Services.GetService<LoginPage>();

        }
    }
}
