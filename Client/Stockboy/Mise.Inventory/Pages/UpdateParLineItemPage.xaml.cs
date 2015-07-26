using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;
using MR.Gestures;
namespace Mise.Inventory
{
	public partial class UpdateParLineItemPage : MR.Gestures.ContentPage
	{
		bool swipeInProgress;
		public UpdateParLineItemPage ()
		{
			var vm = App.UpdateParLineItemViewModel;
			BindingContext = vm;
			InitializeComponent ();
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
				case Direction.Left:
					if (vm.MovePreviousCommand.CanExecute (null)) {
						vm.MovePreviousCommand.Execute (null);
					}
					break;
				case Direction.Right:
				case Direction.NotClear:
				default:
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

