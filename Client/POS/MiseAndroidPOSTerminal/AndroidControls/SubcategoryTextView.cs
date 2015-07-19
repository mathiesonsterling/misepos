using Android.Widget;
using Android.Util;
using Android.Content;

using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Menu;
namespace MiseAndroidPOSTerminal.AndroidControls
{
	sealed class SubcategoryTextView : TextView{
		public MenuItemCategory MenuItemCategory{ get; private set;}
		public SubcategoryTextView(Context context, MenuItemCategory category, IMiseAndroidTheme theme) : base(context){
			SetTextColor(theme.CategoryTextColor);
			SetTextSize(ComplexUnitType.Pt, theme.CategoryTextSize);
			SetPadding(theme.MenuItemButtonPadding, theme.MenuItemButtonPadding,  theme.MenuItemButtonPadding,  theme.MenuItemButtonPadding);
			MenuItemCategory = category;
			Text = " " + category.Name + " >";
		}
	}
}

