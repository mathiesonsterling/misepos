using Xamarin.Forms;

namespace Mise.Inventory.Services
{
	public enum Pages
	{

		RegisterRestaurant,

		EmployeesManage,
		Inventory,
		InventoryFind,
		InventoryVisuallyMeasure,
		ItemAdd,
		ItemFind,
		ItemScan,
		Login,
		MainMenu,
		Par,
		PurchaseOrderReview,
		ReceivingOrder,
		RestaurantSelect,
		SectionAdd,
		SectionSelect,
		VendorAdd,
		VendorFind,
        UpdateRecievingOrderLineItem,
        UpdateParLineItem,
		RegisterUser,
		Invitations,
		PurchaseOrderSelect,
		AccountRegistration,
		AuthorizeCreditCard,
        Reports,
	    CompletedInventoriesSelect,
	    ReportResults,
		RestaurantLoading,
		Settings,
		ChangePassword
	}

	public interface IPageFactory
	{
		Page GetPage(Pages page);
	}
}