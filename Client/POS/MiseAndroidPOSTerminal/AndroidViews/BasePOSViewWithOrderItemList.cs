using System;
using System.Linq;
using Android.Views;
using Android.Widget;

using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Client.ApplicationModel;
using MiseAndroidPOSTerminal.Themes;
using MiseAndroidPOSTerminal.AndroidControls;

namespace MiseAndroidPOSTerminal.AndroidViews
{
	public class BasePOSViewWithOrderItemList : BasePOSView
	{
		protected LinearLayout OrderItemsCol;


		protected LinearLayout CreateOrderItemArea ()
		{
			var oiWrapper = new LinearLayout (this) {
				Orientation = Orientation.Vertical
			};
			var oiWrapperParams = new ViewGroup.LayoutParams (POSTheme.OrderItemPanelWidth, POSTheme.OrderMiddleRowHeight);
			oiWrapper.LayoutParameters = oiWrapperParams;
			OrderItemsCol = new LinearLayout (this) {
				Orientation = Orientation.Vertical
			};
			OrderItemsCol.SetMinimumWidth (POSTheme.OrderItemPanelWidth);

			oiWrapper.AddView (OrderItemsCol);
			oiWrapper.SetBackgroundColor (POSTheme.OrderItemListBackgroundColor);
			return oiWrapper;
		}

		/// <summary>
		/// Synh the orderItemsCol with our current bartab
		/// </summary>
		/// <param name = "viewModel"></param>
		/// <param name="orderItemsCol"></param>
		/// <param name = "clickHandler"></param>
		protected void PopulateOrderItemsList(ITerminalApplicationModel viewModel, 
		                                      ViewGroup orderItemsCol, 
		                                      EventHandler clickHandler)
		{
			if (orderItemsCol == null) {
				Logger.Log ("Order Items Col is null, cannot populate!");
				throw new Exception ("Order items col is null");
			}
			orderItemsCol.RemoveAllViews();

			var selectedBartab = viewModel.SelectedCheck;

			if (viewModel.SelectedCheck == null) {
				Logger.Log ("No selected tab currently on view model");
				throw new Exception ("No tab has been selected!");
			}

			//hold our column
			var nameText = string.IsNullOrEmpty (selectedBartab.GetTopOfCheck ()) 
				? "Order" 
				: selectedBartab.GetTopOfCheck().Length > POSTheme.MaxTabNameLength 
				?selectedBartab.GetTopOfCheck().Substring (0, POSTheme.MaxTabNameLength)
				:selectedBartab.GetTopOfCheck();
			var name = new TextView(this){Text = nameText};
			name.SetBackgroundColor (POSTheme.OrderItemListBackgroundColor);
			name.SetTextColor (POSTheme.OrderItemTextColor);
			name.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.CheckNameTextSize);
			name.SetTextColor (POSTheme.OrderItemTextColor);
			orderItemsCol.AddView(name);

			var totalWithTax = viewModel.GetTotalWithSalesTaxForSelectedCheck();
			var total = new TextView (this){ Text = totalWithTax.ToFormattedString() };
			total.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.CheckNameTextSize);
			total.SetTextColor (POSTheme.OrderItemTextColor);
			orderItemsCol.AddView (total);

			var scroller = new ScrollView (this);
			var itemsPanel = new LinearLayout(this){ Orientation = Orientation.Vertical };
			scroller.AddView (itemsPanel);
			orderItemsCol.AddView (scroller);

			//display the grats FIRST
			foreach(var discount in selectedBartab.GetDiscounts().OrderByDescending (d => d.AddsMoney)){
				var amt = discount.GetDiscountAmount (selectedBartab.Total);
				var symbol = discount.AddsMoney? "+":"-";
				var label = new TextView (this){ Text = symbol + discount.Name + "  " + amt.ToFormattedString () };
				label.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.OrderItemTextSize);
				label.SetTypeface (POSTheme.Typeface, POSTheme.TypefaceStyle);
				label.SetTextColor(discount.AddsMoney?POSTheme.GratuityColor:POSTheme.DiscountColor); 
				label.Gravity = GravityFlags.Right;
				itemsPanel.AddView (label);
			}

			foreach (var orderItem in selectedBartab.OrderItems.Reverse ())
			{
				var isSent = orderItem.Status == OrderItemStatus.Sent;
				var textColor = orderItem.IsComped
					? POSTheme.CompColor
					:isSent
						?POSTheme.OrderItemTextColorSent 
						: POSTheme.OrderItemTextColorUnsent;
				var symbol = isSent? "":"+";

				var amtText = orderItem.IsComped ? "COMP" : orderItem.Total.ToFormattedString ();
				var label = new OrderItemLabel (this) { 
					Text = symbol + orderItem.MenuItem.OIListName +"  " + amtText,
					OrderItemID = orderItem.ID,
				};
				label.SetTypeface (POSTheme.Typeface, POSTheme.TypefaceStyle);
				label.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.OrderItemTextSize);
				label.Gravity = GravityFlags.Right;

				label.SetTextColor (textColor);
				label.SetBackgroundColor (POSTheme.OrderItemListBackgroundColor);

				label.Click += clickHandler;
				itemsPanel.AddView(label);

				//add any mods we have here
				foreach (var mod in orderItem.Modifiers) {
					var text = mod.PriceChange.HasValue 
						? "  -" + mod.Name + "  " + mod.PriceChange.ToFormattedString()
							: "  -" + mod.Name + "     ";
					var modL = new TextView(this){Text = text, Gravity = GravityFlags.Right};
					modL.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.OrderItemModifierTextSize);
					modL.SetTextColor (textColor);
					itemsPanel.AddView(modL);
				}

				if (string.IsNullOrEmpty (orderItem.Memo) == false) {
					var memo = new TextView (this){ Text = orderItem.Memo, Gravity = GravityFlags.Right };
					memo.SetTextSize (Android.Util.ComplexUnitType.Pt, POSTheme.OrderItemModifierTextSize);
					itemsPanel.AddView (memo);
				}
			} //end order items
		}
			
	}
}

