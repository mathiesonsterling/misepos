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
		PAR,
		PurchaseOrderReview,
		ReceivingOrder,
		Reports,
		RestaurantSelect,
		SectionAdd,
		SectionSelect,
		VendorAdd,
		VendorFind,
		UpdateQuantity,
		RegisterUser,
		Invitations,
		PurchaseOrderSelect,
		AccountRegistration,
	}

	public interface IPageFactory
	{
		Page GetPage(Pages page);
	}
}