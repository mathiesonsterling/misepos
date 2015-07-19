using Android.Content;
using Android.Widget;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Payments;

namespace MiseAndroidPOSTerminal.AndroidControls
{
	sealed class DiscountButton : Button{
		public IDiscount Discount{ get; private set;}
		readonly IMiseAndroidTheme _theme;
		public bool SelectedDiscount{ get; private set;}
		public DiscountButton(Context context, IDiscount discount, IMiseAndroidTheme theme) : base(context){
			Discount = discount;
			_theme = theme;
			_theme.ThemeButton (this);
			Text = discount.Name;
			SetMinimumHeight(theme.PayCheckHeight);
			SetMinimumWidth(theme.PaymentItemButtonWidth);
		}

		public void Select(){
			SelectedDiscount = true; 
			_theme.MarkAsSelected (this);
		}

		public void Deselect(){
			SelectedDiscount = false;
			SetTextColor (_theme.DefaultTextColor);
			SetBackgroundColor (Discount.AddsMoney?_theme.GratuityColor:_theme.DiscountColor);
		}
	}
}

