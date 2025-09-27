using MessManagement.Helpers;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
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
                MainPage = new AppShell();
            }
            else
            {
                //MainPage = new NavigationPage(_serviceProvider.GetService<LoginPage>());
                MainPage = new LRAppShell();
            }
        }
    }
}