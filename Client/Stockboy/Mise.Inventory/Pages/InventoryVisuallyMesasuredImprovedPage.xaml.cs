using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
using XLabs.Platform.Device;
namespace Mise.Inventory.Pages
{
	public partial class InventoryVisuallyMesasuredImprovedPage : ContentPage
	{
		private List<MeasureButton> MeasureButtons;
		public InventoryVisuallyMesasuredImprovedPage ()
		{
			var vm = App.InventoryVisuallyMeasureItemImprovedViewModel;
			this.BindingContext = vm;

			InitializeComponent ();

			CreateMeasureBottle (vm);
		}

		private void CreateMeasureBottle(InventoryVisuallyMeasureItemImprovedViewModel vm){

			MeasureButtons = new List<MeasureButton> ();

			var device = App.Resolve<IDevice> ();

			//determine the max height, and make the level hieght off of that
			double levelSize = 25;
			double bottleWidth = 100;
			double scaleHeight = 1.0;
			//we only scale by height, to keep our shape properly formatted
			if(device != null && device.Display != null){
				var maxHeight = device.Display.Height - 200;
				scaleHeight = device.Display.Height *  (1.0/ maxHeight);
			} 
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

			var container = new AbsoluteLayout ();
			stckMeasure.Children.Add (container);

			//add the top empty one
			var addPoint = new Point (centerPoint, 0);

			foreach (var mB in MeasureButtons) {
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
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "InventoryVisuallyMeasuredImprovedPage"}});
			var vm = BindingContext as InventoryVisuallyMeasureItemImprovedViewModel;
			if(vm != null){
				await vm.OnAppearing ();
			}
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

