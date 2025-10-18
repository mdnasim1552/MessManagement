using MessManagement.MVVM.ViewModels;
using MessManagement.Shared.DTOs;

namespace MessManagement.MVVM.Views;

public partial class MealsPage : ContentPage
{
    private bool _initialized;
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
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_initialized)
        {
            return;
        }
        if (BindingContext is MealsViewModel vm)
        {
            // Get the current MessId from Preferences
            int messId = Preferences.Get("CurrentMessId", 0);
            if (messId > 0)
            {
                // Reload meals every time the page appears
                await vm.LoadMealsCommand.ExecuteAsync(messId);
                _initialized = true;
                // Optionally select the first member automatically
                //if (vm.Members.Any())
                //    await vm.SelectMemberCommand.ExecuteAsync(vm.Members.First());
            }
        }
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BindingContext is MealsViewModel vm &&
            e.CurrentSelection.FirstOrDefault() is MessMemberDto member)
        {
            vm.IsBusy = true;
            vm.SelectMemberCommand.Execute(member);
        }
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is MealDto meal)
        {
            
            Task.Run(async () =>
            {
                try
                {
                    // Call your service or ViewModel method to save the meal
                    await ((MealsViewModel)BindingContext).SaveMealAsync(meal);
                }
                catch (Exception ex)
                {
                    // Optional: handle error (show toast, log, etc.)
                    Console.WriteLine($"Failed to save meal: {ex.Message}");
                }
            });
        }
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        Entry_Unfocused(sender, null);
    }
    //private void ScrollToToday()
    //{
    //    if (BindingContext is MealsViewModel vm)
    //    {
    //        // Find the first meal whose date is today
    //        var todayMeal = vm.Meals.FirstOrDefault(m => m.MealDate == DateOnly.FromDateTime(DateTime.Now));
    //        if (todayMeal != null)
    //        {
    //            // Scroll the CollectionView to that item
    //            MealsCollectionView.ScrollTo(todayMeal, position: ScrollToPosition.Start, animate: true);
    //        }
    //    }
    //}


}