using Android.Content;
using Android.Widget;
using Mise.Core.ValueItems;
using MiseAndroidPOSTerminal.Themes;

namespace MiseAndroidPOSTerminal.AndroidControls
{
	class PayAmtButton : Button{
		public Money Amt{ get; private set;}
		public PayAmtButton(Context context, Money amt, IMiseAndroidTheme theme) : base(context){
			Amt = amt;
			Text = Amt.ToFormattedString();
			SetMinimumHeight(theme.PayCheckHeight);
			SetMinimumWidth(theme.PaymentItemButtonWidth);
			SetBackgroundColor(theme.PayCheckColor);

			theme.ThemeButton(this, theme.MenuItemButtonPadding);
		}
	}

	sealed class PayExactButton : PayAmtButton{
		public PayExactButton(Context context, Money amt, IMiseAndroidTheme theme) : base(context, amt, theme){
			Text = "EXACT " + Amt.ToFormattedString ();
		}
	}
}

