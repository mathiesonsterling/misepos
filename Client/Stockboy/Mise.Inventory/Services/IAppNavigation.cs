using System.Threading.Tasks;
using System;
using Mise.Core.ValueItems;

namespace Mise.Inventory.Services
{
	public interface IAppNavigation
	{
		Pages DefaultPage{ get; }

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

		Task ShowPAR();

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

		Task ShowRestaurantLoading();

		Task ShowCreatePurchaseOrder();

		Task ShowReports();

		Task ShowRoot();

		Task ShowVendorAdd();
		Task ShowVendorFind();

		Task ShowSectionAdd();
		Task ShowSectionSelect();

		Task ShowSelectRestaurant();

		/// <summary>
		/// Show our screen to create a new placeholder restaurant
		/// </summary>
		/// <returns>The registration.</returns>
		Task ShowRestaurantRegistration();

		Task ShowUserRegistration();

		Task ShowAccountRegistration();

		Task ShowAuthorizeCreditCard ();

		Task ShowSettings();

		Task ShowChangePassword();

		/// <summary>
		/// Shows the invitations.
		/// </summary>
		/// <returns>The invitations.</returns>
		Task ShowInvitations();

	    Task ShowUpdateReceivingOrderLineItem();

        Task ShowEULA();
        Task CloseEULA();
        #region Reports

	    Task ShowSelectCompletedInventory();
	    Task ShowReportResults();
        #endregion

        Task CloseUpdateQuantity();
		Task CloseItemScan();
		Task CloseVendorAdd();
		Task CloseItemAdd();
		Task CloseItemFind ();
		Task CloseSectionAdd();

        /// <summary>
        /// Fired when an RO is done, to update our skipped screen and move us to main menu
        /// </summary>
        /// <returns></returns>
	    Task CloseReceivingOrder();

	    Task ShowUpdateParLineItem();
	}
}