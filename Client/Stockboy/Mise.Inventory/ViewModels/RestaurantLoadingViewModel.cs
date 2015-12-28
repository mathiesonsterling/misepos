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
        private readonly IBeverageItemService _beverageItemService;
		public RestaurantLoadingViewModel (ILoginService loginService, IFunFactService funFacts, 
            IBeverageItemService bevService, IAppNavigation nav, ILogger logger) 
			: base(nav, logger)
		{
			_loginService = loginService;
			_funFacts = funFacts;
            _beverageItemService = bevService;
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

                //check if our selected restaurant has an account
                var hasAccount = await _loginService.DoesCurrentRestaurantHaveValidAccount();
                if(!hasAccount){
                    //see if we're over the time
                    var overTimeToRegister = await _beverageItemService.IsRestaurantOverTimeToRegister();
                    if(overTimeToRegister){
                        var res = await this.AskUserQuestionModal("Need to register account", "You need to register an account to continue using Stockboy.  You'll still get an additional free trial period!  Register an account now?",
                            "Register");
                        if(res){
                            await Navigation.ShowAccountRegistration();
                        } else {
                            await Navigation.ShowLogin();
                        }
                        return;
                    }
                }

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

