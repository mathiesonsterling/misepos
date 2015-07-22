using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Entities.People;
using Mise.Inventory.Services;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Mise.Core.ValueItems;
using Mise.Core.Services;


namespace Mise.Inventory.ViewModels
{
	public class InvitationViewModel : BaseViewModel
	{

		readonly ILoginService _loginService;
		public InvitationViewModel (IAppNavigation navi, ILoginService loginService, ILogger logger) : base(navi, logger)
		{
			_loginService = loginService;

			AcceptCommand = new Command<IApplicationInvitation> (Accept);
			RejectCommand = new Command<IApplicationInvitation> (Reject);
		}


		public Action LoadDataOnView{ get; set;}

		public override async Task OnAppearing(){
			var items = (await _loginService.GetInvitationsForCurrentEmployee ()).ToList();

		    if (items.Any() == false)
		    {
		        //do we have a restaurant?
		        var rest = await _loginService.GetCurrentRestaurant();
		        if (rest != null)
		        {
		            await Navigation.ShowMainMenu();
		        }
		        else
		        {
		            //go to our registration screen
		            await Navigation.ShowRestaurantRegistration();
		        }
		    }
		    else
		    {
		        InvitesForUser = items;
		    }


			if(LoadDataOnView != null){
				LoadDataOnView ();
			}
		}

		public IEnumerable<IApplicationInvitation> InvitesForUser{ get; set;}

		public EmailAddress Email{get;set;}

		public Command<IApplicationInvitation> AcceptCommand {
			get;
			private set;
		}

		public Command<IApplicationInvitation> RejectCommand {
			get;
			private set;
		}

		async void Accept (IApplicationInvitation invite)
		{
			try{
				Processing = true;
				//do we have a current employee?  If not, we need to make one!
				await _loginService.AcceptInvitation (invite);
				Processing = false;
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void Reject(IApplicationInvitation invite){
			try{
				await _loginService.RejectInvitation (invite);
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

