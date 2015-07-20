using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Services;
using Xamarin.Forms;
namespace Mise.Inventory.ViewModels
{
	public class ReceivingOrderDisplayLine : BaseLineItemDisplayLine<IReceivingOrderLineItem>{
		public ReceivingOrderDisplayLine(IReceivingOrderLineItem source) : base(source){
			
		}


		#region implemented abstract members of BaseLineItemDisplayLine
		public override Color TextColor {
			get {
				return (Source.LineItemPrice != null && Source.LineItemPrice.HasValue) || Source.ZeroedOut
						? Color.Default
						: Color.Accent;
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
					res += Source.GetCategories ().First ().Name;
				}
				if (Source.ZeroedOut) {
					res += " ZEROED OUT";
				} else {
					if (Source.LineItemPrice != null) {
						res += "  " + string.Format ("{0:C}", Source.LineItemPrice.Dollars);
					} else {
						res += " NO PRICE";
					}
				}
				return res;
			}
		}
		#endregion
	}

	public class ReceivingOrderViewModel : BaseSearchableViewModel<ReceivingOrderDisplayLine>
	{
		readonly IVendorService _vendorService;
		readonly IReceivingOrderService _roService;
	    private readonly IInsightsService _insights;
		public ReceivingOrderViewModel(ILogger logger, IAppNavigation appNavigation,
			IReceivingOrderService roService, IVendorService vendorService, IInsightsService insights) : base(appNavigation, logger)
		{
			_vendorService = vendorService;
			_roService = roService;
		    _insights = insights;
			_itemSettingQuantity = null;

		    PropertyChanged += (sender, args) =>
		    {
		        if (args.PropertyName != "CanSave")
		        {
					CanSave = NotProcessing && (string.IsNullOrEmpty(InvoiceID) == false) && LineItems.Any() 
						&& LineItems.All(li => li.Source.ZeroedOut 
							|| (li.Source.LineItemPrice != null && li.Source.LineItemPrice.HasValue));
						
		        }
		    };
		}
			
			
		IReceivingOrderLineItem _itemSettingQuantity;

		public string Title{get{return GetValue<string> ();}private set{ SetValue (value); }}
		public string VendorName{ get { return GetValue<string> (); } private set { SetValue (value); } }

		public string Notes{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string InvoiceID{get{return GetValue<string> ();}set{SetValue(value);}}
		public bool CanSave{get{return GetValue<bool> ();}private set{SetValue (value);}}

		#region Commands

		public ICommand AddNewItemCommand {
			get { return new SimpleCommand(AddNewLineItem); }
		}

		public ICommand SaveCommand {
			get { return new SimpleCommand(Save); }
		}

		#endregion

		async void AddNewLineItem()
		{
			await Navigation.ShowReceivingOrderItemFind ();
		}

		async void Save()
		{
			try{
			    using (_insights.TrackTime("Save RO time"))
			    {
			        Processing = true;
			        CanSave = false;
                    PurchaseOrderStatus status;
			        using (_insights.TrackTime("Completing receiving order"))
			        {
			            var res = await _roService.CompleteReceivingOrderForSelectedVendor(Notes, InvoiceID);
			            status = res ? PurchaseOrderStatus.ReceivedTotally : PurchaseOrderStatus.ReceivedWithAlterations;
			        }
                    using(_insights.TrackTime("Committing receiving order"))
			        {
			            await _roService.CommitCompletedOrder(status);
			        }
			        Processing = false;
			        CanSave = true;
			        await Navigation.CloseReceivingOrder();
			    }
			} catch(Exception e){
				HandleException (e);
			}
		}

		public override async Task SelectLineItem(ReceivingOrderDisplayLine lineItem){
			try{
				var roLineItem = lineItem.Source;
				_itemSettingQuantity = roLineItem;
				await Navigation.ShowUpdateQuantity (roLineItem.Quantity, roLineItem.DisplayName, 
					QuantitySetCallback, ZeroOutCallback, roLineItem.LineItemPrice, true, "Receive Item");
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void ZeroOutCallback(){
			if(_itemSettingQuantity != null){
				Processing = true;
				await _roService.ZeroOutLineItem (_itemSettingQuantity);
				Processing = false;
			}

			await LoadItems ();
		}

		async void QuantitySetCallback(int newQuant, decimal price){
			if(_itemSettingQuantity != null)
			{
				var rawPerBottle = price;
				var priceMoney = new Money (rawPerBottle);
				if(_itemSettingQuantity.Quantity != newQuant || 
					(
						priceMoney.HasValue && (_itemSettingQuantity.LineItemPrice == null ||( _itemSettingQuantity.LineItemPrice.Equals(priceMoney) == false))
					)
				){
					await _roService.UpdateQuantityOfLineItem (_itemSettingQuantity, newQuant, priceMoney);
				}
			}

			_itemSettingQuantity = null;
			await LoadItems ();
		}

		protected override async Task<ICollection<ReceivingOrderDisplayLine>> LoadItems(){
			//get the RO for the current vendor - if it doesn't exist it should be created
			var items = new List<IReceivingOrderLineItem>();
			try{
					var ro = await _roService.GetCurrentReceivingOrder();
					if (ro != null) {
						items = ro.GetBeverageLineItems ().ToList ();
				} else {
					throw new InvalidOperationException ("No RO exists!");
				}
			} catch(Exception e){
				HandleException (e);
			}
			return items.OrderBy (li => li.DisplayName).Select (li => new ReceivingOrderDisplayLine (li)).ToList ();
		}

		public override async Task OnAppearing ()
		{
			try{
				Processing = true;
				await base.OnAppearing ();
				var vendor = await _vendorService.GetSelectedVendor ();
				VendorName = vendor.Name;
				var ro = await _roService.GetCurrentReceivingOrder ();
			    if (ro != null)
			    {
			        InvoiceID = ro.InvoiceID;
			        var items = ro.GetBeverageLineItems().ToList();

			        if (ro.PurchaseOrderDate.HasValue)
			        {
			            Title = "Order from " + VendorName + " for PO";
			        }
			        else
			        {
			            Title = "Receive from " + VendorName;
			        }
			    }
			    else
			    {
                    Title = "Receive from " + VendorName;
			        InvoiceID = string.Empty;
			    }
				Processing = false;
			} catch(Exception e){
				HandleException (e);
			}
		}
			
		protected override void AfterSearchDone ()
		{
		}

	}
}

