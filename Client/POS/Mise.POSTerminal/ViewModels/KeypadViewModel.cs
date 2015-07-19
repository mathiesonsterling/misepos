using System;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace Mise.POSTerminal.ViewModels
{
	public class KeypadViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Mise.POSTerminal.ViewModels.KeypadViewModel"/> class.
		/// </summary>
		public KeypadViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
			EnterEnabled = false;

			AddCharCommand = new Command<string>((key) => {
				// Add the key to the input string.
				InputString += key;
			});

			DeleteCharCommand = new Command((nothing) => {
				// Strip a character from the input string.
				InputString = InputString.Substring(0, InputString.Length - 1);
			},
				(nothing) => {
					// Return true if there's something to delete.
					return InputString.Length > 0;  
				});
		}

		public Boolean EnterEnabled { get; protected set; }

		string inputString = "";

		/// <summary>
		/// Gets or sets the input string.
		/// </summary>
		/// <value>The input string.</value>
		public string InputString {
			protected set {
				if (inputString != value) {
					inputString = value;
					OnPropertyChanged("InputString");

					// Perhaps the delete button must be enabled/disabled.
					((Command)this.DeleteCharCommand).ChangeCanExecute();

					EnterEnabled = InputString.Length > 0;
					OnPropertyChanged("EnterEnabled");
				}

			}

			get { return inputString; }
		}

		// ICommand implementations

		/// <summary>
		/// Gets or sets the add char command.
		/// </summary>
		/// <value>The add char command.</value>
		public ICommand AddCharCommand { protected set; get; }

		/// <summary>
		/// Gets or sets the delete char command.
		/// </summary>
		/// <value>The delete char command.</value>
		public ICommand DeleteCharCommand { protected set; get; }

		public ICommand EnterCommand { protected set; get; }

	}
}

