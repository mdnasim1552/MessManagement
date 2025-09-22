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
    public partial class MessWizardViewModel: ObservableObject
    {
        public ObservableCollection<MessMemberModel> Members { get; set; } = new ObservableCollection<MessMemberModel>();

        private readonly MessService _messService;

        [ObservableProperty] private bool isStep1Visible = true;
        [ObservableProperty] private bool isStep2Visible = false;

        [ObservableProperty] private string messName;
        [ObservableProperty] private string description;
        [ObservableProperty] private DateTime month = DateTime.Now;
        public event Action<MessMemberModel>? MemberAdded;
        public MessWizardViewModel(MessService messService)
        {
            _messService = messService;
        }
        [RelayCommand]
        private async Task NextButtonAsync()
        {
            IsStep1Visible = false;
            IsStep2Visible = true;
            // Ensure at least one member row exists
            if (Members.Count == 0)
            {
                Members.Add(new MessMemberModel());
                UpdateHeadings();
            }              
        }
        [RelayCommand]
        private async Task AddMemberButtonAsync()
        {
            var newMember = new MessMemberModel();
            Members.Add(newMember);
            UpdateHeadings();
            MemberAdded?.Invoke(newMember);

        }
        [RelayCommand]
        private async Task BackButtonAsync()
        {
            IsStep1Visible = true;
            IsStep2Visible = false;
        }
        [RelayCommand]
        private async Task FinishButtonAsync()
        {
            try
            {
                var messDto = new MessDto()
                {
                    MessName = MessName,
                    Description = Description,
                    Month = Month,
                    MessMembers = Members.Select(m => new MessMemberDto
                    {
                        Name = m.Name,
                        Email = m.Email,
                        Role = m.Role
                    }).ToList()
                };
                var result = await _messService.CreateMessAsync(messDto);
                if (result != null && result.Success) {
                    await Application.Current.MainPage.DisplayAlert("Success", result.Data, "OK");
                }
                else
                {
                    var errorMessage = result?.Message ?? $"Mess '{MessName}' is not created!";
                    await Application.Current.MainPage.DisplayAlert("Failed", errorMessage, "OK");
                }
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                // Handle exceptions
            }
        }
        [RelayCommand]
        private async Task RemoveMemberAsync(MessMemberModel member)
        {
            if (member != null && Members.Contains(member))
            {
                Members.Remove(member);
                UpdateHeadings();
            }              
        }
        private void UpdateHeadings()
        {
            for (int i = 0; i < Members.Count; i++)
            {
                int number = i + 1;
                Members[i].Heading = number switch
                {
                    1 => "1st Member",
                    2 => "2nd Member",
                    3 => "3rd Member",
                    _ => $"{number}th Member"
                };
            }
        }
    }
}
