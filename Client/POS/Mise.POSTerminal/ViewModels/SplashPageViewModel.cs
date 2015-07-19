
using System;
using System.Threading;
using System.Threading.Tasks;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Client.Services;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.POSTerminal.Async;
using Mise.POSTerminal.Services;
using Mise.POSTerminal.Pages;

namespace Mise.POSTerminal.ViewModels
{
	public class SplashPageViewModel : BaseViewModel
	{

		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{

		}

		#endregion

		readonly INavigationService _navi;

		/// <summary>
		/// Initializes a new instance of the <see cref="Mise.POSTerminal.ViewModels.SplashPageViewModel"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="model">Model.</param>
		public SplashPageViewModel(ILogger logger, ITerminalApplicationModel model, INavigationService navi) : base(logger, model)
		{
			_navi = navi;
			IsBusy = new NotifyTaskCompletion<int>(GoToFirstPage());
		}

		public NotifyTaskCompletion<int> IsBusy { get; private set; }

		private async Task<int> GoToFirstPage()
		{
			await Task.Delay(TimeSpan.FromSeconds(2));
			await _navi.PushAsync(new EmployeesPage());
			return 0;
		}
	}
}
