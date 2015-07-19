using Android.Content;
using Android.Widget;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Check;
namespace MiseAndroidPOSTerminal
{
	sealed class CheckButton : Button
	{
		public ICheck Check{ get;private set;}

		public CheckButton(Context context, IMiseAndroidTheme theme, ICheck check) : base(context){
			Check = check;
			var tabColor = theme.GetCheckButtonColor (Check);
			SetBackgroundColor (tabColor);

			Text = check.DisplayName;
			SetTextColor (theme.DefaultTextColor);

			SetMinHeight (theme.TabButtonHeight);
			SetMinWidth (theme.TabButtonWidth);

			theme.ThemeButton (this);
		}
	}
}

