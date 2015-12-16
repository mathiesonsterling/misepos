using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Pages;
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Services.Implementation
{
    public class AppNavigation : IAppNavigation
	{
	    /// <summary>
	    /// The default page.
	    /// </summary>
	    private const Pages DEFAULT_PAGE = Pages.MainMenu;

	    public Pages DefaultPage { get { return DEFAULT_PAGE; } }

		readonly INavigationService _navi;
		readonly IPageFactory _pages;
		readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppNavigation"/> class.
		/// </summary>
		/// <param name = "logger"></param>
		/// <param name="navi">Navi.</param>
		/// <param name="pages">Pages.</param>
		public AppNavigation(ILogger logger, INavigationService navi, IPageFactory pages)
		{
			_navi = navi;
			_pages = pages;
			_logger = logger;
		}
			
		/// <summary>
		/// Loggeds the in. Assumes you are on the Login page.
		/// </summary>
		/// <returns>The in.</returns>
		public async Task ShowMainMenu()
		{
			await _navi.PopToRootAsync ();
		}

		public async Task ShowRoot()
		{
			await _navi.PopToRootAsync();
		}
			

		/// <summary>
		/// Shows the inventories.
		/// </summary>
		/// <returns>The inventories.</returns>
		public async Task ShowInventory()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.Inventory));
		}

		/// <summary>
		/// Shows the inventory find.
		/// </summary>
		/// <returns>The inventory find.</returns>
		public async Task ShowInventoryFind()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.InventoryFind));
		}

		/// <summary>
		/// Shows the items.
		/// </summary>
		/// <returns>The items.</returns>
		public async Task ShowItems()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.ItemFind));
		}

		/// <summary>
		/// Shows the login.
		/// </summary>
		/// <returns>The login.</returns>
		public async Task ShowLogin()
		{
			//blank out the restaurant name for iOS nav bar
			App.MainMenuViewModel.RestaurantName = string.Empty;
			await _navi.PushAsync(_pages.GetPage (Pages.Login));
		}

		public async Task ShowRestaurantLoading ()
		{
			await _navi.PushAsync (_pages.GetPage (Pages.RestaurantLoading));
		}

		public async Task ShowCreatePurchaseOrder()
		{
			//until we have more stuff, go to review
			await _navi.PushAsync (_pages.GetPage (Pages.PurchaseOrderReview));
		}

		public async Task ShowSelectPurchaseOrder ()
		{
			await _navi.PushAsync (_pages.GetPage (Pages.PurchaseOrderSelect));
		}

		/// <summary>
		/// Shows the vendor add.
		/// </summary>
		/// <returns>The vendor add.</returns>
		public async Task ShowVendorAdd()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.VendorAdd));
		}

		/// <summary>
		/// Shows the vendor find.
		/// </summary>
		/// <returns>The vendor find.</returns>
		public async Task ShowVendorFind()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.VendorFind));
		}

		public async Task ShowReceivingOrder()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.ReceivingOrder));
		}

		public async Task ShowPAR()
		{
			const Pages PAGE = Pages.Par;
			await _navi.PushAsync(_pages.GetPage(PAGE));
		}

		public async Task ShowStaff()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.EmployeesManage));
		}

		public Task ShowInventoryItemFind ()
		{
			return ShowItemFind (AddLineItemType.Inventory);
		}
		public Task ShowReceivingOrderItemFind ()
		{
			return ShowItemFind (AddLineItemType.ReceivingOrder);
		}	


		public Task ShowPARItemFind ()
		{
			return ShowItemFind (AddLineItemType.PAR);
		}

		

		async Task ShowItemFind(AddLineItemType type)
		{
			var vm = App.ItemFindViewModel;
			vm.CurrentType = type;

			const Pages PAGE = Pages.ItemFind;

			await _navi.PushAsync(_pages.GetPage(PAGE));

		}

		public Task ShowPARItemAdd ()
		{
			return ShowItemAdd (AddLineItemType.PAR);
		}

		public Task ShowInventoryItemAdd ()
		{
			return ShowItemAdd (AddLineItemType.Inventory);
		}

		public Task ShowReceivingOrderItemAdd ()
		{
			return ShowItemAdd (AddLineItemType.ReceivingOrder);
		}

		async Task ShowItemAdd(AddLineItemType type){
			//get the view model and set it
			//we can't inject it due to circular dependency
			var vm = App.ItemAddViewModel;
			vm.CurrentAddType = type;
			await _navi.PushAsync (_pages.GetPage (Pages.ItemAdd));
		}
			

		public async Task ShowItemScan()
		{
			await _navi.PushModalAsync(_pages.GetPage(Pages.ItemScan));
		}

		public async Task ShowSectionAdd()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.SectionAdd));
		}

		public async Task ShowSectionSelect()
		{
			const Pages PAGE = Pages.SectionSelect;
			await _navi.PushAsync(_pages.GetPage(PAGE));
		}

		public async Task ShowSelectRestaurant()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.RestaurantSelect));
		}

		public Task ShowInventoryVisuallyMeasureItem ()
		{
			return _navi.PushAsync (_pages.GetPage (Pages.InventoryVisuallyMeasure));
		}

	    public Task ShowUpdateParLineItem()
	    {
	        return _navi.PushAsync(_pages.GetPage(Pages.UpdateParLineItem));
	    }

		public async Task ShowInvitations(){
			await _navi.PushAsync (_pages.GetPage (Pages.Invitations));	
		}

	    public Task ShowUpdateReceivingOrderLineItem()
	    {
	        return _navi.PushAsync(_pages.GetPage(Pages.UpdateRecievingOrderLineItem));
	    }

        public Task ShowRestaurantRegistration ()
		{
			return _navi.PushAsync (_pages.GetPage (Pages.RegisterRestaurant));
		}

		public Task ShowAccountRegistration ()
		{
			return _navi.PushAsync (_pages.GetPage (Pages.AccountRegistration));
		}

		public Task ShowAuthorizeCreditCard ()
		{
			return _navi.PushAsync(_pages.GetPage(Pages.AuthorizeCreditCard));

		}

		public async Task ShowUserRegistration(){
			await _navi.PushAsync (_pages.GetPage (Pages.RegisterUser));
		}

		public async Task ShowSettings ()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.Settings));
		}

		public async Task ShowChangePassword ()
		{
			await _navi.PushAsync(_pages.GetPage(Pages.ChangePassword));
		}

		public async Task CloseInventoryVisuallyMeasureItem ()
		{
			await App.InventoryViewModel.OnAppearing ();
			await _navi.PopAsync ();
		}

	    public async Task CloseUpdateQuantity ()
		{
			#if __ANDROID__
				//reload the items prior to us going there
			App.PARViewModel.OnAppearing ();
			#endif
			await  _navi.PopAsync ();
		}

		public Task CloseItemScan ()
		{
			return _navi.PopModalAsync ();
		}

		public async Task CloseVendorAdd(){
			await _navi.PopAsync ();
		}

		public async Task CloseSectionAdd ()
		{
			await _navi.PopAsync();
		}

		public async Task CloseItemAdd(){
			var findPage = _navi.NavigationStack.FirstOrDefault(p => p is ItemFindPage);
			//this should have us back to find, we need one more
			if(findPage != null){
				_navi.RemovePage (findPage);
			}

			//we might need to alert the caling page we're coming back now
			await _navi.PopAsync ();
			/*
			//can we go to measurement here?
			if(App.ItemAddViewModel.CurrentAddType == AddLineItemType.Inventory){
				try{
				await ShowInventoryVisuallyMeasureItem ();
				} catch(Exception e){
					_logger.HandleException (e);
				}
			}*/
		}

		public async Task CloseItemFind(){
			await _navi.PopAsync ();
		}

	    public async Task CloseReceivingOrder()
	    {
	    	await ShowMainMenu();
	    }

        #region Reports

	    public async Task ShowReports()
	    {
	        await _navi.PushAsync(_pages.GetPage(Pages.Reports));
	    }

	    public Task ShowSelectCompletedInventory()
        {
                return _navi.PushAsync(_pages.GetPage(Pages.CompletedInventoriesSelect));
        }

        public async Task ShowReportResults()
	    {
	        await _navi.PushAsync(_pages.GetPage(Pages.ReportResults));
	    }

        public async Task ShowEULA()
        {
            await _navi.PushModalAsync(_pages.GetPage(Pages.EULA));
        }

        public async Task CloseEULA(){
            await _navi.PopModalAsync();
        }
        #endregion	
			
	}
}