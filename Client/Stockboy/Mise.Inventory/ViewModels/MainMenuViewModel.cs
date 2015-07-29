﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class MainMenuViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;
	    private readonly IInventoryService _inventoryService;
		public MainMenuViewModel(ILogger logger, IAppNavigation navigationService, ILoginService loginService, IInventoryService inventoryService)
			:base(navigationService, logger)
		{
		    _loginService = loginService;
		    _inventoryService = inventoryService;
		}

	    public override async Task OnAppearing(){
			try{
				var emp = await _loginService.GetCurrentEmployee ();
				if(emp == null){
					await Navigation.ShowLogin ();
				}
					
				var rest = await _loginService.GetCurrentRestaurant ();
				if (rest == null) {
					RestaurantName = "ERROR NO RESTAURANT";
				} else {
	                //if we're not a production one, let's make it obvious!
				    var buildLevel = DependencySetup.GetBuildLevel();
				    switch (buildLevel)
				    {
				        case BuildLevel.Debugging:
	                        RestaurantName = "DEBUG " + rest.Name.ShortName;
				            break;
	                    case BuildLevel.Demo:
	                        RestaurantName = "DEMO " + rest.Name.ShortName;
				            break;
	                    case BuildLevel.Development:
	                        RestaurantName = "DEV " + rest.Name.ShortName;
				            break;
	                    case BuildLevel.QA:
	                        RestaurantName = "QA " + rest.Name.ShortName;
				            break;
	                    case BuildLevel.Production:
	                        RestaurantName = rest.Name.ShortName;
				            break;
				    }
					
				}

				await _inventoryService.LoadLatest();
			    var inv = _inventoryService.GetSelectedInventory();
			    if (inv == null || inv.IsCompleted)
			    {
			        InventoryButtonText = "Start Count";
			    }
			    else
			    {
			        InventoryButtonText = "Continue Count";
			    }
			} catch(Exception e){
				HandleException (e);
			}
		}

		public string RestaurantName{get{ return GetValue<string>(); }set{ SetValue(value); }}
        public string InventoryButtonText {
            get { return GetValue<string>(); }
            set{SetValue(value); }
        }

		#region Commands

		public ICommand ReceivingOrderCommand {
			get{ return new SimpleCommand(ReceivingOrder, () => NotProcessing); }
		}

		/// <summary>
		/// Gets the inventories command.
		/// </summary>
		/// <value>The inventories command.</value>
		public ICommand SectionSelectCommand {
			get { return new SimpleCommand(SectionSelect, () => NotProcessing); }
		}
			
		public ICommand PARCommand {
			get { return new SimpleCommand(PAR, () => NotProcessing); }
		}

		public ICommand ManageEmployeesCommand {
			get { return new SimpleCommand(ManageEmployees, () => NotProcessing); }
		}

		public ICommand CreatePurchaseOrderCommand{
			get{return new SimpleCommand (CreatePurchaseOrder, () => NotProcessing);}
		}
		/// <summary>
		/// Gets the reports command.
		/// </summary>
		/// <value>The reports command.</value>
		public ICommand ReportsCommand {
			get { return new SimpleCommand(Reports, IsCurrentUserAdmin); }
		}

		public ICommand LogoutCommand {
			get { return new SimpleCommand(Logout, () => NotProcessing); }
		}

		#endregion

		async void ReceivingOrder()
		{
			try{
			await Navigation.ShowVendorFind ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		/// <summary>
		/// Inventories this instance.
		/// </summary>
		async void SectionSelect()
		{
			try{
                Processing = true;
                var selectedInventory = await _inventoryService.GetSelectedInventory();
                if (selectedInventory == null)
                {
                    //TODO might want to alert the user to this
                    await _inventoryService.StartNewInventory();
                }
			    Processing = false;
			await Navigation.ShowSectionSelect();
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// PA this instance.
		/// </summary>
		async void PAR()
		{
			try{
				await Navigation.ShowPAR();
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Manages the employees.
		/// </summary>
		async void ManageEmployees()
		{
			try{
				await Navigation.ShowStaff();
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// Reports this instance.
		/// </summary>
		async void Reports()
		{
			try{
				await Navigation.ShowReports();
			} catch(Exception e){
				HandleException(e);
			}
		}

		async void CreatePurchaseOrder()
		{
			try{
				Processing = true;
				await Navigation.ShowCreatePurchaseOrder();
				Processing = false;
			} catch(Exception e){
				HandleException(e);
				Processing = false;
			}
		}

		/// <summary>
		/// Logout this instance.
		/// </summary>
		async void Logout()
		{
			try{
				Processing = true;
				await _loginService.LogOutAsync();
				Processing = false;
				App.LoginViewModel.Username = string.Empty;
				App.LoginViewModel.Password = string.Empty;
				await Navigation.ShowLogin();
			} catch(Exception e){
				HandleException(e);
			}
		}

		/// <summary>
		/// If this is true, our current user is an admin, and can access our admin stuff
		/// </summary>
		/// <returns><c>true</c> if this instance is current user admin; otherwise, <c>false</c>.</returns>
		bool IsCurrentUserAdmin()
		{
			return NotProcessing;
		}
	}
}

