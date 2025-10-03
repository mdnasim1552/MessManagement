using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessManagement.MVVM.Models;
using MessManagement.Services;
using MessManagement.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.MVVM.ViewModels
{
    public partial class MessWizardViewModel: ObservableObject
    {
        public ObservableCollection<MessMemberModel> Members { get; set; } = new ObservableCollection<MessMemberModel>();
        public ObservableCollection<CommonBillModel> CommonBills { get; set; } = new ObservableCollection<CommonBillModel>();
        private readonly MessService _messService;
        private readonly UserSessionService _userSession;

        //[ObservableProperty] private int currentStep=1;
        //[ObservableProperty] private bool isStep1Visible = true;
        //[ObservableProperty] private bool isStep2Visible = false;
        //[ObservableProperty] private bool isStep3Visible = false;
        [ObservableProperty]
        private int currentStep = 1;

        public bool IsStep1Visible => CurrentStep == 1;
        public bool IsStep2Visible => CurrentStep == 2;
        public bool IsStep3Visible => CurrentStep == 3;

        partial void OnCurrentStepChanged(int value)
        {
            OnPropertyChanged(nameof(IsStep1Visible));
            OnPropertyChanged(nameof(IsStep2Visible));
            OnPropertyChanged(nameof(IsStep3Visible));
        }

        [ObservableProperty] private string messName;
        [ObservableProperty] private string description;
        [ObservableProperty] private DateTime month = DateTime.Now;
        public event Action<MessMemberModel>? MemberAdded;
        public event Action<CommonBillModel>? CommonBillAdded;
        public MessWizardViewModel(MessService messService, UserSessionService userSession)
        {
            _messService = messService;
            _userSession = userSession;
        }       
        [RelayCommand]
        private async Task NextButtonAsync()
        {
            if (CurrentStep == 1)
            {
                if (string.IsNullOrWhiteSpace(MessName) || string.IsNullOrWhiteSpace(Description))
                {
                    await Application.Current.MainPage.DisplayAlert("Validation Error", "Mess name and escription are required.", "OK");
                    return;
                }
                //IsStep1Visible = false;
                //IsStep2Visible = true;
                var currentUser = _userSession.CurrentUser;

                // Ensure at least one member row exists
                if (Members.Count == 0)
                {
                    Members.Add(new MessMemberModel()
                    {
                        Name = currentUser.FullName,
                        Email = currentUser.Email,
                        Role = "Manager"
                    });
                    UpdateHeadings();
                }
                CurrentStep++;
            }
            else if (CurrentStep == 2)
            {
                if (Members.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Validation Error",
                        $"Members are required.",
                        "OK");
                    return;
                }
                var invalidMember = Members.FirstOrDefault(m =>string.IsNullOrWhiteSpace(m.Name) || string.IsNullOrWhiteSpace(m.Email));
                if (invalidMember != null)
                {
                    int index = Members.IndexOf(invalidMember);
                    await Application.Current.MainPage.DisplayAlert(
                        "Validation Error",
                        $"Member #{index + 1}: Name and Email are required.",
                        "OK");

                    // 🔹 Raise event so UI can navigate/scroll to the invalid row
                    MemberAdded?.Invoke(invalidMember);
                    return; // stop navigation
                }

                if (CommonBills.Count == 0)
                {
                    CommonBills.Add(new CommonBillModel());
                    UpdateHeadings();
                }
                CurrentStep++;
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
        private async Task AddCommonBillButtonAsync()
        {
            var newCommonBill = new CommonBillModel();
            CommonBills.Add(newCommonBill);
            UpdateHeadings();
            CommonBillAdded?.Invoke(newCommonBill);

        }
        [RelayCommand]
        private async Task BackButtonAsync()
        {
            if (CurrentStep > 1)
                CurrentStep--;
            //IsStep1Visible = true;
            //IsStep2Visible = false;
        }
        [RelayCommand]
        private async Task FinishButtonAsync()
        {
            try
            {

                var invalidCommonBill = CommonBills.FirstOrDefault(cb =>string.IsNullOrWhiteSpace(cb.BillType) || cb.Amount <= 0m);

                if (invalidCommonBill != null)
                {
                    int index = CommonBills.IndexOf(invalidCommonBill);
                    await Application.Current.MainPage.DisplayAlert(
                        "Validation Error",
                        $"Common bill #{index + 1}: BillType and Amount are required.",
                        "OK");

                    // 🔹 Raise event so UI can navigate/scroll to the invalid row
                    CommonBillAdded?.Invoke(invalidCommonBill);
                    return; // stop navigation
                }


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
                    }).ToList(),
                    CommonBills = CommonBills.Select(cb => new CommonBillDto
                    {
                        BillType = cb.BillType,
                        Amount = cb.Amount,
                    }).ToList(),
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
        [RelayCommand]
        private async Task RemoveCommonBillAsync(CommonBillModel commonbill)
        {
            if (commonbill != null && CommonBills.Contains(commonbill))
            {
                CommonBills.Remove(commonbill);
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
            for (int i = 0; i < CommonBills.Count; i++)
            {
                int number = i + 1;
                CommonBills[i].Heading = number switch
                {
                    1 => "1st Common bill",
                    2 => "2nd Common bill",
                    3 => "3rd Common bill",
                    _ => $"{number}th Common bill"
                };
            }
        }
    }
}
