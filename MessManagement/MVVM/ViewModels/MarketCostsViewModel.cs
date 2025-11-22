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

namespace MessManagement.MVVM.ViewModels
{
    public partial class MarketCostsViewModel: ObservableObject
    {
        private readonly MessMemberService _messMemberService;
        private readonly MessService _messService;
        private readonly UserSessionService _userSession;
        public ObservableCollection<MessMemberDto> Members { get; set; } = new ObservableCollection<MessMemberDto>();
        public ObservableCollection<MarketCostDto> MarketCosts { get; set; } = new ObservableCollection<MarketCostDto>();
        public ObservableCollection<MarketCostDto> _cachedMarketCosts { get; set; } = new ObservableCollection<MarketCostDto>();

        public ObservableCollection<UnitDto> Units { get; set; }=new ObservableCollection<UnitDto>();

        [ObservableProperty]
        private MessMemberDto selectedMember;
        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private bool isListEmpty;
        [ObservableProperty]
        private bool canEdit;
        public MarketCostsViewModel(MessMemberService messMemberService, MessService messService, UserSessionService userSession)
        {
            _messMemberService = messMemberService;
            _messService = messService;
            _userSession = userSession;
        }
        private void CheckIfEmpty()
        {
            IsListEmpty = CanEdit && MarketCosts.Count == 0;
        }
        [RelayCommand]
        private async Task RefreshMarketCostsAsync()
        {
            int messId = Preferences.Get("CurrentMessId", 0);
            await LoadMarketCostsAsync(messId);
        }
        [RelayCommand]
        private async Task LoadUnitsAsync()
        {
            var result = await _messService.GetUnitsAsync(); // <-- create this endpoint if not exists
            if (result.Success && result.Data?.Any() == true)
            {
                Units.Clear();
                foreach (var u in result.Data)
                    Units.Add(u);
            }
        }

        [RelayCommand]
        private async Task LoadMarketCostsAsync(int messId)
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

                    // ✅ Select the first member automatically and await it
                    //await SelectMemberAsync(messMembers.Data.First());

                    //var messMeals = await _messMemberService.GetMealsAsync(messId);
                    var marketCosts = await _messMemberService.GetMarketCostsAsync(messId);
                    _cachedMarketCosts.Clear();
                    if (marketCosts.Success && marketCosts.Data != null)
                    {
                        foreach (var mCost in marketCosts.Data)
                            _cachedMarketCosts.Add(mCost);
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
                }
                else
                {
                    Members.Clear();
                    MarketCosts.Clear();
                    _cachedMarketCosts.Clear();
                }
                CheckIfEmpty();
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

                var mCost = _cachedMarketCosts
                                            .Where(m => m.MessMemberId == member.MessMemberId)
                                            .ToList();

                MarketCosts.Clear();
                foreach (var m in mCost)
                {
                    if (m.Unit != null)
                    {
                        m.SelectedUnit = Units.FirstOrDefault(u => u.Id == m.Unit);
                    }
                    MarketCosts.Add(m);
                }
                CanEdit = SelectedMember.CanEdit;
                CheckIfEmpty();
                await Task.Delay(2000);
            }
            finally
            {
                IsBusy = false;
            }         
        }
        [RelayCommand]
        private async Task AddMarketCostsAsync(MarketCostDto marketCost)
        {
            try
            {
                // You can insert after the clicked item
                if (marketCost != null)
                {
                    var index = MarketCosts.IndexOf(marketCost);
                    if (index >= 0 && index < MarketCosts.Count)
                    {
                        MarketCosts.Insert(index + 1, new MarketCostDto
                        {
                            MessId= SelectedMember.MessId,
                            MessMemberId = SelectedMember.MessMemberId,
                            ExpenseDate = DateTime.Now,
                            ProductName = string.Empty,
                            Quantity = 0.00M,
                            Cost = 0.00M,
                            CanEdit=true
                        });
                    }
                    else
                    {
                        MarketCosts.Add(new MarketCostDto()
                        {
                            MessId = SelectedMember.MessId,
                            MessMemberId = SelectedMember.MessMemberId,
                            ExpenseDate = DateTime.Now,
                            ProductName = string.Empty,
                            Quantity = 0.00M,
                            Cost = 0.00M,
                            CanEdit = true
                        });
                    }
                }
                else
                {
                    // Add to end if no reference bill
                    MarketCosts.Add(new MarketCostDto()
                    {
                        MessId = SelectedMember.MessId,
                        MessMemberId = SelectedMember.MessMemberId,
                        ExpenseDate = DateTime.Now,
                        ProductName = string.Empty,
                        Quantity = 0.00M,
                        Cost = 0.00M,
                        CanEdit = true
                    });
                }
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding bill: {ex.Message}");
            }
        }
        [RelayCommand]
        private async Task RemoveMarketCostsAsync(MarketCostDto marketCost)
        {
            try
            {
                if (marketCost == null)
                    return;

                // Confirm it's in the collection before removing
                if (MarketCosts.Contains(marketCost))
                {
                    MarketCosts.Remove(marketCost);
                    if (marketCost.CostId > 0) // only remove from DB if already saved
                    {
                        var  rerult= await _messMemberService.DeleteMarketCostsAsync(marketCost.CostId);
                        if (rerult.Success)
                        {
                            var cachedMarketCost = _cachedMarketCosts.FirstOrDefault(m => m.CostId == marketCost.CostId);
                            if (cachedMarketCost != null)
                            {
                                _cachedMarketCosts.Remove(cachedMarketCost);
                            }
                        }

                    }
                        
                }
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing bill: {ex.Message}");
            }
        }
        public async Task SaveMarketCostsAsync(MarketCostDto marketCost)
        {
            //await _messMemberService.UpdateMealAsync(meal);
            if (marketCost == null) return;

            marketCost.Unit=marketCost.SelectedUnit.Id;
            
            if (string.IsNullOrWhiteSpace(marketCost.ProductName) ||
                   marketCost.Quantity <= 0 ||
                   marketCost.Cost <= 0)
            {
                return; // skip incomplete records
            }

            var result = await _messService.UpdateAndSaveMarketCostsAsync(marketCost);
            if (!result.Success)
            {
                // Optional: show error toast
                Console.WriteLine("Failed to save bill in database");
            }
            else
            {
                marketCost.CostId = result.Data.CostId;

                var cachedMarketCost = _cachedMarketCosts.FirstOrDefault(m => m.CostId == result.Data.CostId);
                if (cachedMarketCost != null)
                {
                    var index = _cachedMarketCosts.IndexOf(cachedMarketCost);
                    _cachedMarketCosts[index] = marketCost;
                }
                else
                {
                    _cachedMarketCosts.Add(marketCost);
                }
            }
        }
    }
}
