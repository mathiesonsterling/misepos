using System;
using System.Linq;
using System.Collections.Generic;
using Android.Widget;
using Android.Util;
using Android.Content;

using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Check;

namespace MiseAndroidPOSTerminal.AndroidControls
{		
	internal interface IMenuItemModifierGroupControl{
		IEnumerable<MenuItemModifier> GetSelectedModifiers ();
	}

	sealed class ModifierRadioButton : RadioButton{
		public MenuItemModifier Modifier{ get; private set;}
		public ModifierRadioButton(Context context, MenuItemModifier mod, IMiseAndroidTheme theme) : base(context){
			Modifier = mod;
			Text = mod.Name;
			SetTextColor (theme.DefaultTextColor);
		}
	}

	sealed class ExclusiveMenuItemModifierGroupControl : LinearLayout, IMenuItemModifierGroupControl
	{
		public MenuItemModifierGroup ModGroup{ get; private set; }

		Action<ModifierButton, MenuItemModifier> _modifiersChangedCallback;

		/// <summary>
		/// The radion buttons we have, along with their associated groups
		/// </summary>
		//readonly Dictionary<ModifierRadioButton, LinearLayout> _buttonsAndGroups;
		readonly List<ModifierRadioButton> _allButtons;

		public ExclusiveMenuItemModifierGroupControl(Context context, MenuItemModifierGroup modGroup, 
			IMiseAndroidTheme theme, OrderItem orderItem,
			Action<ModifierButton, MenuItemModifier> modChangedCallback,
			int numItemsPerRow) : base(context){
			Orientation = Orientation.Horizontal;
			_modifiersChangedCallback = modChangedCallback;

			//_buttonsAndGroups = new Dictionary<ModifierRadioButton, RadioGroup> ();
			_allButtons = new List<ModifierRadioButton> ();
			ModGroup = modGroup;

			//create our overall label
			var title = new TextView(context){Text = modGroup.DisplayName};
			title.SetBackgroundColor(theme.WindowBackground);
			title.SetTextColor (theme.CategoryTextColor);
			title.SetTextSize (ComplexUnitType.Pt, theme.OrderItemTextSize);
			//TODO get this from the theme
			title.SetMinimumWidth (theme.OrdersScreenModifierTitleWidth);
			AddView (title);

			var miRows = new LinearLayout (context){ Orientation = Orientation.Vertical };
			AddView (miRows);

			var radioGroup = new LinearLayout (context){Orientation = Orientation.Horizontal};
			radioGroup.SetMinimumHeight (100);
			miRows.AddView (radioGroup);
			var columnIndex = 0;
			foreach (var mod in modGroup.Modifiers) {
				var rb = new ModifierRadioButton (context, mod, theme);

				rb.SetMinWidth (theme.ModifierItemWidth);
				rb.Click += HandleRBClick;

				if (columnIndex >= numItemsPerRow) {
					//make a new row
					radioGroup = new RadioGroup (context){ Orientation = Orientation.Horizontal };
					radioGroup.SetMinimumHeight (100);
					miRows.AddView (radioGroup);
					columnIndex = 0;
				}

				//_buttonsAndGroups.Add (rb, radioGroup);
				_allButtons.Add (rb);
				radioGroup.AddView (rb);
				columnIndex++;
			}
		}
			

		/// <summary>
		/// Event handler for radio buttons, ensures only ONE gets selected here
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleRBClick (object sender, EventArgs e)
		{
			//only this should be selected!
			var thisRB = sender as RadioButton;
			if (thisRB == null) {
				return;
			}
				
			thisRB.Checked = true;
			foreach (var rb in _allButtons) {
				if (rb != thisRB) {
					rb.Checked = false;
				} 
			}
		}

	
		#region MenuItemModifierGroupControl implementation
		public IEnumerable<MenuItemModifier> GetSelectedModifiers ()
		{
			var selectedMods = _allButtons.Where (rb => rb.Checked).Select (rb => rb.Modifier);
			return selectedMods;
		}

		//TODO override the click so we know which item to do
		#endregion
	}
}

