using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using Mise.Core.Entities.People;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Xamarin.Forms;
using Mise.Core.ValueItems;


namespace Mise.Inventory.ViewModels
{
	public class InvitationDisplayModel : BaseDisplayLine<IApplicationInvitation>
	{
		public InvitationDisplayModel(IApplicationInvitation source) : base(source){
		}

		#region implemented abstract members of BaseDisplayLine
		public override string DisplayName {
			get {
				return Source.RestaurantName != null 
					? Source.RestaurantName.FullName
						: "Error Unfound Restaurant";
			}
		}

		#region implemented abstract members of BaseDisplayLine

		public override string DetailDisplay {
			get {
				return Source.InvitingEmployeeName != null
					? Source.InvitingEmployeeName.ToSingleString()
						: string.Empty;
			}
		}

		#endregion
		#endregion
	}

	public class InvitationViewModel : BaseSearchableViewModel<InvitationDisplayModel>
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
			await NavigateBasedOnLineItems ();
	    }

	    protected override async Task<ICollection<InvitationDisplayModel>> LoadItems()
	    {
            var items = await _loginService.GetInvitationsForCurrentEmployee();
			return items.Select(i => new InvitationDisplayModel(i)).ToList();
	    }

	    protected override void AfterSearchDone()
	    {
	    }

        public override Task SelectLineItem(InvitationDisplayModel lineItem)
        {
            _selectedInvitation = lineItem.Source;
            HasSelection = lineItem.Source != null;
            return Task.FromResult(true);
        }

		public EmailAddress Email{get;set;}

		public ICommand AcceptCommand => new Command(Accept);

	    public ICommand RejectCommand => new Command(Reject);

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

				//if we have more invitations, stay here.  Otherwise pop back one
				await DoSearch();
				await NavigateBasedOnLineItems();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async Task NavigateBasedOnLineItems ()
		{
			if (LineItems.Any () == false) {
				//do we have a restaurant?
				var rest = await _loginService.GetCurrentRestaurant ();
				if (rest != null) {
					await Navigation.ShowMainMenu ();
				}
				else {
					//go to our registration screen
					await Navigation.ShowRestaurantRegistration ();
				}
			}
		}
	}
}

