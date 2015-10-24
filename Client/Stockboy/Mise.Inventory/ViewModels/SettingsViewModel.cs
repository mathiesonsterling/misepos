using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;

using Xamarin.Forms;
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

		public ICommand ChangePasswordCommand{ get { return new Command (ChangePassword, () => NotProcessing); } }

		public async void ChangePassword(){
		}
	}
}

