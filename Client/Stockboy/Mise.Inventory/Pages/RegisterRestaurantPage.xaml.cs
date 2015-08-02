using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin;
using Mise.Inventory.ViewModels;


namespace Mise.Inventory
{
	public partial class RegisterRestaurantPage : ContentPage
	{
		public RegisterRestaurantPage ()
		{
			BindingContext = App.RestaurantRegistrationViewModel;
			InitializeComponent ();
		}

		protected override async void OnAppearing(){
			Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "RegisterRestaurantPage"}});

			var vm = BindingContext as RestaurantRegistrationViewModel;
			if(vm != null){
				await vm.OnAppearing ();
				pckState.Items.Clear ();
				foreach(var state in vm.States){
					pckState.Items.Add (state.Abbreviation);
				}

				pckState.SelectedIndexChanged += (sender, e) => {
					var selectedItem = pckState.Items[pckState.SelectedIndex];
					vm.State = selectedItem;
				};
			}
		}
	}
}

