using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}