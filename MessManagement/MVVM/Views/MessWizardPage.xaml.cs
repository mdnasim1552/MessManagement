using MessManagement.MVVM.ViewModels;

namespace MessManagement.MVVM.Views;

public partial class MessWizardPage : ContentPage
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
}