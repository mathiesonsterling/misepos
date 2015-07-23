using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;
using System;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Services
{
	/// <summary>
	/// Holds state of selections, employee and restaurant operations
	/// </summary>
	public interface ILoginService
	{
		void OnAppStarting ();

		Task<IEmployee> LoginAsync(EmailAddress email, Password password);

		Task<IEnumerable<IRestaurant>> GetPossibleRestaurantsForLoggedInEmployee();

		Task SelectRestaurantForLoggedInEmployee (Guid restaurantID);

		Task LogOutAsync();

		/// <summary>
		/// Gets the employee currently logged in
		/// </summary>
		/// <returns>The current employee.</returns>
		Task<IEmployee> GetCurrentEmployee ();

		Task<IRestaurant> GetCurrentRestaurant();

		Task<IRestaurantInventorySection> GetCurrentSection ();
		Task SelectSection (IRestaurantInventorySection section);

		Task AddNewSectionToRestaurant (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection);
	
		Task<IEmployee> RegisterEmployee (EmailAddress email, Password password, PersonName name);
		Task InviteEmployeeToUseApp (EmailAddress destEmail);
		Task AcceptInvitation (IApplicationInvitation invite);
		Task RejectInvitation (IApplicationInvitation invite);
		Task<IEnumerable<IApplicationInvitation>> GetInvitationsForCurrentEmployee ();
		Task<IEnumerable<IApplicationInvitation>> GetPendingInvitationsForRestaurant(Guid restaurantID);

		/// <summary>
		/// Makes a single employee restaurant for a user that registers
		/// </summary>
		/// <returns>The placeholder restaurant for current employee.</returns>
		Task CreatePlaceholderRestaurantForCurrentEmployee();

	    Task<IRestaurant> RegisterRestaurant(RestaurantName name, StreetAddress address, PhoneNumber phone);

		/// <summary>
		/// Fired when a user registers a restaurant, but doesn't create a restaurant
		/// </summary>
		/// <returns>The restaurant registration without account.</returns>
		Task CommitRestaurantRegistrationWithoutAccount();

        /// <summary>
        /// Primes our service to register an account - must be split into two so we can then pass it off to mercury!
        /// </summary>
        /// <returns></returns>
		/// <param name = "email"></param>
		/// <param name = "code"></param>
		/// <param name = "accountName"></param>
		/// <param name = "app"></param>
		Task StartRegisterAccount(EmailAddress email, ReferralCode code, PersonName accountName, MiseAppTypes app);

		/// <summary>
		/// Finishes registering the account, once we've got the card info ready!
		/// </summary>
		/// <returns>The register account.</returns>
		/// <param name="card">Card.</param>
		Task<IAccount> CompleteRegisterAccount(CreditCard card);
	}
}