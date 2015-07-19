using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class PurchaseOrderReviewPage : ContentPage
	{
		public PurchaseOrderReviewPage ()
		{
			var vm = App.PurchaseOrderReviewViewModel;
			vm.LoadItemsOnPage = LoadItems;
			BindingContext = vm;
			InitializeComponent ();
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "PurchaseOrderReviewPage"}});
			var vm = App.PurchaseOrderReviewViewModel;
		    if (vm != null)
		    {
		        await vm.OnAppearing();
		    }
		}

		void LoadItems(){
			var vm = App.PurchaseOrderReviewViewModel;

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

