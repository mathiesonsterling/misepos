using System;

using Xamarin.Forms;

using Mise.Inventory.Pages;

namespace Mise.Inventory.Services
{
	public class PageFactory : IPageFactory
	{
		public Page GetPage(Pages page)
		{
			switch (page) {
			case Pages.EmployeesManage:
				return new EmployeesManagePage();
			case Pages.Inventory:
				return new InventoryPage();
			case Pages.InventoryVisuallyMeasure:
				return new InventoryVisuallyMesasuredImprovedPage();
				//return new Mise.Inventory.Pages.InventoryVisuallyMeasurePageNew();
			case Pages.ItemAdd:
				return new ItemAddPage();
			case Pages.ItemFind:
				return new ItemFindPage();
			case Pages.ItemScan:
				return new ItemScanPage();
			case Pages.Login:
				return new LoginPage();
			case Pages.MainMenu:
				return new MainMenuPage();
			case Pages.PAR:
				return new PARPage();
			case Pages.PurchaseOrderReview:
				return new PurchaseOrderReviewPage ();
			case Pages.ReceivingOrder:
				return new ReceivingOrderPage();
			case Pages.Reports:
				return new ReportsPage();
			case Pages.RestaurantSelect:
				return new RestaurantSelectPage();
			case Pages.SectionAdd:
				return new SectionAddPage();
			case Pages.SectionSelect:
				return new SectionSelectPage();
			case Pages.UpdateQuantity:
				return new UpdateQuantityPage ();
			case Pages.VendorAdd:
				return new VendorAddPage();
			case Pages.VendorFind:
				return new VendorFindPage();
			case Pages.RegisterUser:
				return new UserRegistrationPage ();
			case Pages.RegisterRestaurant:
				return new RegisterRestaurantPage ();
			case Pages.Invitations:
				return new InvitationsPage ();
			case Pages.PurchaseOrderSelect:
				return new PurchaseOrderSelectPage ();
			case Pages.AccountRegistration:
				return new AccountRegistrationPage ();
			default:
				//TODO this needs xamarin insights and an error page!
				throw new ArgumentException(string.Format("Unknown page type {0}", page));
			}
		}

		/*	
		Staff,
		ReceivingOrder*/
	}
}