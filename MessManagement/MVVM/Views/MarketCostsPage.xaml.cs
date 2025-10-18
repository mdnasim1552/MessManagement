using MessManagement.MVVM.ViewModels;
using MessManagement.Shared.DTOs;

namespace MessManagement.MVVM.Views;

public partial class MarketCostsPage : ContentPage
{
    private bool _initialized;
    public MarketCostsPage(MarketCostsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_initialized)
        {
            return;
        }
        if (BindingContext is MarketCostsViewModel vm)
        {
            await vm.LoadUnitsCommand.ExecuteAsync(null);
            // Get the current MessId from Preferences
            int messId = Preferences.Get("CurrentMessId", 0);
            if (messId > 0)
            {
                await vm.LoadMarketCostsCommand.ExecuteAsync(messId);
                _initialized = true;
            }
        }
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is MarketCostsViewModel vm &&
           e.CurrentSelection.FirstOrDefault() is MessMemberDto member)
        {
            vm.IsBusy = true;
            vm.SelectMemberCommand.Execute(member);
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is MarketCostDto marketCost)
        {
            //try
            //{
            //    // Call your service or ViewModel method to save the meal
            //    ((MarketCostsViewModel)BindingContext).SaveMarketCostsAsync(marketCost);
            //}
            //catch (Exception ex)
            //{
            //    // Optional: handle error (show toast, log, etc.)
            //    Console.WriteLine($"Failed to save meal: {ex.Message}");
            //}
            Task.Run(async () =>
            {
                try
                {
                    await ((MarketCostsViewModel)BindingContext).SaveMarketCostsAsync(marketCost);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving: {ex.Message}");
                }
            });
        }
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        Entry_Unfocused(sender, null);
    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (sender is DatePicker picker && picker.BindingContext is MarketCostDto marketCost)
        {
            //((MarketCostsViewModel)BindingContext).SaveMarketCostsAsync(marketCost);
            Task.Run(async () =>
            {
                try
                {
                    await ((MarketCostsViewModel)BindingContext).SaveMarketCostsAsync(marketCost);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving: {ex.Message}");
                }
            });
        }

    }
}