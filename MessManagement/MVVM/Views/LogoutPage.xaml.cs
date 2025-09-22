namespace MessManagement.MVVM.Views;

public partial class LogoutPage : ContentPage
{
	public LogoutPage()
	{
		InitializeComponent();
        LogoutAsync();
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Remove("auth_token");
        SecureStorage.Remove("refresh_token");
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}