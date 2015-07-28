﻿using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Pages
{
	public partial class SectionSelectPage : ContentPage
	{
		public SectionSelectPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "SectionSelectPage"}});
			var vm = BindingContext as SectionSelectViewModel;

			await vm.OnAppearing ();

			slOther.Children.Clear ();
			var template = new DataTemplate (typeof(TextCell));
			template.SetBinding (TextCell.TextProperty, "Name");
			var lv = new ListView {
				ItemsSource = vm.LineItems,
				ItemTemplate = template,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			lv.ItemTapped += async (sender, e) => {
				var selectedSection = e.Item as IInventorySection;
				((ListView)sender).SelectedItem = null;
				if(selectedSection != null){
					await vm.SelectLineItem(selectedSection);
				}
			};
			slOther.Children.Add (lv);
		}
	}
}

