using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace Mise.Inventory.ViewModels
{
	public class RestaurantSelectViewModel : BaseViewModel
	{
		private readonly ILoginService _loginService;

		public IEnumerable<IRestaurant> PossibleRestaurants{ get; private set; }
		public IEnumerable<RestaurantName> PossibleRestaurantNames{ get; set;}
	
		#region commands

		#endregion

		public RestaurantSelectViewModel(ILoginService loginService, IAppNavigation appNav, ILogger logger)
			:base(appNav, logger)
		{
			_loginService = loginService;
		}

		public override async Task OnAppearing(){
			await LoadPossibleRestaurant ();
		}

		async Task LoadPossibleRestaurant()
		{
			PossibleRestaurants = await _loginService.GetPossibleRestaurantsForLoggedInEmployee();

			var names = PossibleRestaurants.Where (r => r.Name != null).Select (r => r.Name);
		    PossibleRestaurantNames = names;
		}

		public async Task SelectRestaurant(RestaurantName name)
		{
			Processing = true;
			try{
				var rest = PossibleRestaurants.FirstOrDefault (r => r.Name != null && r.Name.Equals (name));
				await _loginService.SelectRestaurantForLoggedInEmployee(rest.ID);
				Processing = false;
				//now go to main menu
				await Navigation.ShowMainMenu();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

