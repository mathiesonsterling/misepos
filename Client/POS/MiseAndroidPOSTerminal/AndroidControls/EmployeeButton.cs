using Android.Content;
using Android.Widget;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.People;
namespace MiseAndroidPOSTerminal .AndroidControls
{
	sealed class EmployeeButton : Button{
		public IEmployee Employee{get;private set;}

		readonly IMiseAndroidTheme _theme;
		public EmployeeButton(Context context, IEmployee emp, IMiseAndroidTheme theme) : base(context){
			Employee = emp;
			Text = emp.DisplayName;
			_theme = theme;
			SetMinHeight (_theme.EmployeeButtonHeight);
			SetMinWidth (_theme.EmployeeButtonWidth);

			_theme.ThemeButton (this);
		}

		public void SetSelected(){
			_theme.MarkAsSelected (this);
		}

		public void SetLastSelected(){
			SetBackgroundColor (_theme.LastSelectedEmployeeButtonColor);
			SetTextColor(_theme.LastSelectedEmployeeButtonTextColor);
			if(Background != null){
				Background.SetLevel (100);
			}
		}

		public void SetUnselected(){
			var color = _theme.GetColorForEmployee (Employee.ID);
			SetBackgroundColor (color);
			SetTextColor (_theme.EmployeeButtonTextColor);
			if(Background != null){
				Background.SetLevel (0);
			}
		}
	}
}

