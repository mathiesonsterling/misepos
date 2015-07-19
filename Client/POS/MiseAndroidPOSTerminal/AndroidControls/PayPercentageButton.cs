using Android.Content;
using Android.Widget;
using MiseAndroidPOSTerminal.Themes;

namespace MiseAndroidPOSTerminal.AndroidControls
{
	sealed class PayPercentageButton : Button{
		public decimal Percent{ get; private set; }
		public PayPercentageButton(Context context, decimal amt, IMiseAndroidTheme theme) : base(context){
			Percent = amt;
			Text = ((int)(amt * 100)) + "%";
			SetMinimumHeight(theme.PayCheckHeight);
			SetMinimumWidth(theme.PaymentItemButtonWidth);
			SetBackgroundColor(theme.PayPercentageColor);

			theme.ThemeButton(this, theme.MenuItemButtonPadding);
		}
	}
}

