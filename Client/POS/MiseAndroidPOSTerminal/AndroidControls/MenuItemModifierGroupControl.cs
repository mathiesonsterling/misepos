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
	sealed class MenuItemModifierGroupControl : LinearLayout, IMenuItemModifierGroupControl
	{
		public MenuItemModifierGroup ModGroup{ get; private set; }
		readonly IList<ModifierButton> _buttonList;

		public void NotifyModifierHasChanged (ModifierButton source, MenuItemModifier mod){
			//pass up to our event handler, if any
			if (_modifiersChangedCallback != null) {
				_modifiersChangedCallback (source, mod);
			}
		}

		Action<ModifierButton, MenuItemModifier> _modifiersChangedCallback;

		public MenuItemModifierGroupControl(Context context, MenuItemModifierGroup modGroup, 
			IMiseAndroidTheme theme, OrderItem orderItem,
			Action<ModifierButton, MenuItemModifier> modChangedCallback,
			int numItemsPerRow) : base(context)
		{
			_buttonList = new List<ModifierButton>();
			_modifiersChangedCallback = modChangedCallback;
			Orientation = Orientation.Horizontal;
			ModGroup = modGroup;

			//create the text label
			var title = new TextView(context){Text = ModGroup.DisplayName};
			title.SetBackgroundColor(theme.WindowBackground);
			title.SetTextColor (theme.CategoryTextColor);
			title.SetTextSize (ComplexUnitType.Pt, theme.OrderItemTextSize);
			//TODO get this from the theme
			title.SetMinimumWidth (theme.OrdersScreenModifierTitleWidth);
			AddView (title);

			var allRows = new LinearLayout (context){ Orientation = Orientation.Vertical };
			AddView (allRows);
			//now make a horizontal layout for each item in the group
			var row = theme.CreateRowForGrid(context, theme.MenuItemButtonPadding);
			row.SetMinimumHeight (100);
			//add our title
			allRows.AddView (row);

			var numInRow = 0;
			foreach(var mod in modGroup.Modifiers){
				var modButton = new ModifierButton(context, mod, theme, this); 
				modButton.Click += (sender, e) => {
					var clickedButton = sender as ModifierButton;
					if (clickedButton != null) {
						if (clickedButton.SelectedMod && (ModGroup.Required == false)) {
							//if we're already clicked, deselect!
							clickedButton.Deselect ();
						} else {
							//if we're exclusive, select just one
							if (ModGroup.Exclusive) {
								foreach (var button in _buttonList) {
									if (button == clickedButton) {
										button.Select ();
									} else { 
										button.Deselect ();
									}
								}
							} else {
								if (clickedButton.SelectedMod) {
									clickedButton.Deselect ();
								} else {
									clickedButton.Select ();
								}
							}
						}
					}
				};//end click

				if(numInRow >= numItemsPerRow)
				{
					row = theme.CreateRowForGrid(context, theme.MenuItemButtonPadding);
					row.SetMinimumHeight (100);
					allRows.AddView(row);
					numInRow = 0;
				}
				numInRow++;
				row.AddView (modButton);
				_buttonList.Add(modButton);
			}

			//now select the ones in our order item, if any!
			if(orderItem.Modifiers.Any())
			{
				var modNames = orderItem.Modifiers.Select(m => m.Name).ToList();
				foreach(var button in _buttonList){
					if(modNames.Contains(button.Text)){
						button.Select();
					}
					else{
						button.Deselect();
					}
				}
			}
			else
			{
				//if we have defaults, select them!
				if(ModGroup.DefaultItemID.HasValue) {
					foreach(var button in _buttonList) {
						if(ModGroup.DefaultItemID.Value.ToString() == button.Text){
							button.Select();
						}
						else{
							button.Deselect();
						}
					}
				}
			}
		}

		public IEnumerable<MenuItemModifier> GetSelectedModifiers(){
			return _buttonList.Where (b => b.SelectedMod).Select (b => b.Mod);
		}
	}
}

