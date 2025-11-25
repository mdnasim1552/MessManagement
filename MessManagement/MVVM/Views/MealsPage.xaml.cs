using CommunityToolkit.Mvvm.Messaging;
using MessManagement.MVVM.ViewModels;
using MessManagement.Shared.DTOs;

namespace MessManagement.MVVM.Views;

public partial class MealsPage : ContentPage
{
    private int _initialized=0;
    public MealsPage(MealsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.MealDtoScrollCurrentDate += () =>
        {
            var todayMeal = vm.Meals.FirstOrDefault(m => m.MealDate == DateOnly.FromDateTime(DateTime.Now));
            if (todayMeal != null)
            {
                MealsCollectionView.Dispatcher.DispatchAsync(() =>
                {
                    MealsCollectionView.ScrollTo(
                        item: todayMeal,
                        position: ScrollToPosition.Center,
                        animate: true
                    );
                });
            }
        };

    }
    public async Task LoadMeals(int messId)
    {
        if (_initialized == messId)
        {
            return;
        }
        if (BindingContext is MealsViewModel vm)
        {
            if (messId > 0)
            {
                await vm.LoadMealsCommand.ExecuteAsync(messId);
                _initialized = messId;
            }
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        int messId = Preferences.Get("CurrentMessId", 0);
        await LoadMeals(messId);
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is MealsViewModel vm &&
            e.CurrentSelection.FirstOrDefault() is MessMemberDto member)
        {
            vm.IsBusy = true;
            if (!vm.IsInternalSelectionChange)
                vm.SaveMealBeforeNaviagteAsync();
            vm.SelectMemberCommand.Execute(member);
        }
    }

}