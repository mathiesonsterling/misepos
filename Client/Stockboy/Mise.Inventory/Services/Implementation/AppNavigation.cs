using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Services;
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

	    public Task DisplayAlert(string title, string message, string accept = "OK")
	    {
	        return _navi.DisplayAlert(title, message, accept);
	    }

		public Task<bool> AskUser (string title, string message, string accept = "OK", string cancel = "Cancel")
		{
			return _navi.AskUser (title, message, accept, cancel);
		}

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

		public async Task ReplacePage(Pages page)
		{
			try{
				var currentPage = _navi.CurrentPage;
				await _navi.PushAsync(_pages.GetPage(page));
				_navi.RemovePage(currentPage);
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Loggeds the in. Assumes you are on the Login page.
		/// </summary>
		/// <returns>The in.</returns>
		public async Task ShowMainMenu()
		{
			try{
				await _navi.PopToRootAsync ();
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowRoot()
		{
			try{
				await _navi.PopToRootAsync();
			} catch(Exception e){
				HandleException(e);
			}
		}
			

		/// <summary>
		/// Shows the inventories.
		/// </summary>
		/// <returns>The inventories.</returns>
		public async Task ShowInventory()
		{
			try{
			await _navi.PushAsync(_pages.GetPage(Pages.Inventory));
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Shows the inventory find.
		/// </summary>
		/// <returns>The inventory find.</returns>
		public async Task ShowInventoryFind()
		{
			try{
			await _navi.PushAsync(_pages.GetPage(Pages.InventoryFind));
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Shows the items.
		/// </summary>
		/// <returns>The items.</returns>
		public async Task ShowItems()
		{
			try{
			await _navi.PushAsync(_pages.GetPage(Pages.ItemFind));
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Shows the login.
		/// </summary>
		/// <returns>The login.</returns>
		public async Task ShowLogin()
		{
			try{
				await _navi.PushAsync(_pages.GetPage (Pages.Login));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowCreatePurchaseOrder()
		{
			try{
			//until we have more stuff, go to review
			await _navi.PushAsync (_pages.GetPage (Pages.PurchaseOrderReview));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowSelectPurchaseOrder ()
		{
			try{
				await _navi.PushAsync (_pages.GetPage (Pages.PurchaseOrderSelect));
			} catch(Exception e){
				HandleException (e);
			}
		}

		/// <summary>
		/// Shows the vendor add.
		/// </summary>
		/// <returns>The vendor add.</returns>
		public async Task ShowVendorAdd()
		{
			try{
			await _navi.PushAsync(_pages.GetPage(Pages.VendorAdd));
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Shows the vendor find.
		/// </summary>
		/// <returns>The vendor find.</returns>
		public async Task ShowVendorFind()
		{
			try{
			await _navi.PushAsync(_pages.GetPage(Pages.VendorFind));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowReceivingOrder()
		{
			try{
				await _navi.PushAsync(_pages.GetPage(Pages.ReceivingOrder));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowPAR(bool replaceCurrentPage = false)
		{
			try
			{
			    const Pages PAGE = Pages.Par;

			    if (replaceCurrentPage) {
					await ReplacePage(PAGE);
				} else {
					await _navi.PushAsync(_pages.GetPage(PAGE));
				}
			}
			catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowStaff()
		{
			try{
				await _navi.PushAsync(_pages.GetPage(Pages.EmployeesManage));
			} catch(Exception e){
				HandleException(e);
			}
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

		

		async Task ShowItemFind(AddLineItemType type, bool replaceCurrentPage = false)
		{
			try{
				var vm = App.ItemFindViewModel;
				vm.CurrentType = type;

				const Pages PAGE = Pages.ItemFind;

				if (replaceCurrentPage) {
					await ReplacePage(PAGE);
				} else {
					await _navi.PushAsync(_pages.GetPage(PAGE));
				}
			} catch(Exception e){
				HandleException(e);
			}
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
			try{
				//get the view model and set it
				//we can't inject it due to circular dependency
				var vm = App.ItemAddViewModel;
				vm.CurrentAddType = type;
				await _navi.PushAsync (_pages.GetPage (Pages.ItemAdd));
			} catch(Exception e){
				HandleException(e);
			}
		}
			

		public async Task ShowItemScan()
		{
			try{
				await _navi.PushModalAsync(_pages.GetPage(Pages.ItemScan));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowSectionAdd()
		{
			try{
				await _navi.PushAsync(_pages.GetPage(Pages.SectionAdd));
			} catch(Exception e){
				 HandleException(e);
			}
		}

		public async Task ShowSectionSelect(bool replaceCurrentPage = false)
		{
			try
			{
			    const Pages PAGE = Pages.SectionSelect;

			    if (replaceCurrentPage) {
				await ReplacePage(PAGE);
			} else {
				await _navi.PushAsync(_pages.GetPage(PAGE));
			}
			}
			catch(Exception e){
				HandleException(e);
			}
		}

		public async Task ShowSelectRestaurant()
		{
			try{
				if(_navi.CurrentPage is LoginPage){
					await _navi.PopAsync ();
				}
				await _navi.PushAsync(_pages.GetPage(Pages.RestaurantSelect));
			} catch(Exception e){
				HandleException(e);
			}
		}

		public Task ShowInventoryVisuallyMeasureItem ()
		{
			try{
				return _navi.PushAsync (_pages.GetPage (Pages.InventoryVisuallyMeasure));
			} catch(Exception e){
				HandleException(e);
				return Task.FromResult (false);
			}
		}

	    public Task ShowUpdateParLineItem()
	    {
	        try
	        {
	            return _navi.PushAsync(_pages.GetPage(Pages.UpdateParLineItem));
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	            return Task.FromResult(false);
	        }
	    }

		public async Task ShowInvitations(){
			await _navi.PushAsync (_pages.GetPage (Pages.Invitations));	
		}

	    public Task ShowUpdateReceivingOrderLineItem()
	    {
	        try
	        {
	            return _navi.PushAsync(_pages.GetPage(Pages.UpdateRecievingOrderLineItem));
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
                return Task.FromResult(false);
	        }
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
			try{
				return _navi.PushAsync(_pages.GetPage(Pages.AuthorizeCreditCard));
			} catch(Exception e){
				HandleException (e);
				return Task.FromResult (false);
			}
		}

		public async Task ShowUserRegistration(){
			try{
				await _navi.PushAsync (_pages.GetPage (Pages.RegisterUser));
			} catch(Exception e){
				HandleException (e);
			}
		}

		public async Task CloseInventoryVisuallyMeasureItem ()
		{
			try{
				await App.InventoryViewModel.OnAppearing ();
				await _navi.PopAsync ();
			} catch(Exception e){
				HandleException(e);
			}
		}

	    public async Task CloseUpdateQuantity ()
		{
			#if __ANDROID__
				//reload the items prior to us going there
			App.PARViewModel.OnAppearing ();
			#endif
			try{
				await  _navi.PopAsync ();
			} catch(Exception e){
				HandleException(e);
			}
		}

		public Task CloseItemScan ()
		{
			return _navi.PopModalAsync ();
		}

		public async Task CloseVendorAdd(){
			try{
				await _navi.PopAsync ();
				//just go to RO
			} catch(Exception e){
				HandleException (e);
			}
		}

		public async Task CloseItemAdd(){
			try{
				var findPage = _navi.NavigationStack.FirstOrDefault(p => p is ItemFindPage);
				//this should have us back to find, we need one more
				if(findPage != null){
					_navi.RemovePage (findPage);
				}
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
			} catch(Exception e){
				HandleException (e);
			}
		}

		public async Task CloseItemFind(){
			try{
				await _navi.PopAsync ();
			} catch(Exception e){
				HandleException (e);
			}
		}

	    public async Task CloseReceivingOrder()
	    {
	        try
	        {
	            await ShowMainMenu();
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }
	    }

        #region Reports

	    public async Task ShowReports()
	    {
	        try
	        {
	            await _navi.PushAsync(_pages.GetPage(Pages.Reports));
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }

	    }

	    public Task ShowSelectCompletedInventory()
        {
            try
            {
                return _navi.PushAsync(_pages.GetPage(Pages.CompletedInventoriesSelect));
            }
            catch (Exception e)
            {
                HandleException(e);
                return Task.FromResult(false);
            }
        }

        public async Task ShowReportResults()
	    {
	        try
	        {
	            await _navi.PushAsync(_pages.GetPage(Pages.ReportResults));
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }
	    }
        #endregion	

	    public async void HandleException(Exception e){
			try{
				_logger.HandleException (e);
				await _navi.DisplayAlert ("Error", e.Message);
			} catch(Exception ex){
				_logger.HandleException(ex);
			}
		}
	}
}