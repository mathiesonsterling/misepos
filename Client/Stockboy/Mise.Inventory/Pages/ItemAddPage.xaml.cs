using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using XLabs.Forms.Mvvm;

namespace Mise.Inventory.Pages
{
	public partial class ItemAddPage : ContentPage
	{
		public ItemAddPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "ItemAddPage"}});
			var vm = BindingContext as ItemAddViewModel;
			if(vm != null){
				await vm.OnAppearing ();
				LoadPickers ();
			}
		}

		public void LoadPickers(){
			//get the picker items
			var vm = BindingContext as ItemAddViewModel;
			if(vm != null){
				pckContainer.Items.Clear ();
				foreach(var opt in vm.PossibleContainerNames){
					pckContainer.Items.Add (opt);
				}

				pckContainer.SelectedIndexChanged += (sender, e) => {
					var selected = pckContainer.Items[pckContainer.SelectedIndex];
					vm.SelectedContainerName = selected;
				};

				pckCategory.Items.Clear ();
				foreach(var cat in vm.PossibleCategoryNames){
					pckCategory.Items.Add (cat);
				}

				pckCategory.SelectedIndexChanged += (sender, e) => {
					var selected = pckCategory.Items[pckCategory.SelectedIndex];
					vm.SelectedCategoryName = selected;
				};
			}
				
		}
	}
}

