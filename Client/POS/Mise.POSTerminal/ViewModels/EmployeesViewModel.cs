using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Core.Services;
using Mise.POSTerminal.Services;

namespace Mise.POSTerminal.ViewModels
{
	public class EmployeesViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="MisePOSTerminal.ViewModels.ClockInViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="model">Model.</param>
		public EmployeesViewModel(ILogger logger, ITerminalApplicationModel model, INavigationService navi) : base(logger, model)
		{
//			EnterEnabled = false;
//			InClockInMode = false;

			CurrentEmployees = new ObservableCollection<IEmployee>(model.CurrentEmployees);

			_navi = navi;

			AddCharCommand = new Command<string>((key) => {
				// Add the key to the input string.
				Passcode += key;
			});

			ClockIn = new Command((nothing) => {
				InClockInMode = true;
			});

			DeleteCharCommand = new Command((nothing) => {
				// Strip a character from the input string.
				Passcode = Passcode.Substring(0, Passcode.Length - 1);
			},
				(nothing) => Passcode.Length > 0);

			PasswordEnterCommand = new Command((nothing) => {
				//let our view model know what we did!
				var clockInSuccess = Model.EmployeeClockin(this.Passcode);
				if (clockInSuccess) {
					InClockInMode = false;
					Passcode = "";

					if (!CurrentEmployees.Contains(Model.LastSelectedEmployee)) {
						CurrentEmployees.Add(Model.LastSelectedEmployee);
					}

//					OnPropertyChanged("CurrentEmployees");
//					CurrentEmployees = new ObservableCollection<IEmployee>(model.CurrentEmployees);
//					App.MoveToPage(TerminalViewTypes.EmployeePage);
				}
			});
		}

		// ICommand implementations

		/// <summary>
		/// Gets or sets the add char command.
		/// </summary>
		/// <value>The add char command.</value>
		public ICommand AddCharCommand { protected set; get; }

		/// <summary>
		/// Gets the employee select command.
		/// </summary>
		/// <value>The employee select.</value>
		public ICommand ClockIn { get; private set; }

		/// <summary>
		/// Gets or sets the delete char command.
		/// </summary>
		/// <value>The delete char command.</value>
		public ICommand DeleteCharCommand { protected set; get; }

		/// <summary>
		/// Gets the employee select command.
		/// </summary>
		/// <value>The employee select.</value>
		public ICommand EmployeeSelect { get; private set; }

		/// <summary>
		/// Gets or sets the enter command.
		/// </summary>
		/// <value>The enter command.</value>
		public ICommand PasswordEnterCommand { protected set; get; }

		ObservableCollection<IEmployee> currentEmployees;

		/// <summary>
		/// Gets or sets the current employees.
		/// </summary>
		/// <value>The current employees.</value>
		public ObservableCollection<IEmployee> CurrentEmployees { 
			get { return currentEmployees; } 
			set { 
				currentEmployees = value;
				OnPropertyChanged("CurrentEmployees");
			}
		}

		/// <summary>
		/// The navi.
		/// </summary>
		readonly INavigationService _navi;

		/// <summary>
		/// The selected employee.
		/// </summary>
		IEmployee selectEmployee;

		/// <summary>
		/// Gets or sets the selected employee.
		/// </summary>
		/// <value>The selected employee.</value>
		public IEmployee SelectEmployee {
			get{ return selectEmployee; }
			set {
				selectEmployee = value;

				Model.SelectedEmployee = selectEmployee;

				OnPropertyChanged("SelectEmployee");
			}
		}

		bool inClockInMode = false;

		public Boolean InClockInMode {
			get {
				return inClockInMode;
			}
			set {
				inClockInMode = value;
				OnPropertyChanged("InClockInMode");
			}
		}

		bool enterEnabled = false;

		public Boolean EnterEnabled {
			get{ return enterEnabled; }
			protected set {
				enterEnabled = value;
				OnPropertyChanged("EnterEnabled");
			}
		}

		string passcode = "";

		/// <summary>
		/// Gets or sets the passcode string.
		/// </summary>
		/// <value>The passcode string.</value>
		public string Passcode {
			get { return passcode; }
			protected set {
				if (passcode != value) {
					passcode = value;
					OnPropertyChanged("Passcode");

					// Perhaps the delete button must be enabled/disabled.
					((Command)this.DeleteCharCommand).ChangeCanExecute();

					EnterEnabled = Passcode.Length > 0;
					PasscodeDisplay = "".PadLeft(Passcode.Length, '*');
				}
			}
		}

		string passcodeDisplay = "";

		/// <summary>
		/// Gets or sets the passcode string.
		/// </summary>
		/// <value>The passcode string.</value>
		public string PasscodeDisplay {
			get { return passcodeDisplay; }
			protected set {
				if (passcodeDisplay != value) {
					passcodeDisplay = value;
					OnPropertyChanged("PasscodeDisplay");
				}
			}
		}
	}
}
