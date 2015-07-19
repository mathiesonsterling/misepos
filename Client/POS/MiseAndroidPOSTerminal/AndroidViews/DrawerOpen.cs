using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.ValueItems;

namespace MiseAndroidPOSTerminal.AndroidViews
{
	[Activity (Label = "DrawerOpen")]			
	public class DrawerOpen : BasePOSView
	{
		LinearLayout _noSale;
		LinearLayout _change;
		TextView _changeAmt;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

			_noSale = new LinearLayout(this) { Orientation = Orientation.Horizontal };
			_noSale.SetGravity (GravityFlags.Center);
			var nsText = new TextView (this) { Text = "No Sale" };
			nsText.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
			nsText.SetTextSize (Android.Util.ComplexUnitType.Pt, 64);
			nsText.SetTextColor (POSTheme.DefaultTextColor);
			_noSale.AddView (nsText);
			layout.AddView (_noSale);

			_change = new LinearLayout(this) { Orientation = Orientation.Vertical };
			_change.SetGravity (GravityFlags.Center);
			var changeLabel = new TextView(this){Text = "Change"};
			changeLabel.SetTextSize (Android.Util.ComplexUnitType.Pt, 64);
			changeLabel.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
			_change.AddView (changeLabel);
			_changeAmt = new TextView (this);
			_changeAmt.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
			_changeAmt.SetTextSize (Android.Util.ComplexUnitType.Pt, 64);
			_changeAmt.SetTextColor (POSTheme.DefaultTextColor);
			_change.AddView (_changeAmt);
			_change.Visibility = ViewStates.Gone;
			layout.AddView (_change);

			SetContentView (layout);
		}

		protected override void OnResume(){
			base.OnResume ();
			switch (Model.CurrentTerminalViewTypeToDisplay) {
			case TerminalViewTypes.DisplayChange:
				_changeAmt.Text = Model.SelectedCheck.ChangeDue.ToFormattedString ();
				_noSale.Visibility = ViewStates.Gone;
				_change.Visibility = ViewStates.Visible;
				//TODO consider moving this into view model
				Model.SelectedCheck.ChangeDue = Money.None;
				break;
			case TerminalViewTypes.NoSale:
				_noSale.Visibility = ViewStates.Visible;
				_change.Visibility = ViewStates.Gone;
				break;
			default:
				throw new Exception ("Unknown type!");
			}

			//figure out what we're supposed to do
			Model.SetCashDrawerClosedEvent (() => {
				Model.SetCashDrawerClosedEvent(null);
				MoveToCurrentView();
			});

			//open the drawer, we'll be paused till it comes back!
			Model.OpenCashDrawer ();
		}
	}
}

