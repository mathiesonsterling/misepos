using System;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using System.Threading.Tasks;
namespace Mise.Inventory.ViewModels
{
	public class RestaurantLoadingViewModel : BaseViewModel
	{
		private readonly ILoginService _loginService;
		private readonly IFunFactService _funFacts;
		public RestaurantLoadingViewModel (ILoginService loginService, IFunFactService funFacts, IAppNavigation nav, 
			ILogger logger) 
			: base(nav, logger)
		{
			_loginService = loginService;
			_funFacts = funFacts;
		}

		public string FunFact{ get { return GetValue<string> (); } private set { SetValue<string> (value); } }

		#region implemented abstract members of BaseViewModel

		public override async Task OnAppearing ()
		{
			//TODO - if we take a long time to process, load another
			FunFact = await _funFacts.GetRandomFunFact ();

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

