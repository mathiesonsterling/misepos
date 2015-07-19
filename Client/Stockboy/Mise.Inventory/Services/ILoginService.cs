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

		Task<bool> AddNewSectionToRestaurant (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection);
	
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
        /// Registers an account using the information of the current employee
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
		/// <param name = "code"></param>
		/// <param name = "cardName"></param>
	    Task<IAccount> RegisterAccount(CreditCard card, ReferralCode code, PersonName cardName, MiseAppTypes app);
	}
}