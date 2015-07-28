﻿using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Mise.Inventory.Themes;
using Xamarin.Forms;
using MR.Gestures;
using System.Threading.Tasks;


namespace Mise.Inventory
{
	public partial class UpdateParLineItemPage : Xamarin.Forms.ContentPage
	{
		bool swipeInProgress;
		public UpdateParLineItemPage ()
		{
			var vm = App.UpdateParLineItemViewModel;
			BindingContext = vm;
			InitializeComponent ();

			vm.MovePreviousAnimation = async () => 
				await stckMain.TranslateTo (stckMain.Width, 0, MiseTheme.SwipeAnimationDuration);

			vm.MoveNextAnimation = async () => 
				await stckMain.TranslateTo (this.Width * -1, 0, MiseTheme.SwipeAnimationDuration);

			vm.ResetViewAnimation = () => {
				stckMain.TranslationX = 0;
				return Task.FromResult (true);
			};
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = BindingContext as UpdateParLineItemViewModel;
			if (vm != null) {
				await vm.OnAppearing();

				btnNext.IsEnabled = vm.MoveNextCommand.CanExecute (null);
			}
		}

		private void OnSwiped(object sender, SwipeEventArgs e){
			if(swipeInProgress){
				return;
			}
			var vm = BindingContext as UpdateParLineItemViewModel;

			if (vm != null) {
				swipeInProgress = true;
				switch (e.Direction) {
				case Direction.Right:
					if (vm.MovePreviousCommand.CanExecute (null)) {
						vm.MovePreviousCommand.Execute (null);
					}
					break;
				case Direction.Left:
				case Direction.NotClear:
					if (vm.MoveNextCommand.CanExecute (null)) {
						vm.MoveNextCommand.Execute (null);
					}
					break;
				}
				swipeInProgress = false;

			}
		}
	}
}

