using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class MessListPage : ContentPage
{
	public MessListPage(MessListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MessListViewModel vm)
            await vm.LoadMessesAsync();
    }
}