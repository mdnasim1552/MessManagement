using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;

    //    UpdateNavigationBarColor();
    //}
    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();

    //    Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
    //}
    //private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    //{
    //    UpdateNavigationBarColor();
    //}
    //private void UpdateNavigationBarColor()
    //{
    //    var navPage = (NavigationPage)this.Parent;
    //    navPage.BarBackgroundColor = Application.Current.RequestedTheme switch
    //    {
    //        AppTheme.Dark => Colors.Black,
    //        _ => Color.FromArgb("#F7F8FA")
    //    };
    //    navPage.BarTextColor = Application.Current.RequestedTheme switch
    //    {
    //        AppTheme.Dark => Colors.White,
    //        _ => Colors.Black
    //    };
    //    this.BackgroundColor = Application.Current.RequestedTheme switch
    //    {
    //        AppTheme.Dark => Colors.Black,
    //        _ => Color.FromArgb("#F7F8FA")
    //    };
    //}
}