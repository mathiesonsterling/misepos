using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.POSTerminal.Services;
using Mise.POSTerminal.Pages;

namespace Mise.POSTerminal.ViewModels
{
	public class ClockInViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		readonly INavigationService _navi;

		/// <summary>
		/// Initializes a new instance of the <see cref="MisePOSTerminal.ViewModels.ClockInViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="model">Model.</param>
		public ClockInViewModel(ILogger logger, ITerminalApplicationModel model, INavigationService navi) : base(logger, model)
		{
			_navi = navi;
		}

		public void ClockIn(string passCode)
		{
			var clockInSuccess = Model.EmployeeClockin(passCode);
			if (clockInSuccess) {
				_navi.PushAsync(new EmployeesPage());
			}
		}
	}
}

