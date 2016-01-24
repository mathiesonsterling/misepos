using System;
using System.Threading.Tasks;
using Mise.Core.Services;
using System.Windows.Input;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Xamarin.Forms;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;

		public AboutViewModel(ILogger logger, ILoginService loginService, IAppNavigation navigationService)
			:base(navigationService, logger)
		{
			_loginService = loginService;
		}

		/// <summary>
		/// Gets the login command.
		/// </summary>
		/// <value>The login command.</value>
		public ICommand LogoutCommand {
			get { return new Command(Logout, () => NotProcessing); }
		}

		/// <summary>
		/// Login this instance.
		/// </summary>
		async void Logout()
		{
			try{
				await _loginService.LogOutAsync();

				await Navigation.ShowLogin();
			} catch(Exception e){
				HandleException (e);
			}
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }
	}
}

