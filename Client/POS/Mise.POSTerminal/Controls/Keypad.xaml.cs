using System;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.POSTerminal.ViewModels;

namespace Mise.POSTerminal.Controls
{
	public partial class Keypad : ContentView
	{
		readonly KeypadViewModel _vm;

		public class EnterEventArgs : EventArgs
		{
			public string Input { get; set; }
		}

		public event EventHandler<EnterEventArgs> EnterClicked;

		private void OnEnterClicked(object sender, EventArgs e)
		{
			if (EnterClicked != null) {
				EnterClicked(this, new EnterEventArgs {
					Input = _vm.InputString
				});

				if (_vm.EnterCommand != null) {
					_vm.EnterCommand.Execute(_vm.InputString);
				}
			}
		}

		public Keypad()
		{ 
			InitializeComponent();

			_vm = new KeypadViewModel(App.Logger, App.AppModel);
			BindingContext = _vm;
		}
	}
}

