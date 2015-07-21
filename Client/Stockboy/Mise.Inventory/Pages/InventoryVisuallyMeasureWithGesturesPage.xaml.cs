using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.ValueItems.Inventory;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using XLabs.Platform.Device;
using Xamarin;
namespace Mise.Inventory.Pages
{
	public partial class InventoryVisuallyMeasureWithGesturesPage : ContentPage
	{
		private List<MeasureButton> _measureButtons;
		private double _oldHeight = DEFAULT_HEIGHT;
	    private const double DEFAULT_HEIGHT = 200;
		private bool _loading = false;
	    private LiquidContainerShape _shape = LiquidContainerShape.DefaultBeerBottleShape;
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
            };

			using(Insights.TrackTime("Time to create measure bottle")){
				stckMeasure.Children.Clear ();
				CreateMeasureBottle (stckMeasure);
			}

			stckMeasure.SizeChanged += (sender, e) => {
				var updated = sender as VisualElement;
				if(updated != null){
					//we can get our height now!
					//only update if we're a significant amount changed, to avoid constant redraws
					if(updated.Height > 0 && Math.Abs (updated.Height - _oldHeight) > 50){
						_oldHeight = updated.Height;

						var levelHeight = updated.Height/_shape.WidthsAsPercentageOfHeight.Count;
						foreach(var c in stckMeasure.Children){
							var container = c as AbsoluteLayout;
							if(container != null){
								foreach(var containerChild in container.Children){
									var mb = containerChild as MeasureButton;
									if(mb != null)
									{
                                        //recalc the height based upon the ratio of the real height to the default
									    mb.HeightRequest = levelHeight;
									    //TODO reset the width as well?
										mb.WidthRequest = updated.Height * mb.WidthAsPercentageOfContainerHeight;
									}
								}
							}
						}
					}
				}
			};
		}

		protected override async void OnAppearing ()
		{
			Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "InventoryVisuallyMeasuredImprovedPage"}});

			var vm = BindingContext as InventoryVisuallyMeasureBottleViewModel;
            //TODO get our shape from the line item
			if(vm != null){
				await vm.OnAppearing ();

				//_shape = vm.CurrentLineItem.Container
			}
		}

		private void CreateMeasureBottle(IViewContainer<View> destLayout){
			if(_loading){
				return;
			}
			_loading = true;
			_measureButtons = new List<MeasureButton> ();

			//determine the max height, and make the level hieght off of that

            //TODO get this from the line item
		    var levelHeight = DEFAULT_HEIGHT/_shape.WidthsAsPercentageOfHeight.Count;
			const double defaultWidth = DEFAULT_HEIGHT / 2;
		    var i = 0;
		    foreach (var percentage in _shape.WidthsAsPercentageOfHeight)
		    {
				var width = DEFAULT_HEIGHT * percentage;
		        var button = new MeasureButton(i++, percentage) {HeightRequest = levelHeight, WidthRequest = width};
                button.Clicked += MeasureButtonClicked;
                _measureButtons.Add(button);
		    }

			_measureButtons.Reverse ();

			var centerPoint = defaultWidth / 2;

			var container = new AbsoluteLayout {HeightRequest = DEFAULT_HEIGHT, WidthRequest = defaultWidth/2};
			destLayout.Children.Add (container);

			//add the top empty one
			var addPoint = new Point (centerPoint, 0);

			foreach (var mB in _measureButtons) {
				mB.HorizontalOptions = LayoutOptions.Center;
				addPoint.X = centerPoint - mB.WidthRequest / 2;
				container.Children.Add (mB, addPoint);
				addPoint.Y += levelHeight;
			}

			_loading = false;
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

	    public class MeasureButton : Button{
			public static Color OffColor = Color.Gray;
			public static Color OnColor = Color.Navy;

            public double WidthAsPercentageOfContainerHeight { get; private set; }

			public MeasureButton(int position, double widthAsPercentageOfContainerHeight) : base(){
				BackgroundColor = OffColor;
				On = false;
				Position = position;
			    WidthAsPercentageOfContainerHeight = widthAsPercentageOfContainerHeight;
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

