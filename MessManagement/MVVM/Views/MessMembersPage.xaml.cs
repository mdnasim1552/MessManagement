using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class MessMembersPage : ContentPage
{
    public MessMembersPage(MessMembersViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MessMembersViewModel vm)
        {
            // Get the current MessId from Preferences
            int messId = Preferences.Get("CurrentMessId", 0);
            if (messId > 0)
            {
                // Reload meals every time the page appears
                await vm.LoadMessMemberSummaryCommand.ExecuteAsync(messId);
                // Optionally select the first member automatically
                //if (vm.Members.Any())
                //    await vm.SelectMemberCommand.ExecuteAsync(vm.Members.First());
            }
        }
    }  
}