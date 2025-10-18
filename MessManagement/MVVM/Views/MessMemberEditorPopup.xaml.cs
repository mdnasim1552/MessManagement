using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using MessManagement.MVVM.ViewModels;
using MessManagement.Shared.DTOs;

namespace MessManagement.MVVM.Views;

public partial class MessMemberEditorPopup : Popup
{
	public MessMemberEditorPopup(MessMemberSummaryDto member)
	{
        InitializeComponent();
        var vm = App.Current.Handler.MauiContext.Services.GetService<MessMemberEditorViewModel>();
        vm.Initialize(member);
        BindingContext = vm;
        vm.Popup = this;
    }
}