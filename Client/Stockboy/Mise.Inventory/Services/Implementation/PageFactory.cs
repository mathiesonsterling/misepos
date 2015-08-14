using System;

using Xamarin.Forms;

using Mise.Inventory.Pages;
using Mise.Inventory.Pages.Reports;
namespace Mise.Inventory.Services
{
    public class PageFactory : IPageFactory
    {
        public Page GetPage(Pages page)
        {
            switch (page)
            {
                case Pages.EmployeesManage:
                    return new EmployeesManagePage();
                case Pages.Inventory:
                    return new InventoryPage();
                case Pages.InventoryVisuallyMeasure:
                    return new InventoryVisuallyMeasureWithGesturesPage();
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
                case Pages.Par:
                    return new PARPage();
                case Pages.UpdateParLineItem:
                    return new UpdateParLineItemPage();
                case Pages.PurchaseOrderReview:
                    return new PurchaseOrderReviewPage();
                case Pages.ReceivingOrder:
                    return new ReceivingOrderPage();
                case Pages.UpdateRecievingOrderLineItem:
                    return new UpdateReceivingOrderLineItem();
                case Pages.RestaurantSelect:
                    return new RestaurantSelectPage();
                case Pages.SectionAdd:
                    return new SectionAddPage();
                case Pages.SectionSelect:
                    return new SectionSelectPage();
                case Pages.VendorAdd:
                    return new VendorAddPage();
                case Pages.VendorFind:
                    return new VendorFindPage();
                case Pages.RegisterUser:
                    return new UserRegistrationPage();
                case Pages.RegisterRestaurant:
                    return new RegisterRestaurantPage();
                case Pages.Invitations:
                    return new InvitationsPage();
                case Pages.PurchaseOrderSelect:
                    return new PurchaseOrderSelectPage();
                case Pages.AccountRegistration:
                    return new AccountRegistrationPage();
				case Pages.AuthorizeCreditCard:
					return new AuthorizeCreditCardPage ();
                case Pages.Reports:
                    return new ReportsPage();
                case Pages.CompletedInventoriesSelect:
                    return new SelectCompletedInventoriesPage();
                case Pages.ReportResults:
                    return new ReportResultsPage();
                default:
                    throw new ArgumentException(string.Format("Unknown page type {0}", page));
            }
        }

        /*	
        Staff,
        ReceivingOrder*/
    }
}