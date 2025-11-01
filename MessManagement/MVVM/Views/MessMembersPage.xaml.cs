using MessManagement.MVVM.ViewModels;
using MessManagement.Services;
using UraniumUI.Pages;

namespace MessManagement.MVVM.Views;
[QueryProperty(nameof(MessId), "messId")]
public partial class MessMembersPage : ContentPage
{
    private readonly UserSessionService _userSession;
    private int _messId;
    public int MessId
    {
        get => _messId;
        set
        {
            _messId = value;
            var lastUrl = Preferences.Get("MessDetailsTabBarUrl", string.Empty);
            bool navigatedFromShell = lastUrl.Contains("?");
            if (_messId > 0 && navigatedFromShell)
            {
                _ = LoadMembers(_messId);
            }
            else
            {
                _messId = _userSession.CurrentUser.CurrentMessId??0;
                if (_messId > 0)
                {
                    _ = LoadMembers(_messId);
                }   
            }
            Preferences.Set("CurrentMessId", _messId);

        }
    }
    public MessMembersPage(MessMembersViewModel vm, UserSessionService userSessionService)
	{
		InitializeComponent();
        BindingContext = vm;
        _userSession = userSessionService;
    }
    private async Task LoadMembers(int messId)
    {
        if (BindingContext is MessMembersViewModel vm)
        {
            if (messId > 0)
            {
                await vm.LoadMessMemberSummaryCommand.ExecuteAsync(messId);
            }
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMembers(MessId);
        //if (BindingContext is MessMembersViewModel vm)
        //{
        //    // Get the current MessId from Preferences
        //    int messId = MessId; //Preferences.Get("CurrentMessId", 0);
        //    if (messId > 0)
        //    {
        //        // Reload meals every time the page appears
        //        await vm.LoadMessMemberSummaryCommand.ExecuteAsync(messId);
        //        // Optionally select the first member automatically
        //        //if (vm.Members.Any())
        //        //    await vm.SelectMemberCommand.ExecuteAsync(vm.Members.First());
        //    }
        //}
    }  
}