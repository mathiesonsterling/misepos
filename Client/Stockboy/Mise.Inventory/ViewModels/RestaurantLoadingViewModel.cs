using System;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using System.Threading.Tasks;
namespace Mise.Inventory.ViewModels
{
	public class RestaurantLoadingViewModel : BaseViewModel
	{
		private readonly ILoginService _loginService;
		public RestaurantLoadingViewModel (ILoginService loginService, IAppNavigation nav, ILogger logger) 
			: base(nav, logger)
		{
			_loginService = loginService;
		}

		#region implemented abstract members of BaseViewModel

		public override async Task OnAppearing ()
		{
			try{
				Processing = true;
				await _loginService.LoadSelectedRestaurant ();
				Processing = false;
				await Navigation.ShowMainMenu ();
				return;
			} catch(Exception e){
				HandleException (e);
				//navigate back to where?
			}
			await Navigation.ShowLogin ();
		}

		#endregion
	}
}

