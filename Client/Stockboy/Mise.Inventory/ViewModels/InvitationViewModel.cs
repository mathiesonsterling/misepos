using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Entities.People;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Xamarin.Forms;
using Mise.Core.ValueItems;


namespace Mise.Inventory.ViewModels
{
	public class InvitationViewModel : BaseSearchableViewModel<IApplicationInvitation>
	{

		readonly ILoginService _loginService;
	    private IApplicationInvitation _selectedInvitation;
		public InvitationViewModel (IAppNavigation navi, ILoginService loginService, ILogger logger) : base(navi, logger)
		{
			_loginService = loginService;
		}

        public bool HasSelection { get { return GetValue<bool>(); }private set { SetValue(value);} }

	    public override async Task OnAppearing()
	    {
	        _selectedInvitation = null;
	        HasSelection = false;
	        await base.OnAppearing();
	        if (LineItems.Any() == false)
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
	    }

	    protected override async Task<ICollection<IApplicationInvitation>> LoadItems()
	    {
            var items = (await _loginService.GetInvitationsForCurrentEmployee()).ToList();
	        return items;
	    }

	    protected override void AfterSearchDone()
	    {
	    }

        public override Task SelectLineItem(IApplicationInvitation lineItem)
        {
            _selectedInvitation = lineItem;
            HasSelection = lineItem != null;
            return Task.FromResult(true);
        }

        public IEnumerable<IApplicationInvitation> InvitesForUser{ get; set;}

		public EmailAddress Email{get;set;}

		public Command AcceptCommand => new Command(Accept);

	    public Command RejectCommand => new Command(Reject);

	    async void Accept ()
		{
			try{
				Processing = true;
				//do we have a current employee?  If not, we need to make one!
				await _loginService.AcceptInvitation (_selectedInvitation);
				Processing = false;
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void Reject(){
			try
			{
			    Processing = true;
				await _loginService.RejectInvitation (_selectedInvitation);
			    Processing = false;
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

