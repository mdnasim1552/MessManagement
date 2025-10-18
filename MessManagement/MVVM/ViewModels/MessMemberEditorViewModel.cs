using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.Helpers;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.ViewModels
{
    public partial class MessMemberEditorViewModel: ObservableObject
    {
        private readonly MessMemberService _messMemberService;
        [ObservableProperty] private MessMemberSummaryDto member=new MessMemberSummaryDto();
        public Action? OnMemberSaved { get; set; }

        public Popup Popup { get; set; }
        public MessMemberEditorViewModel(MessMemberService messMemberService)
        {
            _messMemberService = messMemberService;
        }
        [RelayCommand]
        private async Task SaveMemberAsync()
        {
            try
            {
                var memberDto = new MessMemberDto
                {
                    MessMemberId = Member.MessMemberId,
                    Name = Member.Name,
                    Email = Member.Email,
                    Role = Member.Role,
                    Rent = Member.Rent
                };
                var result = await _messMemberService.UpdateMessMemberInfoAsync(memberDto);
                if (!result.Success)
                {
                    var errortoast = Toast.Make(result.Message ?? "Failed to update member!", ToastDuration.Short, 12);
                    await errortoast.Show();
                    //if (Popup is not null)
                    //    await Popup.CloseAsync();
                    //await Shell.Current.DisplayAlert("Error", result.Message ?? "Failed to update member!", "OK");
                    return;
                }
                //if (Popup is not null)
                //    await Popup.CloseAsync();
                if (Popup != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () => await Popup.CloseAsync());
                }

                // small delay ensures overlay fully removed
                await Task.Delay(50);
                //await Shell.Current.GoToAsync(".."); // navigate back

                //await Popup.CloseAsync();
                var toast = Toast.Make("Member updated!", ToastDuration.Short, 12);
                await toast.Show();
                OnMemberSaved?.Invoke();
            }
            catch(Exception ex)
            {
                var toast = Toast.Make($"Error: {ex.Message}", ToastDuration.Short, 12);
                await toast.Show();
            }

        }
        [RelayCommand]
        private async Task CancelMemberAsync()
        {
            if (Popup is not null)
                await Popup.CloseAsync();
        }
        public void Initialize(MessMemberSummaryDto member)
        {
            Member = member;
        }
    }
}
