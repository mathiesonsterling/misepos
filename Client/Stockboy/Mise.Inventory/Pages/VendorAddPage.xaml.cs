using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
	public partial class VendorAddPage : ContentPage
	{
		public VendorAddPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "VendorAddPage"}});

			var vm = BindingContext as VendorAddViewModel;
			if (vm != null) {
				pckState.Items.Clear ();
				foreach (var state in vm.States) {
					pckState.Items.Add (state.Abbreviation);
				}

				pckState.SelectedIndexChanged += (sender, e) => {
					var selectedItem = pckState.Items [pckState.SelectedIndex];
					vm.State = selectedItem;
				};
			}
		}
	}
}

