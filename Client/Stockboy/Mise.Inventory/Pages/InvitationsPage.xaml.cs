using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core;

namespace Mise.Inventory
{
	public partial class InvitationsPage : ContentPage
	{
		public InvitationsPage ()
		{
			BindingContext = App.InvitationViewModel;
			InitializeComponent ();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "InvitationsPage"}});
			DoAppear ();
		}

		private async void DoAppear(){
			var vm = BindingContext as InvitationViewModel;
			if(vm != null){
				vm.LoadDataOnView = LoadItems;
				await vm.OnAppearing ();
			}
		}

		private void LoadItems(){
			var vm = App.InvitationViewModel;
			foreach(var invite in vm.InvitesForUser){
				var container = new StackLayout();

				var label = new Label{ Text = invite.RestaurantName.ShortName};
				container.Children.Add (label);

				stckInvites.Children.Add (container);
			}
		}
	}
}

