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

            if (!PossibleRestaurants.Any())
            {
                //TODO log this!
                Logger.Error("No restaurants found in Select Restaurant!");
                var userConf = await AskUserQuestionModal ("No Restaurant Found", 
                    "We don't show any restaurants assigned to this user.  This could be a server error.  Do you want to register a new restaurant?", "Register");
                if (userConf)
                {
                    await Navigation.ShowRestaurantRegistration();
                }
                else
                {
                    await _loginService.LogOutAsync();
                    await Navigation.ShowLogin();
                }
            }
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
				await _loginService.SelectRestaurantForLoggedInEmployee(rest.Id);
				Processing = false;
				//now go to main menu
				await Navigation.ShowRestaurantLoading ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

