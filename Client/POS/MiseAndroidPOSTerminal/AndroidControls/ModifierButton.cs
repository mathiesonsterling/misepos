using Android.Widget;
using Android.Content;

using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Menu;


namespace MiseAndroidPOSTerminal.AndroidControls
{
	sealed class ModifierButton : CheckBox
	{
		readonly IMiseAndroidTheme _theme;
		/// <summary>
		/// Parent control of this
		/// </summary>
		MenuItemModifierGroupControl _parent;
		/// <summary>
		/// If true, this mod is selected
		/// </summary>
		/// <value><c>true</c> if selected mod; otherwise, <c>false</c>.</value>
		public bool SelectedMod{ get; private set; }
		public MenuItemModifier Mod{ get; private set; }

		public ModifierButton(Context context, MenuItemModifier mod, IMiseAndroidTheme theme, 
			MenuItemModifierGroupControl parent) : base(context){
			Mod = mod;
			_theme = theme;
			_parent = parent;
			Text = mod.Name;
			SetTextColor (theme.DefaultTextColor);
			SetMinimumWidth (_theme.ModifierItemWidth);
	
			SetBackgroundColor(_theme.WindowBackground);
		}

		/// <summary>
		/// Triggers an event in the parent since we changed
		/// </summary>
		void NotifyModifierChanged(){
			_parent.NotifyModifierHasChanged(this, Mod);
		}

		public void Select(){
			if(SelectedMod == false)
			{
				SelectedMod = true;
				Checked = true;
				NotifyModifierChanged ();
			}
		}

		public void Deselect(){
			if (SelectedMod) {
				SelectedMod = false;
				Checked = false;
				NotifyModifierChanged ();
			}
		}
	}
}

