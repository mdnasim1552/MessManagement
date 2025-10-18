using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MessManagement.MVVM.ViewModels
{
    public partial class MealsViewModel: ObservableObject
    {
        private readonly MessMemberService _messMemberService;
        private readonly UserSessionService _userSession;

        public ObservableCollection<MessMemberDto> Members { get; set; } = new ObservableCollection<MessMemberDto>();
        public ObservableCollection<MealDto> Meals { get; set; } = new ObservableCollection<MealDto>();
        private ObservableCollection<MealDto> _cachedMeals { get; set; } = new ObservableCollection<MealDto>();

        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private MessMemberDto selectedMember;

        public event Action? MealDtoScrollCurrentDate;

        public MealsViewModel(MessMemberService messMemberService, UserSessionService userSession)
        {
            _messMemberService = messMemberService;
            _userSession = userSession;
        }

        [RelayCommand]
        private async Task LoadMealsAsync(int messId)
        {
            try
            {
                IsBusy = true;
                var messMembers = await _messMemberService.GetMessMembersAsync(messId);
                if (messMembers.Success && messMembers.Data?.Any() == true)
                {
                    Members.Clear();
                    foreach (var member in messMembers.Data)
                        Members.Add(member);

                    var messMeals = await _messMemberService.GetMealsAsync(messId);
                    _cachedMeals.Clear();
                    if (messMeals.Success && messMeals.Data != null)
                    {
                        foreach (var meal in messMeals.Data)
                            _cachedMeals.Add(meal);
                    }
                    var selectMember = messMembers.Data.FirstOrDefault(m => m.Email == _userSession.CurrentUser.Email);
                    if (selectMember == null)
                    {
                        await SelectMemberAsync(messMembers.Data.First());
                    }
                    else
                    {
                        await SelectMemberAsync(selectMember);
                    }
                        // Select first member                       
                }
                else
                {
                    Members.Clear();
                    Meals.Clear();
                    _cachedMeals.Clear();
                }
            }
            finally
            {
                IsBusy = false;
            }           
        }

        [RelayCommand]
        private async Task SelectMemberAsync(MessMemberDto member)
        {
            try
            {
                if (member == null)
                    return;

                SelectedMember = member;

                var memberMeals = _cachedMeals
                                        .Where(m => m.MessMemberId == member.MessMemberId)
                                        .ToList();

                Meals.Clear();
                foreach (var meal in memberMeals)
                    Meals.Add(meal);

                MealDtoScrollCurrentDate?.Invoke();
                await Task.Delay(2000);

            }
            finally
            {
                IsBusy = false;
            }
            
        }
        public async Task SaveMealAsync(MealDto meal)
        {
            var result = await _messMemberService.UpdateMealAsync(meal);
            if (!result.Success)
                return;
            meal.MealId = result.Data.MealId;
            // 🟢 Update in cache
            var cachedMeal = _cachedMeals.FirstOrDefault(m => m.MealId == meal.MealId);
            if (cachedMeal != null)
            {
                var index = _cachedMeals.IndexOf(cachedMeal);
                _cachedMeals[index] = meal;
                //_cachedMeals[index].Breakfast = meal.Breakfast;
                //_cachedMeals[index].Lunch = meal.Lunch;
                //_cachedMeals[index].Dinner = meal.Dinner;
            }
            else
            {
                _cachedMeals.Add(meal);
            }
        }

    }
}
