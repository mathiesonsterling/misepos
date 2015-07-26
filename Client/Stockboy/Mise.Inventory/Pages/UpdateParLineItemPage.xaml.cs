using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Mise.Inventory.Themes;
using Xamarin.Forms;
using MR.Gestures;
using System.Threading.Tasks;


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

		private async void OnSwiped(object sender, SwipeEventArgs e){
			if(swipeInProgress){
				return;
			}
			var vm = BindingContext as UpdateParLineItemViewModel;

			if (vm != null) {
				var visEl = sender as VisualElement;
				swipeInProgress = true;
				switch (e.Direction) {
				case Direction.Left:
					if (vm.MovePreviousCommand.CanExecute (null)) {
						if (visEl != null) {
							await visEl.TranslateTo (visEl.Width, 0, MiseTheme.SwipeAnimationDuration);
						}
						vm.MovePreviousCommand.Execute (null);
					
						if (visEl != null) {
							visEl.TranslationX = 0;
						}
					}
					break;
				case Direction.Right:
				case Direction.NotClear:
					if (vm.MoveNextCommand.CanExecute (null)) {
						if (visEl != null) {
							//start animation
							await visEl.TranslateTo (-1 * visEl.Width, 0, MiseTheme.SwipeAnimationDuration);
						}
						vm.MoveNextCommand.Execute (null);
					
						if (visEl != null) {
							//move back, hopefully too quick to see
							visEl.TranslationX = 0;
						}
					}
					break;
				}
				swipeInProgress = false;

			}
		}
	}
}

