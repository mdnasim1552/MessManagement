using MessManagement.Helpers;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;


namespace MessManagement
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly JwtHelper _jwtHelper;
        public App(IServiceProvider serviceProvider, JwtHelper jwtHelper)
        {
            _serviceProvider = serviceProvider;
            _jwtHelper = jwtHelper;
            InitializeComponent();
            //GoogleClientManager.Initialize("YOUR_WEB_CLIENT_ID.apps.googleusercontent.com");
            MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Color= Colors.Black,
                    WidthRequest=50,
                    HeightRequest=50
                }
            };          
        }
        protected override async void OnStart()
        {
            base.OnStart();
            await SetMainPage();
        }
        private async Task SetMainPage()
        {
            if (await _jwtHelper.CheckLoginStatusAsync())
            {
                _jwtHelper.SetCurrentUser();
                MainPage = new AppShell();
                //MainPage = new NavigationPage(new AppShell());
            }
            else
            {
                _jwtHelper.ClearCurrentUser();
                //MainPage = new NavigationPage(_serviceProvider.GetService<LoginPage>());
                MainPage = new LRAppShell();
                //MainPage = new NavigationPage(new LRAppShell());
            }
        }
    }
}