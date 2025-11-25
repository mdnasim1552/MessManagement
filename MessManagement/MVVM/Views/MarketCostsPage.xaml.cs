using MessManagement.MVVM.ViewModels;
using MessManagement.Shared.DTOs;

namespace MessManagement.MVVM.Views;

public partial class MarketCostsPage : ContentPage
{
    private int _initialized=0;
    public MarketCostsPage(MarketCostsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
    public async Task LoadMarketCosts(int messId)
    {
        if (_initialized == messId)
        {
            return;
        }
        if (BindingContext is MarketCostsViewModel vm)
        {
            await vm.LoadUnitsCommand.ExecuteAsync(null);
            // Get the current MessId from Preferences           
            if (messId > 0)
            {
                await vm.LoadMarketCostsCommand.ExecuteAsync(messId);
                _initialized = messId;
            }
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        int messId = Preferences.Get("CurrentMessId", 0);
        await LoadMarketCosts(messId);
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is MarketCostsViewModel vm &&
           e.CurrentSelection.FirstOrDefault() is MessMemberDto member)
        {
            vm.IsBusy = true;
            if (!vm.IsInternalSelectionChange)
                vm.SaveMarketCostBeforeNaviagateAsync();
            vm.SelectMemberCommand.Execute(member);
        }
    }
    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (sender is Picker picker && picker.BindingContext is MarketCostDto marketCost)
        {
            ((MarketCostsViewModel)BindingContext).UpdateUnit(marketCost);
        }
    }
}