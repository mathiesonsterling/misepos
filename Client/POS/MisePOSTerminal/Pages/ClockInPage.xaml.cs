using System;

using Xamarin.Forms;

using MisePOSTerminal.ViewModels;

namespace MisePOSTerminal.Pages
{
	public partial class ClockInPage : ContentPage
	{
		readonly ClockInViewModel _vm;

		public ClockInPage()
		{
			InitializeComponent();

			_vm = new ClockInViewModel(App.Logger, App.AppModel);
			BindingContext = _vm;
		}

		// TODO: I suppose I can add this to some event trigger
		// something or other in the VM, but I don't see much difference in it.
		protected void OnClockInOut(object sender, EventArgs args)
		{
			FocusPasscodeEntry();
		}

		private void FocusPasscodeEntry()
		{
			PasscodeEntry.Focus();
		}

	}
}

