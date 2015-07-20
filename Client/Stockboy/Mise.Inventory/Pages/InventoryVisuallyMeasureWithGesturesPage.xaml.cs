using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using XLabs.Platform.Device;
using Xamarin;
namespace Mise.Inventory.Pages
{
	public partial class InventoryVisuallyMeasureWithGesturesPage : ContentPage
	{
		private List<MeasureButton> MeasureButtons;
		private double measureHeight = 300;
		private bool loading = false;
		public InventoryVisuallyMeasureWithGesturesPage ()
		{
			var vm = App.InventoryVisuallyMeasureBottleViewModel;
			this.BindingContext = vm;

			InitializeComponent ();

			using(Insights.TrackTime("Time to create measure bottle")){
				stckMeasure.Children.Clear ();
				CreateMeasureBottle (vm, measureHeight);
			}

			stckMeasure.SizeChanged += (sender, e) => {
				var updated = sender as VisualElement;
				if(updated != null){
					//we can get our height now!
					if(updated.Height > 0 && Math.Abs (updated.Height - measureHeight) > 50){
						measureHeight = updated.Height;

						var ratio = measureHeight * (1.0/300);
						foreach(var c in stckMeasure.Children){
							var container = c as AbsoluteLayout;
							if(container != null){
								foreach(var containerChild in container.Children){
									var mb = containerChild as MeasureButton;
									if(mb != null){
										mb.HeightRequest = 25 * ratio;
									}
								}
							}
						}
					}
					//TODO update our controls with the new height
				}
			};
		}

		protected override async void OnAppearing ()
		{
			Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "InventoryVisuallyMeasuredImprovedPage"}});

			var vm = BindingContext as InventoryVisuallyMeasureBottleViewModel;
			if(vm != null){
				await vm.OnAppearing ();
			}
		}

		private void CreateMeasureBottle(InventoryVisuallyMeasureBottleViewModel vm, double height){
			if(loading){
				return;
			}
			loading = true;
			MeasureButtons = new List<MeasureButton> ();

			var device = App.Resolve<IDevice> ();

			//determine the max height, and make the level hieght off of that
			double levelSize = 25;
			double bottleWidth = 100;
			double baseHeight = 300;
			double scaleHeight = height * (1.0 / baseHeight);
				
			levelSize = levelSize * scaleHeight;
			bottleWidth = bottleWidth * scaleHeight;

			//TODO change this to create, and separately set sizes.  Then set sizes on SizeChanged event
			var profileWidths = new []{ 90, 100, 100, 100, 100, 100, 90, 50, 25, 25 };
			for (int i = 0; i < 10; i++) {
				var width = profileWidths [i] * bottleWidth / 100;
				var button = new MeasureButton (i){WidthRequest = width, HeightRequest = levelSize};
				MeasureButtons.Add (button);
				button.Clicked += (sender, e) => {
					//get our position
					var mb = sender as MeasureButton;
					if(mb != null){
						var position = mb.Position;
						foreach(var mBOther in MeasureButtons){
							if(mBOther.Position <= position){
								mBOther.SetOn();
							} else {
								mBOther.SetOff();
							}
						}

						var posActual = (decimal)position + 1;
						var val = Math.Round(posActual/10.0M, 1);
						vm.CurrentPartial = val;
					}
				};
			}

			MeasureButtons.Reverse ();

			var centerPoint = bottleWidth / 2;

			var container = new AbsoluteLayout {HeightRequest = height};
			stckMeasure.Children.Add (container);

			//add the top empty one
			var addPoint = new Point (centerPoint, 0);

			foreach (var mB in MeasureButtons) {
				mB.HorizontalOptions = LayoutOptions.Center;
				addPoint.X = centerPoint - mB.WidthRequest / 2;
				container.Children.Add (mB, addPoint);
				addPoint.Y += levelSize;
			}

			Action resetMarkers = () => {
				if(MeasureButtons != null){
					foreach(var mb in MeasureButtons){
						mb.SetOff();
					}
				}
			};
			vm.ResetMarkers = resetMarkers;
			loading = false;
		}

		public class MeasureButton : Button{
			public static Color OffColor = Color.Gray;
			public static Color OnColor = Color.Navy;

			public MeasureButton(int position) : base(){
				BackgroundColor = OffColor;
				On = false;
				Position = position;
				WidthRequest = 25;
			}

			public bool On{get;private set;}
			public int Position{get;private set;}

			public void SetOn(){
				On = true;
				BackgroundColor = OnColor;
			}

			public void SetOff(){
				On = false;
				BackgroundColor = OffColor;
			}
		}
	}
}

