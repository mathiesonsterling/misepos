using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
namespace Mise.Inventory.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		public SettingsViewModel(IAppNavigation nav, ILogger logger) : base(nav, logger)
		{
		}

		#region implemented abstract members of BaseViewModel
		public override Task OnAppearing ()
		{
			return Task.FromResult (false);
		}
		#endregion
	}
}

