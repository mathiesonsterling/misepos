﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.ValueItems.Inventory;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Xamarin;
using MR.Gestures;
using Mise.Inventory.Themes;

namespace Mise.Inventory.Pages
{
	public partial class InventoryVisuallyMeasureWithGesturesPage : BasePage
	{
		private List<MeasureButton> _measureButtons;
		private double _oldHeight = DEFAULT_HEIGHT;
	    private const double DEFAULT_HEIGHT = 200;
		private bool _loading = false;
		private LiquidContainerShape _shape;
		bool swipeInProgress = false;

		private void OnSwiped(object sender, SwipeEventArgs e){
			if(swipeInProgress){
				return;
			}
			var vm = BindingContext as InventoryVisuallyMeasureBottleViewModel;

			if(vm != null){
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

		public InventoryVisuallyMeasureWithGesturesPage ()
		{
			var vm = App.InventoryVisuallyMeasureBottleViewModel;
			this.BindingContext = vm;
			InitializeComponent ();
            vm.ResetMarkers = () =>
            {
                if (_measureButtons == null) return;
                foreach (var mb in _measureButtons)
                {
                    mb.SetOff();
                }

                if (stckMeasure != null)
                {
                    StckMeasureSizeChanged(stckMeasure, null);
                }
            };
			vm.MovePreviousAnimation = async () => 
				await stckMain.TranslateTo (stckMain.Width, 0, MiseTheme.SwipeAnimationDuration);

			vm.MoveNextAnimation = async () => 
				await stckMain.TranslateTo (stckMain.Width * -1, 0, MiseTheme.SwipeAnimationDuration);

			vm.ResetViewAnimation = () => {
				stckMain.TranslationX = 0;
				return Task.FromResult (true);
			};

			vm.UpdateShapeOnPage = UpdateBottleShape;

			using(Insights.TrackTime("Time to create measure bottle")){
				UpdateBottleShape (vm.Shape);
			}

            stckMeasure.SizeChanged += StckMeasureSizeChanged;
		}

        private void StckMeasureSizeChanged(object sender, EventArgs e){
            var updated = sender as VisualElement;
            if (updated != null) {
                //we can get our height now!
                //only update if we're a significant amount changed, to avoid constant redraws
                if (updated.Height > 0 && Math.Abs (updated.Height - _oldHeight) > 50) {
                    _oldHeight = updated.Height;

                    var levelHeight = updated.Height / (_shape.WidthsAsPercentageOfHeight.Count + 1);
                    foreach (var c in stckMeasure.Children) {
                        var mb = c as MeasureButton;
                        if (mb != null) {
                            //recalc the height based upon the ratio of the real height to the default
                            mb.HeightRequest = levelHeight;
                            //TODO reset the width as well?
                            mb.WidthRequest = updated.Height * mb.WidthAsPercentageOfContainerHeight;
                        } else {
                            var zero = c as ZeroButton;
                            if(zero != null){
                                zero.HeightRequest = levelHeight;
                                zero.WidthRequest = updated.Width;
                            }
                        }
                    }
                }
            }
        }

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.InventoryVisuallyMeasureBottleViewModel;
			}
		}

		public override String PageName {
			get {
				return "InventoryVisuallyMeasureWithGesturesPage";
			}
		}

		#endregion
		 
		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = ViewModel as InventoryVisuallyMeasureBottleViewModel;
            //TODO get our shape from the line item
			if(vm != null){
				//TODO - do we need to update this shape and recreate controls?
				UpdateBottleShape (vm.Shape);

				btnMoveNext.IsEnabled = vm.MoveNextCommand.CanExecute (null);
			}
		}

		/// <summary>
		/// Given a shape, make a series of buttons that describe the bottle
		/// </summary>
		/// <param name="shape">Shape.</param>
		private void UpdateBottleShape(LiquidContainerShape shape){
			if (_loading) {
				return;
			}
			_loading = true;
			_measureButtons = new List<MeasureButton> ();
			stckMeasure.Children.Clear ();

            if (stckMeasure != null)
            {
                stckMeasure.MinimumHeightRequest = DEFAULT_HEIGHT;
            }
			var levelHeight = DEFAULT_HEIGHT / (shape.WidthsAsPercentageOfHeight.Count + 1);
			var i = 0;

			foreach (var percentage in shape.WidthsAsPercentageOfHeight) {
				var width = DEFAULT_HEIGHT * percentage;
				var button = new MeasureButton (i++, percentage) {
					HeightRequest = levelHeight,
					WidthRequest = width,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
				};
				button.Clicked += MeasureButtonClicked;
				_measureButtons.Add (button);
			}

			//add them top to bottom
			_measureButtons.Reverse();

			foreach (var mb in _measureButtons) {
				stckMeasure.Children.Add (mb);
			}
			var zeroButton = new ZeroButton (levelHeight);
			zeroButton.Clicked += ZeroButton_Clicked;
			stckMeasure.Children.Add (zeroButton);

			_shape = shape;

			_loading = false;
		}

		void ZeroButton_Clicked (object sender, EventArgs e)
		{
			//set our current partial to zero
			var vm = BindingContext as InventoryVisuallyMeasureBottleViewModel;
			if (vm != null) {
				vm.Empty ();
			}
		}

	    private void MeasureButtonClicked(object sender, EventArgs eventArgs)
	    {
            //get our position
            var mb = sender as MeasureButton;
            if (mb != null)
            {
                var position = mb.Position;
                foreach (var mBOther in _measureButtons)
                {
                    if (mBOther.Position <= position)
                    {
                        mBOther.SetOn();
                    }
                    else
                    {
                        mBOther.SetOff();
                    }
                }

                //TODO change this to use the shape
                var percentage = _shape.GetPercentageContainedAt(position);
                var vm = BindingContext as InventoryVisuallyMeasureBottleViewModel;
                if (vm != null)
                {
					vm.CurrentPartial = Math.Round ((decimal)percentage, 1);
                }
            }
	    }

		public class ZeroButton : Xamarin.Forms.Button{
			public ZeroButton(double height)
			{
				//BackgroundColor = Color.Gray;
				HeightRequest = height;
				Text = "Empty";
				HorizontalOptions = LayoutOptions.FillAndExpand;
			}
		}

		public class MeasureButton : Xamarin.Forms.Button{
			public static Color OffColor = Color.Gray;
			public static Color OnColor = Color.Navy;

            public double WidthAsPercentageOfContainerHeight { get; private set; }

			public MeasureButton(int position, double widthAsPercentageOfContainerHeight) : base(){
				BackgroundColor = OffColor;
				On = false;
				Position = position;
			    WidthAsPercentageOfContainerHeight = widthAsPercentageOfContainerHeight;
				base.BorderRadius = 0;
				base.HorizontalOptions = LayoutOptions.Center;
				base.VerticalOptions = LayoutOptions.Center;
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

