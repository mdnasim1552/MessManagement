using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.MVVM.Views;
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
    public partial class MessMembersViewModel: ObservableObject
    {
        private readonly UserSessionService _userSession;
        private readonly MessMemberService _messMemberService;
        [ObservableProperty]
        private bool isBusy;

        public ObservableCollection<MessMemberSummaryDto> MemberSummary { get; set; }= new ObservableCollection<MessMemberSummaryDto>();
        public MessMembersViewModel(MessMemberService messMemberService, UserSessionService userSession)
        {
            _messMemberService = messMemberService;
            _userSession = userSession;
        }
        [RelayCommand]
        private async Task LoadMessMemberSummaryAsync(int messId)
        {
            try
            {
                IsBusy = true;
                var result = await _messMemberService.GetMessMemberSummaryAsync(messId);
                if (result.Success && result.Data?.Any() == true)
                {
                    MemberSummary.Clear();
                    foreach (var member in result.Data)
                        MemberSummary.Add(member);
                }
                else
                {
                    MemberSummary.Clear();
                }
            }
            finally
            {
                IsBusy = false;
            }            
        }
        [RelayCommand]
        private async Task DeleteMemberAsync(MessMemberSummaryDto member)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{member.Name}'?",
                "Yes", "Cancel");

            if (!confirm)
                return;
            var memberDto = new MessMemberDto
            {
                MessMemberId = member.MessMemberId,
                MessId = member.MessId,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
                Rent = member.Rent
            };
            var result = await _messMemberService.DeleteMessMemberAsync(memberDto);
            if (result.Success)
            {
                MemberSummary.Remove(member);
                var toast = Toast.Make("Member deleted successfully.", ToastDuration.Short, 12);
                await toast.Show();
            }
            else
            {
                var toast = Toast.Make(result.Message ?? "Failed to delete member.", ToastDuration.Short, 12);
                await toast.Show();
            }

        }
        [RelayCommand]
        private async Task ShowMemberEditorAsync(MessMemberSummaryDto member)
        {
            int messId = Preferences.Get("CurrentMessId", 0);
            var lastUrl = Preferences.Get("MessDetailsTabBarUrl", string.Empty);
            if (member == null)
                member = new MessMemberSummaryDto() { MessId= messId };
            var popuppage = new MessMemberEditorPopup(member);

            // Show the popup via Shell
            //await Shell.Current.CurrentPage.ShowPopupAsync(popuppage);

            //await Application.Current.MainPage.ShowPopupAsync(popuppage, new PopupOptions { PageOverlayColor = Colors.Transparent });

            if (popuppage.BindingContext is MessMemberEditorViewModel vm)
            {
                // Set refresh callback
                vm.OnMemberSaved = async () =>
                {
                    if (messId > 0)
                        await LoadMessMemberSummaryCommand.ExecuteAsync(messId);
                };
            }

            await Shell.Current.CurrentPage.ShowPopupAsync(popuppage, new PopupOptions
            {
                PageOverlayColor = Colors.Transparent
            });
        }
    }
}
