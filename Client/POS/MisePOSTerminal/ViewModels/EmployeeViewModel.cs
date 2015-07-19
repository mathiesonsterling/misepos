using System;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.ValueItems;
namespace MisePOSTerminal.ViewModels
{
	public class EmployeeViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped (CreditCard card)
		{

		}

		#endregion

		/// <summary>
		/// Gets the cancel command
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public ICommand Cancel { get; private set; }

		/// <summary>
		/// Gets the display name of the SelectedEmployee.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get { return Model.SelectedEmployee.DisplayName; } }

		/// <summary>
		/// Gets the open checks of our IEmployee.
		/// </summary>
		/// <value>The open checks.</value>
		public IEnumerable<ICheck> OpenChecks {
			get { return Model.OpenChecks; }
		}

		private ICheck _selectCheck;

		/// <summary>
		/// Gets or sets the selected employee.
		/// </summary>
		/// <value>The selected employee.</value>
		public ICheck SelectCheck {
			get{ return _selectCheck; }
			set {
				_selectCheck = value;

				OnPropertyChanged("SelectCheck");

				App.MoveToPage(TerminalViewTypes.OrderOnCheck);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MisePOSTerminal.ViewModels.EmployeeViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="model">Model.</param>
		public EmployeeViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
			Cancel = new Command (() => MoveToView (TerminalViewTypes.ClockInPage));
		}
	}
}

