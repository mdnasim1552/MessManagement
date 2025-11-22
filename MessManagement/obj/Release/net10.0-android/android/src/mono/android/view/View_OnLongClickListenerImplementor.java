package mono.android.view;


public class View_OnLongClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.view.View.OnLongClickListener
{

	public View_OnLongClickListenerImplementor ()
	{
		super ();
		if (getClass () == View_OnLongClickListenerImplementor.class) {
			mono.android.TypeManager.Activate ("Android.Views.View+IOnLongClickListenerImplementor, Mono.Android", "", this, new java.lang.Object[] {  });
		}
	}

	public boolean onLongClick (android.view.View p0)
	{
		return n_onLongClick (p0);
	}

	private native boolean n_onLongClick (android.view.View p0);

	public boolean onLongClickUseDefaultHapticFeedback (android.view.View p0)
	{
		return n_onLongClickUseDefaultHapticFeedback (p0);
	}

	private native boolean n_onLongClickUseDefaultHapticFeedback (android.view.View p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
