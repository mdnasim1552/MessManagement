using CommunityToolkit.Maui;
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
        private async Task ShowMemberEditorAsync(MessMemberSummaryDto member)
        {
            var popuppage = new MessMemberEditorPopup(member);

            // Show the popup via Shell
            //await Shell.Current.CurrentPage.ShowPopupAsync(popuppage);

            //await Application.Current.MainPage.ShowPopupAsync(popuppage, new PopupOptions { PageOverlayColor = Colors.Transparent });

            if (popuppage.BindingContext is MessMemberEditorViewModel vm)
            {
                // Set refresh callback
                vm.OnMemberSaved = async () =>
                {
                    int messId = _userSession.CurrentUser.CurrentMessId ?? 0;
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
