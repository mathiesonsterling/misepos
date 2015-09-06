using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Pages
{
	public partial class PurchaseOrderReviewPage : BasePage
	{
		public PurchaseOrderReviewPage ()
		{
			var vm = ViewModel as PurchaseOrderReviewViewModel;
			vm.LoadItemsOnPage = LoadItems;
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage
		public override BaseViewModel ViewModel {
			get {
				return App.PurchaseOrderReviewViewModel;
			}
		}
		public override String PageName {
			get {
				return "PurchaseOrderReviewPage";
			}
		}
		#endregion			

		void LoadItems(){
			var vm = ViewModel as PurchaseOrderReviewViewModel;

			foreach(var vendorAndItems in vm.VendorsAndPOs){
				var vendorLabel = new Label{ 
					Text = vendorAndItems.VendorName, 
					FontAttributes = FontAttributes.Bold,
					FontSize = 24
				};
				stckItems.Children.Add (vendorLabel);

				foreach(var poLineItem in vendorAndItems.POLineItems.OrderBy(li => li.DisplayName)){
					var lineStack = new StackLayout { Orientation = StackOrientation.Horizontal };

					var itemL = new Label{ 
						Text = poLineItem.DisplayName, 
						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
						WidthRequest = 300,
						LineBreakMode = LineBreakMode.TailTruncation
					};
					itemL.HorizontalOptions = LayoutOptions.FillAndExpand;
					lineStack.Children.Add (itemL);

					var quantL = new Label{ Text = poLineItem.Quantity .ToString ()};
					quantL.HorizontalOptions = LayoutOptions.FillAndExpand;
					quantL.FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label));
					quantL.WidthRequest = 50;
					lineStack.Children.Add (quantL);

					stckItems.Children.Add (lineStack);

					//do we have a price?
					var detailL = new Label{Text = poLineItem.DetailDisplay, FontSize=10};
					stckItems.Children.Add (detailL);
				}
					
			}
		}
	}
}

