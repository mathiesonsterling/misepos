using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;

using Mise.Core.Entities.Vendors;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using System.Collections.ObjectModel;
using System;
using Mise.Core.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class VendorFindViewModel : BaseSearchableViewModel<IVendor>
	{
		readonly IVendorService _vendorService;
		readonly ILoginService _loginService;
		readonly IPurchaseOrderService _poService;
		readonly IReceivingOrderService _roService;
		public bool CanAdd{get{return GetValue<bool> ();}set{SetValue (value); }}

		public VendorFindViewModel(IAppNavigation appNavigation, IVendorService vendorService, 
			ILoginService loginService, IPurchaseOrderService poService, IReceivingOrderService roService,
			ILogger logger) 
			: base(appNavigation, logger)
		{
			_vendorService = vendorService;
			_loginService = loginService;
			_poService = poService;
			_roService = roService;

			PropertyChanged += async (sender, e) =>{
				if(e.PropertyName == "ShowOutOfStateVendors"){
					await DoSearch ();
				}
			} ;
		}

		protected override void AfterSearchDone ()
		{
			CanAdd = LineItems.Any () == false;
		}

		public bool ShowOutOfStateVendors{get{return GetValue<bool> ();}set{ 
				SetValue (value);
			}
		}
		#region commands


		public ICommand AddNewVendorCommand {
			get { return new Command(AddNewVendor, () => CanAdd); }	
		}
			
		#endregion

		async void AddNewVendor()
		{
			try{
				//let's put our search term into the Name of the add
				var vm = App.VendorAddViewModel;
				vm.Name = SearchString;
				await Navigation.ShowVendorAdd();
			} catch(Exception e){
				HandleException (e);
			}
		}

		#region implemented abstract members of BaseSearchableViewModel

		protected override async Task<ICollection<IVendor>> LoadItems ()
		{
			Processing = true;
			var items = (await _vendorService.GetPossibleVendors ()).ToList();

			var rest = await _loginService.GetCurrentRestaurant ();
			Processing = false;
			//get our vendors that are tied to us FIRST

			if(ShowOutOfStateVendors == false){
				if(rest.StreetAddress != null && rest.StreetAddress.State != null){
					items = items.Where(v => v.StreetAddress == null || v.StreetAddress.State == null || 
						v.StreetAddress.State.Equals (rest.StreetAddress.State))
						.ToList();
				}
			}
			return items.OrderByDescending (v => v.GetRestaurantIDsAssociatedWithVendor ().Contains (rest.ID))
				.ThenBy (v => v.Name).ToList();
		}

		public override async Task SelectLineItem(IVendor lineItem){
			try{
				Processing = true;
				await _vendorService.SelectVendor (lineItem);
				//do we have POs for this vendor?  if so show them, if not go directly to new PO
				var posForVendor = await _poService.GetPurchaseOrdersWaitingForVendor(lineItem);

				if(posForVendor.Any())
				{
					Processing = false;
					await Navigation.ShowSelectPurchaseOrder ();
				} else {
					await _roService.StartReceivingOrderForSelectedVendor();
					Processing = false;
					await Navigation.ShowReceivingOrder();
				}
			} catch(Exception e){
				HandleException (e);
			}
		}
		#endregion
	}
}

