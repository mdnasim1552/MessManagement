using MessManagement.MVVM.Models;
using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;
public partial class CommonBillPage : ContentPage
{
    private int _initialized=0;

    public CommonBillPage(CommonBillViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    public async Task LoadCommonBill(int messId)
    {
        if (_initialized == messId)
        {
            return;
        }
        if (BindingContext is CommonBillViewModel vm)
        {
            // Get the current MessId from Preferences

            if (messId > 0)
            {
                // Reload meals every time the page appears
                await vm.LoadCommonBillCommand.ExecuteAsync(messId);
                _initialized = messId;
                // Optionally select the first member automatically
                //if (vm.Members.Any())
                //    await vm.SelectMemberCommand.ExecuteAsync(vm.Members.First());
            }
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        int messId = Preferences.Get("CurrentMessId", 0);
        await LoadCommonBill(messId);
        //if (_initialized == messId)
        //{
        //    return;
        //}
        //if (BindingContext is CommonBillViewModel vm)
        //{
        //    // Get the current MessId from Preferences

        //    if (messId > 0)
        //    {
        //        // Reload meals every time the page appears
        //        await vm.LoadCommonBillCommand.ExecuteAsync(messId);
        //        _initialized = messId;
        //        // Optionally select the first member automatically
        //        //if (vm.Members.Any())
        //        //    await vm.SelectMemberCommand.ExecuteAsync(vm.Members.First());
        //    }
        //}
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is CommonBillModel bill)
        {
            Task.Run(async () =>
            {
                try
                {
                    int messId = Preferences.Get("CurrentMessId", 0);
                    bill.MessId = messId;
                    await ((CommonBillViewModel)BindingContext).SaveBillAsync(bill);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save bill: {ex.Message}");
                }
            });
        }

    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        Entry_Unfocused(sender, null);
    }
}