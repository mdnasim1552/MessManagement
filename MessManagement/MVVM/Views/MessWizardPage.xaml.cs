using InputKit.Shared.Controls;
using MessManagement.MVVM.ViewModels;
using UraniumUI.Pages;

namespace MessManagement.MVVM.Views;

public partial class MessWizardPage : UraniumContentPage
{
	public MessWizardPage(MessWizardViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        vm.MemberAdded += async member =>
        {
            await MemberCollection.Dispatcher.DispatchAsync(() =>
            {
                if (vm.Members.Count > 0)
                {
                    MemberCollection.ScrollTo(
                        item: member,           // scroll by item
                        position: ScrollToPosition.End,
                        animate: true
                    );
                }
            });
        };
        vm.CommonBillAdded += async commonBill =>
        {
            await CommonBillCollection.Dispatcher.DispatchAsync(() =>
            {
                if (vm.Members.Count > 0)
                {
                    CommonBillCollection.ScrollTo(
                        item: commonBill,           // scroll by item
                        position: ScrollToPosition.End,
                        animate: true
                    );
                }
            });
        };
    }

    private void MessFormSubmit_Clicked(object sender, EventArgs e)
    {
        CreateMessForm.Submit();
    }
}