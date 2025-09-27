namespace MessManagement.MVVM.Views;

public partial class LRAppShell : Shell
{
	public LRAppShell()
	{
        InitializeComponent();
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
    }
}