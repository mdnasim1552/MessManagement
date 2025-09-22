using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}