using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MisePOSTerminal.ViewModels
{
	public class ClockInViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		/// <summary>
		/// Gets the clock in command.
		/// </summary>
		/// <value>The clock in.</value>
		public ICommand ClockIn{ get; private set; }

		/// <summary>
		/// Gets the clock out command.
		/// </summary>
		/// <value>The clock out.</value>
		public ICommand ClockOut{ get; private set; }

		/// <summary>
		/// Gets the clock in/out command.
		/// </summary>
		/// <value>The clock in out.</value>
		public ICommand ClockInOut{ get; private set; }

		/// <summary>
		/// Gets the cancel clock in/out command
		/// </summary>
		/// <value><c>true</c> if this instance cancel clock in out; otherwise, <c>false</c>.</value>
		public ICommand CancelClockInOut{ get; private set; }

		/// <summary>
		/// Gets the employee select command.
		/// </summary>
		/// <value>The employee select.</value>
		public ICommand EmployeeSelect{ get; private set; }

		/// <summary>
		/// Gets the current employees.
		/// </summary>
		/// <value>The current employees.</value>
		public IEnumerable<IEmployee> CurrentEmployees {
			get { return Model.CurrentEmployees; }
		}

		private bool _inPasscodeMode = false;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="MisePOSTerminal.ViewModels.ClockInViewModel"/> in passcode mode.
		/// </summary>
		/// <value><c>true</c> if in passcode mode; otherwise, <c>false</c>.</value>
		public bool InPasscodeMode {
			get { return _inPasscodeMode; }
			set {
				if (_inPasscodeMode == value) {
					return;
				}

				_inPasscodeMode = value;

				if (_inPasscodeMode == false) {
					Passcode = "";
				}

				OnPropertyChanged("InPasscodeMode");
			}
		}

		private string _passcode = "";

		/// <summary>
		/// Gets or sets the passcode.
		/// </summary>
		/// <value>The passcode.</value>
		public string Passcode {
			get { return _passcode; }
			set {
				if (_passcode == value) {
					return;
				}
				_passcode = value;
				OnPropertyChanged("Passcode");
				OnPropertyChanged("CanLogInOut");
			}
		}

		/// <summary>
		/// Gets the validation errors.
		/// </summary>
		/// <value>The validation errors.</value>
		public string ValidationErrors { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance can login.
		/// </summary>
		/// <value><c>true</c> if this instance can login; otherwise, <c>false</c>.</value>
		public bool CanLogInOut {
			get {
				ValidationErrors = string.Empty;

				if (string.IsNullOrEmpty(Passcode)) {
					ValidationErrors += "Please enter a passcode.";
				}

				return string.IsNullOrEmpty(ValidationErrors);
			}
		}

		private IEmployee _selectEmployee;

		/// <summary>
		/// Gets or sets the selected employee.
		/// </summary>
		/// <value>The selected employee.</value>
		public IEmployee SelectEmployee {
			get{ return _selectEmployee; }
			set {
				_selectEmployee = value;

				Model.SelectedEmployee = _selectEmployee;

				OnPropertyChanged("SelectEmployee");

				App.MoveToPage(TerminalViewTypes.EmployeePage);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MisePOSTerminal.ViewModels.ClockInViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="model">Model.</param>
		public ClockInViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
			Passcode = "";

			ClockIn = new Command(() => {
				//let our view model know what we did!
				var clockInSuccess = Model.EmployeeClockin(this.Passcode);
				if (clockInSuccess) {
					OnPropertyChanged("CurrentEmployees");
					InPasscodeMode = false;

					App.MoveToPage(TerminalViewTypes.EmployeePage);
				}
			});

			ClockOut = new Command(eh => {
				//let our view model know what we did!
				var clockOutSuccess = Model.EmployeeClockout(this.Passcode, Model.SelectedEmployee);
				if (clockOutSuccess) {
					OnPropertyChanged("CurrentEmployees");
					InPasscodeMode = false;
				}
			});

			ClockInOut = new Command(() => {
				InPasscodeMode = true;
			});

			CancelClockInOut = new Command(() => {
				InPasscodeMode = false;
			});

//			TODO: CB: Make it so we switch to the logged in employees view if there is only one logged in user.
		}
	}
}

