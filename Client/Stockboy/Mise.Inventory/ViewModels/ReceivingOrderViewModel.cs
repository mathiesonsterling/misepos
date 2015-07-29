using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;

using Mise.Inventory.Services;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Services;
using Xamarin.Forms;
using Mise.Core.ValueItems;
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
		public override string Quantity {
			get {
				return Source.Quantity.ToString();
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

			DateReceived = DateTime.Now;

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

		public override async Task OnAppearing ()
		{
			try{
				Processing = true;
				DateReceived = DateTime.Now;
				await base.OnAppearing ();
				var vendor = await _vendorService.GetSelectedVendor ();
				VendorName = vendor.Name;
				var ro = await _roService.GetCurrentReceivingOrder ();
				if (ro != null)
				{
					if (ro.PurchaseOrderDate.HasValue)
					{
						Title = "Order from " + VendorName + " for PO";
					}
					else
					{
						Title = "Receive from " + VendorName;
					}

					//debug
					CanSave = true;
					foreach(var li in ro.GetBeverageLineItems()){
						if(li.ZeroedOut == false){
							var hasPrice = li.UnitPrice != null && li.UnitPrice.HasValue && li.Quantity > 0;
							if(hasPrice == false){
								CanSave = false;
								break;
							}
						}
					}
					//CanSave = ro.GetBeverageLineItems().All(li => li.ZeroedOut || (li.UnitPrice != null && li.UnitPrice.HasValue && li.Quantity > 0));
				}
				else
				{
					Title = "Receive from " + VendorName;
					CanSave = false;
				}
				Processing = false;
			} catch(Exception e){
				HandleException (e);
			}
		}
		public DateTime DateReceived{get{return GetValue<DateTime> ();}set{ SetValue (value); }}

		public string Title{get{return GetValue<string> ();}private set{ SetValue (value); }}
		public string VendorName{ get { return GetValue<string> (); } private set { SetValue (value); } }

		public string Notes{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string InvoiceID{get{return GetValue<string> ();}set{SetValue(value);}}
		public bool CanSave{get{return GetValue<bool> ();}private set{SetValue (value);}}

		public ReceivingOrderDisplayLine FocusOnLineItem{
			get{return GetValue<ReceivingOrderDisplayLine> ();}
			private set{ SetValue (value); }
		}
		#region Commands

		public ICommand AddNewItemCommand {
			get { return new Command(AddNewLineItem, () => NotProcessing); }
		}

		public ICommand SaveCommand {
			get { return new Command(Save, () => CanSave); }
		}

		#endregion

		async void AddNewLineItem()
		{
			await Navigation.ShowReceivingOrderItemFind ();
		}

		async void Save()
		{
			try{
				Processing = true;
				CanSave = false;
			    using (_insights.TrackTime("Save RO time"))
			    {
                    PurchaseOrderStatus status;
			        using (_insights.TrackTime("Completing receiving order"))
			        {
			            var res = await _roService.CompleteReceivingOrderForSelectedVendor(DateReceived, Notes, InvoiceID);
			            status = res ? PurchaseOrderStatus.ReceivedTotally : PurchaseOrderStatus.ReceivedWithAlterations;
			        }
                    using(_insights.TrackTime("Committing receiving order"))
			        {
			            await _roService.CommitCompletedOrder(status);
			        }
			        CanSave = true;
					InvoiceID = string.Empty;
					DateReceived = DateTime.Now;
					Processing = false;
			        await Navigation.CloseReceivingOrder();
			    }
			} catch(Exception e){
				HandleException (e);
			}
		}

		public override async Task SelectLineItem(ReceivingOrderDisplayLine lineItem){
			try{
			    await _roService.SetCurrentLineItem(lineItem.Source);
			    await Navigation.ShowUpdateReceivingOrderLineItem();
			} catch(Exception e){
				HandleException (e);
			}
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

			var displayItems = items.OrderBy (li => li.DisplayName)
				.Select (li => new ReceivingOrderDisplayLine (li)).ToList ();
			FocusOnLineItem = displayItems.FirstOrDefault(li => 
				li.Source.ZeroedOut == false 
				&& (li.Source.LineItemPrice == null || li.Source.LineItemPrice.Equals(Money.None))
			);

			if(FocusOnLineItem == null){
				FocusOnLineItem = displayItems.LastOrDefault ();
			}
			return displayItems;
		}
			
		protected override void AfterSearchDone ()
		{
		}

	}
}

