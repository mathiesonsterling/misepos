using System;
using Mise.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using Mise.Inventory.Services;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using System.Windows.Input;
using Mise.Inventory.MVVM;
using System.Collections.ObjectModel;
using Mise.Core.Entities.Vendors;
using Xamarin;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class PODisplayLineItem : BaseLineItemDisplayLine<IPurchaseOrderLineItem>{
		public PODisplayLineItem(IPurchaseOrderLineItem source): base(source){
			
		}

		#region implemented abstract members of BaseLineItemDisplayLine

		public override Color TextColor {
			get {
				return Color.Default;
			}
		}

		public override decimal Quantity {
			get {
				return Source.Quantity;
			}
		}

		public override string DetailDisplay {
			get {
				var res = string.Empty;
				if(Source.GetCategories ().Any()){
					res += Source.GetCategories().First ().Name;
				}
				if(Source.Container != null){
					res += "  " + Source.Container.DisplayName;
				}
				return res;
			}
		}

		#endregion
	}

	public class PurchaseOrderReviewViewModel : BaseViewModel
	{
		readonly IPurchaseOrderService _poService;
		readonly IVendorService _vendorService;
		readonly ILoginService _loginService;
	    private readonly IInsightsService _insights;

		public PurchaseOrderReviewViewModel(ILogger logger, IAppNavigation appNavi, IPurchaseOrderService poService, 
			IVendorService vendorService, ILoginService loginService, IInsightsService insightsService) : base(appNavi, logger)
		{
			_poService = poService;
			_vendorService = vendorService;
			_loginService = loginService;
		    _insights = insightsService;
		}

		public bool FullInventory{ get{ return GetValue<bool> ();} set{ SetValue (value);} }

		public override async Task OnAppearing(){
			try{
				FullInventory = false;
				Processing = true;
				await LoadPO ();	
				Processing = false;

				if(VendorsAndPOs.Any()){
					if(LoadItemsOnPage != null){
						LoadItemsOnPage ();
					}
				} else {
					FullInventory = true;
					var rest = await _loginService.GetCurrentRestaurant();
					_insights.Track("Fully stocked when creating PO", new Dictionary<string, string>{{"Restaurant ID", rest.ID.ToString ()}});
				}
			} catch(Exception e){
				HandleException (e);
			}
		}
			
		public Action LoadItemsOnPage{ get; set;}

		IPurchaseOrder _po;
		async Task LoadPO(){
			_po = await _poService.CreatePurchaseOrder ();
			if (_po == null) {
				throw new Exception ("Error, could not create Purchase Order");
			}
			_insights.Track("Created Purchase Order", new Dictionary<string, string>{{"PO ID", _po.ID.ToString ()}});
			var rest = await _loginService.GetCurrentRestaurant ();
			var poByVs = _po.GetPurchaseOrderPerVendors ();

			var vendors = (await _vendorService.GetVendorsAssociatedWithRestaurant (rest)).ToList ();

			var res = new List<VendorAndItems> ();
			foreach(var poByV in poByVs){
				IVendor vendor = null;
				if(poByV.VendorID.HasValue){
					vendor = vendors.FirstOrDefault (v => v.ID == poByV.VendorID.Value);
				}

				var newItem = new VendorAndItems {
					VendorName = vendor != null ? vendor.Name : "No Vendor",
					POLineItems = new ObservableCollection<PODisplayLineItem>(
						poByV.GetLineItems ().Select(p => new PODisplayLineItem (p))
					)
				};
				res.Add (newItem);
			}

			var ordered = res.OrderBy (v => v.VendorName == "No Vendor")
				.ThenBy (v => v.VendorName);
			var list = new ObservableCollection<VendorAndItems> (ordered);
			VendorsAndPOs = list;

		}

		public ObservableCollection<VendorAndItems> VendorsAndPOs{ get; private set;}


		public ICommand SubmitPOCommand{get{return new SimpleCommand (SubmitPO);}}

		async void SubmitPO(){
			try{
				//send the PO
				Processing = true;
				await _poService.SubmitPO (_po);
				Processing = false;
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		public class VendorAndItems{
			public string VendorName{get;set;}
			public ObservableCollection<PODisplayLineItem> POLineItems{get;set;}
		}
	}
}

