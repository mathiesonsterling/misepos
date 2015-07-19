using System;
using System.Linq;
using Gtk;
using Mise.Core.Entities.Menu;
using Mise.Core.Services;
using Mise.Core.Client.ViewModel;

namespace MiseGTKPostTerminal.GTKViews
{
	class OrderOnTab : IGTKTerminalView
	{
		private readonly Window _window;
		private readonly ITerminalApplicationModel _viewModel;
		private readonly DefaultTheme _theme;
		private readonly ILogger _logger;
		private readonly UpdateViewCallback _updateView;
		private Box _orderedItemsContainer;

		public OrderOnTab (DefaultTheme theme, ITerminalApplicationModel viewModel, Window window, ILogger logger, UpdateViewCallback updateView)
		{
			_theme = theme;
			_viewModel = viewModel;
			_window = window;
			_logger = logger;
			_updateView = updateView;
		}

		public TerminalViewTypes Type { get { return TerminalViewTypes.OrderOnTab; } }

		public void LoadView ()
		{
			//we need to get our window size somewhere - we might be able to hard code at some point
			int winHeight, winWidth;
			_window.GetSize (out winWidth, out winHeight);

			//var mainView = new Fixed ();

			//we want our items laid out as part of the 

			var mainTable = new Table(5, 10, true);
			//_window.Add (mainView);
			_window.Add (mainTable);

			//add our main drink layout
			var menuItemRows = new VBox ();
			//mainView.Put (menuItemRows, 0, 0);

			mainTable.Attach(menuItemRows, 0, 7, 0, 4, AttachOptions.Expand , AttachOptions.Expand | AttachOptions.Fill, 0, 0);

			LoadAdminButtons (menuItemRows);

			LoadHotItemButtons (menuItemRows);

			LoadCategories (menuItemRows);

			//add our order panel
			var scroller = new ScrolledWindow ();
			_orderedItemsContainer = new VBox ();
			scroller.Add (_orderedItemsContainer);
			LoadOrderedItems (_orderedItemsContainer);

			var orderX = winWidth/1.5;

			scroller.HeightRequest = winHeight;
			//mainView.Put (scroller, (int)orderX, 0);
			mainTable.Attach(scroller, 7, 10, 0, 1, AttachOptions.Expand, AttachOptions.Expand | AttachOptions.Fill, 2, 2);

			//add our bottom row
			var tabViewReturnButton = new Button {Label = "Tabs"};
            mainTable.Attach(tabViewReturnButton, 0, 5, 4, 5, AttachOptions.Expand | AttachOptions.Fill, AttachOptions.Expand | AttachOptions.Fill, 2, 2);

			var orderItemReturnButton = new Button { Label = "Order" };
			mainTable.Attach (orderItemReturnButton, 5, 10, 4, 5, AttachOptions.Expand | AttachOptions.Fill, AttachOptions.Expand | AttachOptions.Fill, 2, 2);

		}

		private void LoadOrderedItems (Box orderItemContainer)
		{
			//clear the container

			//add our stuff
			foreach (var item in _viewModel.SelectedCheck.OrderItems) {
				var eventBox = new EventBox ();
				var label = new Label { Text = item.MenuItem.Name };
				label.ModifyFg (StateType.Normal, _theme.ButtonTextColor);
				label.ModifyBg (StateType.Normal, _theme.WindowBackground);
				label.ModifyBg (StateType.Active, _theme.WindowBackground);
				//label.Show ();

				eventBox.Add (label);
				eventBox.ButtonReleaseEvent += OrderedItemClicked;
				//eventBox.Show ();
				orderItemContainer.PackStart (eventBox, false, false, 2);
			}
		}

		private void LoadAdminButtons (Box menuItemRowsContainer)
		{
			var adminRow = new HBox ();
			menuItemRowsContainer.PackStart (adminRow, true, true, 2);

			foreach (var admin in _viewModel.AdminItems) {
				var adminButton = new Button { Label = admin.Name };
				adminButton.ModifyBg (StateType.Active, _theme.AdminItemButtonColor);
				adminButton.ModifyBg (StateType.Prelight, _theme.AdminItemButtonColor);
				adminButton.ModifyBg (StateType.Normal, _theme.AdminItemButtonColor);
				adminButton.Clicked += AdminButtonOnClicked;
				adminRow.PackStart (adminButton, true, true, 2);
			}
		}

		private void LoadHotItemButtons (Box menuItemRowsContainer)
		{
			var hotItemRow = new HBox ();
			menuItemRowsContainer.PackStart (hotItemRow, true, true, 2);

			foreach (var admin in _viewModel.HotItemsUnderCurrentCategory) {
				var adminButton = new Button { Label = admin.Name };
				adminButton.ModifyBg (StateType.Active, _theme.CategoryButtonColor);
				adminButton.ModifyBg (StateType.Prelight, _theme.CategoryButtonColor);
				adminButton.ModifyBg (StateType.Normal, _theme.CategoryButtonColor);
				adminButton.Clicked += HotButtonOnClicked;
				hotItemRow.PackStart (adminButton, true, true, 2);
			}
		}

		private void LoadCategories (Box menuItemRowsContainer)
		{
			//make a table to hold everything
			var catTable = new Table (5, 5, true);

			//new HBox();
			menuItemRowsContainer.Add (catTable);

			uint col = 0;
			uint row = 0;
			foreach (var cat in _viewModel.Categories) {
				var catButton = new Button { Label = cat.Name };
				catButton.Clicked += CategoryClicked;
				//check our row and col
				if (col > 5) {
					col = 0;
					row++;
				}
				col++;

				//catTable.PackStart(catButton);
				catTable.Attach (catButton, col, col + 1, row, row + 1);
			}
		}
		#region Events
		private void CategoryClicked (object sender, EventArgs eventArgs)
		{
			var button = sender as Button;
			if (button == null) {
				//error
				_logger.Log ("Unable to get category from control!", LogLevel.Error);
				return;
			}

			//tell the view model what we clicked on, and see what it says to do
			var categoryResult = _viewModel.DrinkCategoryClicked (button.Label);

			//pop the dialog
			var modal = new CategoryModal (_viewModel, _theme);
			modal.Show ();

			switch (categoryResult) {
			case CategoryClickedResultType.ShowSubCategories:
				//modal should hold until it gets an item down to zero
				break;
			case CategoryClickedResultType.OrderedItem:
				//get the item from the modal
				var res = modal.SelectedItem;
				//order the res
				OrderDrink (res);
				break;
			case CategoryClickedResultType.NoItemsFound:
				break;
			}
		}

		private void AdminButtonOnClicked (object sender, EventArgs eventArgs)
		{
			//find our menu items
			var button = sender as Button;
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

		void OrderedItemClicked (object o, ButtonReleaseEventArgs args)
		{
			//get the label
			var eventBox = o as EventBox;

			if (eventBox == null) {
				_logger.Log ("Error while retriving ordered item!", LogLevel.Error);
				return;
			}

			var label = eventBox.Child as Label;
			if (label == null) {
				_logger.Log ("Error while retriving ordered item!", LogLevel.Error);
				return;
			}

			//TODO - we need to know which of our items this is!
			var orderItem = _viewModel.SelectedCheck.OrderItems.FirstOrDefault (oi => oi.MenuItem != null && oi.MenuItem.Name == label.Text);

			_viewModel.OrderedDrinkClicked (orderItem);
		}
		#endregion
		private void OrderDrink (IMenuItem drink)
		{
			_viewModel.DrinkClicked (drink);
			//update with the order
			LoadOrderedItems (_orderedItemsContainer);

			_updateView ();
		}

		internal class MessageBox
		{ 
			public static void Show (Gtk.Window parent_window, DialogFlags flags, MessageType msgtype, ButtonsType btntype, string msg)
			{ 
				MessageDialog md = new MessageDialog (parent_window, flags, msgtype, btntype, msg); 
				md.Run (); 
				md.Destroy (); 
			}

			public static void Show (string msg)
			{ 
				MessageDialog md = new MessageDialog (null, DialogFlags.Modal, MessageType.Other, ButtonsType.Ok, msg); 
				md.Run (); 
				md.Destroy (); 
			}
		}
	}
}
