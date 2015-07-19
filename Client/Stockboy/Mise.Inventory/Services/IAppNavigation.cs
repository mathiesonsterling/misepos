using System.Threading.Tasks;
using System;
using Mise.Core.ValueItems;

namespace Mise.Inventory.Services
{
	public interface IAppNavigation
	{
        Task DisplayAlert(
            string title,
            string message,
            string accept = "OK"
        );

		Task<bool> AskUser (string title, string message, string accept = "OK", string cancel = "Cancel");

		Pages DefaultPage{ get; }

		/// <summary>
		/// Allows view models to pass exceptions for possible display
		/// </summary>
		/// <param name="e">E.</param>
		void HandleException (Exception e);

		Task ShowMainMenu();

		Task ShowSelectPurchaseOrder();
		Task ShowReceivingOrder();

		Task ShowInventory();

		Task ShowInventoryFind();
		Task ShowInventoryVisuallyMeasureItem();
		/// <summary>
		/// Show we're done measuring the item, and want to go back
		/// </summary>
		/// <returns>The inventory visually measure item.</returns>
		Task CloseInventoryVisuallyMeasureItem();

		Task ShowPAR(bool replaceCurrentPage = false);

		/// <summary>
		/// Invites page
		/// </summary>
		Task ShowStaff();

		//all item adds go to the same screen, but with different params
		Task ShowInventoryItemAdd();
		Task ShowReceivingOrderItemAdd();
		Task ShowPARItemAdd();

		Task ShowInventoryItemFind ();
		Task ShowReceivingOrderItemFind();
		Task ShowPARItemFind();

		Task ShowItemScan();

		Task ShowLogin();

		Task ShowCreatePurchaseOrder();

		Task ShowReports();

		Task ShowRoot();

		Task ShowVendorAdd();
		Task ShowVendorFind();

		Task ShowSectionAdd();
		Task ShowSectionSelect(bool replaceCurrentPage = false);

		Task ShowSelectRestaurant();

		/// <summary>
		/// Show our screen to create a new placeholder restaurant
		/// </summary>
		/// <returns>The registration.</returns>
		Task ShowRestaurantRegistration();

		Task ShowUserRegistration();

		Task ShowAccountRegistration();

		/// <summary>
		/// Shows the invitations.
		/// </summary>
		/// <returns>The invitations.</returns>
		Task ShowInvitations();

		Task ShowUpdateQuantity(int quantity, string itemName, Action<int, decimal> updateQuantCallback, Action zeroOutCallback,
			Money currentPrice, bool addPrices = false, string title = "Update Quantity");
		Task CloseUpdateQuantity();
		Task CloseItemScan();
		Task CloseVendorAdd();
		Task CloseItemAdd();
		Task CloseItemFind ();

        /// <summary>
        /// Fired when an RO is done, to update our skipped screen and move us to main menu
        /// </summary>
        /// <returns></returns>
	    Task CloseReceivingOrder();

	}
}