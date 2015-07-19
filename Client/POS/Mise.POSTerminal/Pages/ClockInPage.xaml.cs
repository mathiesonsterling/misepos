using Xamarin.Forms;

using Mise.POSTerminal.ViewModels;

namespace Mise.POSTerminal.Pages
{
	public partial class ClockInPage : ContentPage
	{
		readonly ClockInViewModel _vm;

		void OnKeypadEnterClicked(object sender, Mise.POSTerminal.Controls.Keypad.EnterEventArgs e)
		{
			_vm.ClockIn(e.Input);
		}

		public ClockInPage()
		{
			InitializeComponent();

			_vm = new ClockInViewModel(App.Logger, App.AppModel, App.Navigation);
			BindingContext = _vm;

			Keypad.EnterClicked += OnKeypadEnterClicked;
		}

	}
}

