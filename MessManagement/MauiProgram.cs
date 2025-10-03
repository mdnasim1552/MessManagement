using CommunityToolkit.Maui;
using MessManagement.Helpers;
using MessManagement.MVVM.ViewModels;
using MessManagement.MVVM.Views;
using MessManagement.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace MessManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            //var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            //                ? "http://10.0.2.2:5242/"   // Android emulator uses 10.0.2.2
            //                : "https://localhost:7223/"; // Windows/iOS simulator

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });
            // 🔹 Determine environment
            string environment = "Production";
#if DEBUG
            builder.Logging.AddDebug();
            environment = "Development";
#endif
            // 🔹 Load config files
            var assembly = typeof(MauiProgram).Assembly;

            var configBuilder = new ConfigurationBuilder();

            // base config
            using var baseStream = assembly.GetManifestResourceStream("MessManagement.appsettings.json");
            configBuilder.AddJsonStream(baseStream!);

            // environment-specific config (if exists)
            using var envStream = assembly.GetManifestResourceStream($"MessManagement.appsettings.{environment}.json");
            if (envStream != null)
                configBuilder.AddJsonStream(envStream);

            var config = configBuilder.Build();

            // 🔹 Get base URL
            string apiBaseUrl;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                apiBaseUrl = config["ApiBaseUrl_Android"] ?? config["ApiBaseUrl"];
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                apiBaseUrl = config["ApiBaseUrl_Windows"] ?? config["ApiBaseUrl"];
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                apiBaseUrl = config["ApiBaseUrl_iOS"] ?? config["ApiBaseUrl"];
            }
            else
            {
                apiBaseUrl = config["ApiBaseUrl"];
            }

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MessWizardViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<MessWizardPage>();

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<MessService>();
            builder.Services.AddSingleton<JwtHelper>();
            builder.Services.AddSingleton<UserSessionService>();

            builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(apiBaseUrl));
            builder.Services.AddHttpClient<MessService>(c => c.BaseAddress = new Uri(apiBaseUrl));

            return builder.Build();
        }
    }
}
