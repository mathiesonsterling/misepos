using System;
using System.Linq;
using Gtk;
using Mise.Core.Entities.Menu;
using Mise.Core.Client.ViewModel;
using Mise.Core.Services;
namespace MiseGTKPostTerminal.GTKViews
{
	public class OrderOnTabFixed : IGTKTerminalView
	{
		private readonly Window _window;
		private readonly ITerminalApplicationModel _viewModel;
		private readonly DefaultTheme _theme;
		private readonly ILogger _logger;
		private readonly UpdateViewCallback _updateView;

		private Fixed _itemPanel;
		private Box _orderedItemsContainer;
		private Fixed _menuItemsPanel;
		private Box _categoryPanel;
		public OrderOnTabFixed(DefaultTheme theme, ITerminalApplicationModel viewModel, Window window, ILogger logger, UpdateViewCallback updateView)
		{
			_theme = theme;
			_viewModel = viewModel;
			_window = window;
			_logger = logger;
			_updateView = updateView;
		}

		#region IGTKTerminalView implementation



		public void LoadView ()
		{
			var mainView = new Fixed ();

			//make our subviews
			//ordered items panel
			_orderedItemsContainer = new VBox {WidthRequest = _theme.OrderScreenOrderPanelWidth};

		    LoadOrderedItems (_orderedItemsContainer);
			mainView.Put (_orderedItemsContainer, 0, 0);

			//menu items panel
			_itemPanel = new Fixed ();
			mainView.Put (_itemPanel, _theme.OrderScreenOrderPanelWidth, 0);

			LoadAdminOrderButtons (_itemPanel, 0);

			//LoadHotMenuItems (_itemPanel, _theme.MenuItemButtonHeight + _theme.ButtonPadding);

			_menuItemsPanel = new Fixed ();
			mainView.Put (_menuItemsPanel, _theme.OrderScreenOrderPanelWidth, (_theme.MenuItemButtonHeight + _theme.ButtonPadding));
			LoadMenuItemButtons (_menuItemsPanel, 0);

			 //categories pattern
			_categoryPanel = new VBox ();
		
			mainView.Put (_categoryPanel, _theme.OrderScreenOrderPanelWidth + _theme.OrderScreenMenuItemsPanelWidth,0);

			//load the categories
			LoadCategories (_categoryPanel);

			//add our cash, tab, credit, and cancel panel
			var cmdPanel = new Fixed ();
			LoadCommandPanel (cmdPanel);

			mainView.Put (cmdPanel, 0, _theme.WindowHeight - (_theme.MenuItemButtonWidth * 2));

			_window.Add (mainView);
		}

		public TerminalViewTypes Type {
			get {
				return TerminalViewTypes.OrderOnTab;
			}
		}

		#endregion

		#region Event Handlers
		private void AdminButtonOnClicked (object sender, EventArgs eventArgs)
		{
			//find our menu items
			var button = sender as Button;
            if (button == null)
            {
                return;
            }
			var adminItem = _viewModel.AdminItems.FirstOrDefault (a => a.Name == button.Label);

			//make the order
			OrderDrink (adminItem);
		}

		private void HotButtonOnClicked (object sender, EventArgs eventArgs)
		{
			//find our menu items
			var button = sender as Button;
			var hotItem = _viewModel.HotItems.FirstOrDefault (a => a.Name == button.Label);

			//make the order
			OrderDrink (hotItem);
		}

		/// <summary>
		/// Event handler for when a category is clicked
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void CategoryClicked (object sender, EventArgs e)
		{
			//get the cateogry
			var button = sender as Button;
            if (button == null)
            {
                return;
            }
			var category = _viewModel.Categories.FirstOrDefault (c => c.Name == button.Label);
            if (category == null)
            {
                return;
            }
			//if we're clicking on the previous selection, turn it off
			if (_viewModel.SelectedCategory != null) {
				if (category.Name == _viewModel.SelectedCategory.Name) {
					_viewModel.SelectedCategory = null;
					category = null;
					button.ModifyBg (StateType.Normal, _theme.CategoryButtonColor);
				}
			} else {
				button.ModifyBg (StateType.Normal, _theme.SelectedEmployeeButtonColor);
			}

			if (category != null) {
				_viewModel.SelectedCategory = category;
				button.ModifyFg (StateType.Normal, _theme.SelectedEmployeeButtonColor);
			}

			//reload the menu items
			LoadMenuItemButtons (_menuItemsPanel, 0);

			//redraw
			_menuItemsPanel.ShowAll ();
			_categoryPanel.ShowAll ();
		}

		void MenuItemClicked (object sender, EventArgs e)
		{
			var button = sender as Button;
			var item = _viewModel.MenuItemsUnderCurrentCategory.FirstOrDefault (m => m.Name == button.Label);

			if (item != null) {
				OrderDrink (item);
			}
		}



		void OrderedItemPressed (object o, ButtonPressEventArgs args)
		{
			//get the ordered item we're talking about
			var orderEB = o as EventBox;
			if (orderEB == null) {
				//log the error
				return;
			}

			var btn = orderEB.Child as OrderItemControl;

			if (btn == null) {
				//log mismatch
			}
			var orderItem = btn.OrderItem;
			if (orderItem != null && orderItem.MenuItem != null && orderItem.MenuItem.PossibleModifiers != null && orderItem.MenuItem.PossibleModifiers.Any ()) {
				//launch our dialog here
				var dialog = GetDialog ();


				//add our menu item modifiers for this object
				var mods = new VBox ();
				dialog.Add (mods);


				foreach (var mod in orderItem.MenuItem.PossibleModifiers) {
					var modButton = new Button (mod.Name);
					modButton.WidthRequest = _theme.MenuItemButtonWidth;
					modButton.ModifyFg (StateType.Normal,_theme.AdminItemButtonColor);
					mods.Add (modButton);
				}
				dialog.Show ();
			}
		}
		#endregion

		private void OrderDrink (IMenuItem drink)
		{
			_viewModel.DrinkClicked (drink);
			//update with the order panel
			LoadOrderedItems (_orderedItemsContainer);

			_updateView ();
		}

		void LoadCommandPanel (Fixed cmdPanel)
		{
			cmdPanel.WidthRequest = _theme.OrderScreenOrderPanelWidth;
			cmdPanel.HeightRequest = _theme.MenuItemButtonHeight * 2;

			var tabButton = new Button ("Tab")
			    {
			        WidthRequest = _theme.OrderScreenOrderPanelWidth/2,
			        HeightRequest = _theme.MenuItemButtonHeight
			    };
		    tabButton.Clicked += (sender, args) =>
		        {
		            _viewModel.SendCurrentTab();
		            _updateView();
		        };
			cmdPanel.Put (tabButton, 0, 0);

			var clear = new Button ("Clear")
			    {
			        WidthRequest = _theme.OrderScreenOrderPanelWidth/2,
			        HeightRequest = _theme.MenuItemButtonHeight
			    };

		    cmdPanel.Put (clear, _theme.OrderScreenOrderPanelWidth / 2, 0);

			var cashButton = new Button ("Cash")
			    {
			        WidthRequest = _theme.OrderScreenOrderPanelWidth/2,
			        HeightRequest = _theme.MenuItemButtonHeight,
			    };
		    cashButton.Clicked += (sender, args) => ShowCashTotal();
			cashButton.ModifyBg (StateType.Normal, _theme.CashButtonColor);
			cmdPanel.Put (cashButton, 0, _theme.MenuItemButtonHeight);
		}

		private Dialog GetDialog ()
		{
			var dialog = new Dialog {WidthRequest = _theme.DialogWidth, HeightRequest = _theme.DialogHeight};
		    dialog.SetPosition (WindowPosition.CenterOnParent);
			dialog.ModifyBg (StateType.Active, _theme.WindowBackground);
			dialog.ModifyBg (StateType.Normal, _theme.WindowBackground);
			return dialog;
		}

		private void ShowCashTotal()
		{
		    //switch to the total screen, giving it the callback to go to our main screen after
			_viewModel.CloseOrderButtonClicked();

			_updateView();

		}

		private void LoadOrderedItems(Box container)
		{
			//clear the box first

			//add a scrolling box

			//add the items
			foreach (var item in _viewModel.SelectedCheck.OrderItems) {
				var eventBox = new EventBox ();
				eventBox.ButtonPressEvent += OrderedItemPressed;
				eventBox.ModifyBg (StateType.Normal, _theme.WindowBackground);
				eventBox.Realize ();

				eventBox.HeightRequest += _theme.MenuItemButtonHeight/4;

				var label = new OrderItemControl (item);
				eventBox.Add (label);
				label.ModifyFg (StateType.Normal, _theme.OrderItemColor);
				container.PackStart (eventBox);
			}
		}

		private void LoadCategories(Box categoryBox)
		{
			foreach (var cat in _viewModel.Categories) {
				//TODO change this to a different button shape
				var button = CreateMenuItemButton (cat.Name);
				button.ModifyBg (StateType.Normal, _theme.CategoryButtonColor);
				if (_viewModel.SelectedCategory != null) {
					if (cat.Name == _viewModel.SelectedCategory.Name) {
						button.ModifyFg (StateType.Normal, _theme.SelectedEmployeeButtonColor);
					}
				}
				button.Clicked += CategoryClicked;
				categoryBox.PackStart (button);
			}
		}


		private void LoadMenuItemButtons(Fixed orderPanel, int startingYPos)
		{
			foreach(Widget item in orderPanel.AllChildren)
			{
				orderPanel.Remove (item);
			}
			int xPos = 0;
			int yPos = startingYPos;

			foreach (var item in _viewModel.MenuItemsUnderCurrentCategory) {
				if (xPos > (_theme.OrderScreenMenuItemsPanelWidth - _theme.MenuItemButtonWidth)) {
					xPos = 0;
					yPos += (_theme.MenuItemButtonHeight + _theme.ButtonPadding);
				}

				var button = CreateMenuItemButton (item.Name);

				//assign the event handler here
			   button.Clicked += MenuItemClicked;

				orderPanel.Put (button, xPos, yPos);
				xPos += (_theme.MenuItemButtonWidth + _theme.ButtonPadding);
			}
			
		}

		private void LoadAdminOrderButtons(Fixed orderPanel, int yPos)
		{
			//do a row of admin items
		    int xPos = 0;
			foreach (var adminItem in _viewModel.AdminItems) 
			{
				var button = CreateMenuItemButton (adminItem.Name);
				button.ModifyBg (StateType.Normal, _theme.AdminItemButtonColor);
				button.Clicked += AdminButtonOnClicked;

				orderPanel.Put (button, xPos, yPos);
				xPos += (_theme.MenuItemButtonWidth + _theme.ButtonPadding);
			}
		}


		Button CreateMenuItemButton (string text)
		{
			var itemBut = new Button
			    {
			        Label = text,
			        WidthRequest = _theme.MenuItemButtonWidth,
			        HeightRequest = _theme.MenuItemButtonHeight
			    };
		    return itemBut;
		}

	}
}

