using System.Linq;
using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Views.InputMethods;
using Android.Util;
using Android.Graphics;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Services;
using Mise.Core.ValueItems;

using MiseAndroidPOSTerminal.AndroidControls;

using MisePOSTerminal.ViewModels;

namespace MiseAndroidPOSTerminal.AndroidViews
{

	[Activity(Label = "Order On Tab New", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]		
	public class OrderOnTabNew : BasePOSViewWithOrderItemList
	{


		#region Private Fields

		LinearLayout _hotItemsContainer;
		LinearLayout _menuItemsGrid;
		LinearLayout _modifiersGrid;
		LinearLayout _sentItemsModifiers;
		LinearLayout _categoryRow;

		ScrollView _categoryScroller;

		LinearLayout _categoriesCol;
		Button _selectedCategoryButton;

		Button _selectedOrderItemView;

		//we'll change the text of this depending if the item is added or not
		Button _voidDeleteButton;
		Button _wasteButton;
		TextView _currentCategoryDisplay;

		LinearLayout _ordersRightSide;
		LinearLayout _modifiersRightSide;
		LinearLayout _bottomBar;

		Button compItem;
		Button undoCompItem;
		/// <summary>
		/// The updated memo text for our selected order item
		/// </summary>
		string _updatedMemoText;

		bool _doWaste;

		OrderOnCheckViewModel _vm;
		#endregion


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			_vm = new OrderOnCheckViewModel(Logger, Model);
			_vm.OnMoveToView += calling => MoveToView (calling.DestinationView);
			_vm.OnLoadCategories += calling => LoadCategories(_categoriesCol);
			_vm.OnLoadMenuItems += calling => LoadMenuItems(_menuItemsGrid);
			_vm.OnLoadHotItems += calling => LoadHotItems();
			_vm.OnLoadOrderItems += calling => PopulateOrderItems(OrderItemsCol);
			_vm.OnLoadModifiers += calling => {
				LoadModifiers(_modifiersGrid);
				ChangeToModifierMode();
			};

			_vm.OnModifierModeChanged += (calling) => {

				var vm = calling as OrderOnCheckViewModel;
				if(vm != null){
					if(vm.ModifierActive){
						ChangeToModifierMode();
					} else {
						ChangeToOrderMode();
					}
				}
			};
			_vm.OnUpdateCommands += UpdateCommands;

			var mainLayout = new LinearLayout(this){ Orientation = Orientation.Vertical };

			var topBar = new LinearLayout(this){ Orientation = Orientation.Horizontal };
			mainLayout.AddView(topBar);

			//Create the user interface in code
			var layout = new LinearLayout(this) { Orientation = Orientation.Horizontal };
			mainLayout.AddView(layout);

			//main sections
			var leftSideCol = new LinearLayout(this) { Orientation = Orientation.Vertical };
			layout.AddView(leftSideCol);

			var oiWrapper = CreateOrderItemArea();
			leftSideCol.AddView(oiWrapper);

			_ordersRightSide = new LinearLayout(this){ Orientation = Orientation.Horizontal };

			_ordersRightSide.SetBackgroundColor(POSTheme.WindowBackground);
			layout.AddView(_ordersRightSide);

			CreateMenuItemStructure();

			_modifiersRightSide = new LinearLayout(this) {
				Orientation = Orientation.Horizontal
			};
			layout.AddView(_modifiersRightSide);
			CreateModifiersStructure();

			ChangeToOrderMode();

			//add our bottom bar LAST, so it's always on top
			_bottomBar = CreateCommandBar();

			mainLayout.AddView(_bottomBar);
			//add us out
			SetContentView(mainLayout);
		}

		Button fastCashButton;
		Button sendButton;
		Button cancelOrderButton;
		Button closeOrderButton;
		Button extrasButton;




		LinearLayout CreateCommandBar()
		{
			var bottomBar = new LinearLayout(this) {
				Orientation = Orientation.Horizontal,
			};
			bottomBar.SetMinimumHeight(POSTheme.OrdersScreenCommandBarHeight);
			sendButton = POSTheme.CreateButton(this, "Send", POSTheme.SendCheckButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			sendButton.Click += (sender, e) => _vm.Send.Execute(null);
			bottomBar.AddView(sendButton);

			cancelOrderButton = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			cancelOrderButton.Click += (sender, e) => _vm.Cancel.Execute(null);
			bottomBar.AddView(cancelOrderButton);

			//add our close button
			closeOrderButton = POSTheme.CreateButton(this, "Close", POSTheme.CloseOrderButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			closeOrderButton.Click += (sender, e) => _vm.Close.Execute(null);
			bottomBar.AddView(closeOrderButton);

			fastCashButton = POSTheme.CreateButton(this, "Fast Cash", POSTheme.CashButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			fastCashButton.Click += (sender, e) => _vm.FastCash.Execute(null);
			bottomBar.AddView(fastCashButton);

			extrasButton = POSTheme.CreateButton(this, "+", POSTheme.ExtrasColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			extrasButton.Click += (sender, e) => _vm.Extra.Execute(null);
			bottomBar.AddView(extrasButton);


			UpdateCommands(_vm);
			return bottomBar;
		}

		void BringCommandsToFront()
		{
			if (_bottomBar != null) {
				_bottomBar.BringToFront();
			}
		}
			
		void UpdateCommands(BaseViewModel vm){
			var calling = vm as OrderOnCheckViewModel;
			POSTheme.SetButtonEnabled (fastCashButton, calling.FastCash.CanExecute(null));
			POSTheme.SetButtonEnabled (sendButton, calling.Send.CanExecute (null));
			POSTheme.SetButtonEnabled (cancelOrderButton, calling.Cancel.CanExecute(null));
			POSTheme.SetButtonEnabled (closeOrderButton, calling.Close.CanExecute(null));
			POSTheme.SetButtonEnabled (extrasButton, calling.Extra.CanExecute(null));

			_homeButton.Visibility = calling.CategoryIsShowing ? ViewStates.Visible : ViewStates.Invisible;
			_homeButton.Enabled = calling.CategoryIsShowing;

			//check home
			BringCommandsToFront();
		}

		void CreateMenuItemStructure()
		{
			var menuItemsCol = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			menuItemsCol.SetPadding(POSTheme.MenuItemButtonPadding, POSTheme.MenuItemButtonPadding, POSTheme.MenuItemButtonPadding, POSTheme.MenuItemButtonPadding);
			menuItemsCol.SetMinimumWidth(POSTheme.OrdersScreenCenterColumnWidth);
			//category display
			_categoryRow = POSTheme.CreateRowForGrid(this);
			_categoryRow.SetMinimumWidth(POSTheme.OrdersScreenCenterColumnWidth);
			menuItemsCol.AddView(_categoryRow);

			var prevSibling = new TextView(this){ Text = "< " };
			prevSibling.SetTextSize(ComplexUnitType.Pt, POSTheme.CategoryTextSize);
			prevSibling.SetTextColor(POSTheme.DefaultTextColor);
			prevSibling.Click += (sender, e) => {
				if (_vm.CategoryUpClicked.CanExecute(null)) {
					_vm.CategoryUpClicked.Execute(null);
				}
			};
			//TODO add the click
			_categoryRow.AddView(prevSibling);

			_currentCategoryDisplay = new TextView(this) {
				Text = ""
			};

			_currentCategoryDisplay.SetTextSize(ComplexUnitType.Pt, POSTheme.CategoryTextSize);
			_currentCategoryDisplay.SetTextColor(POSTheme.DefaultTextColor);
			_currentCategoryDisplay.SetMinimumHeight(POSTheme.CategoryButtonHeight);
			_currentCategoryDisplay.Gravity = GravityFlags.CenterHorizontal;
			_currentCategoryDisplay.Click += (sender, e) => {
				if (_vm.CategoryUpClicked.CanExecute(null)) {
					_vm.CategoryUpClicked.Execute(null);
				}
			};
			_categoryRow.AddView(_currentCategoryDisplay);
			_categoryRow.Visibility = ViewStates.Invisible;


			_ordersRightSide.AddView(menuItemsCol);
			var centerScrollHolder = new LinearLayout(this){ DividerPadding = 0 };
			centerScrollHolder.SetPadding(0, 0, 0, 0);
			var centerScrollerLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, POSTheme.OrderScreenCenterScrollerHeight);
			centerScrollerLayoutParams.BottomMargin = 0;
			centerScrollHolder.LayoutParameters = centerScrollerLayoutParams;

			var menuCenterScroller = new ScrollView(this);
			menuCenterScroller.SetPadding(0, 0, 0, 0);
			centerScrollHolder.AddView(menuCenterScroller);

			var menuCenterLayout = new LinearLayout(this) {
				Orientation = Orientation.Vertical,
			};

			menuCenterScroller.AddView(menuCenterLayout);

			//menuItemsCol.AddView (menuCenterScroller);
			menuItemsCol.AddView(centerScrollHolder);
			var realRow = new LinearLayout(this){ Orientation = Orientation.Horizontal };
			menuCenterLayout.AddView(realRow);
			;
			_hotItemsContainer = realRow;
			_hotItemsContainer.SetMinimumHeight(POSTheme.MenuItemButtonHeight);
			_menuItemsGrid = new LinearLayout(this) {
				Orientation = Orientation.Vertical 
			};
			menuCenterLayout.AddView(_menuItemsGrid);
			var menuRightSideCol = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			menuRightSideCol.SetBackgroundColor(POSTheme.SubCategoryColor);
			menuRightSideCol.SetGravity(GravityFlags.Center);
			menuRightSideCol.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			_ordersRightSide.AddView(menuRightSideCol);


			var categoryMenu = CreateCategoryMenu();
			menuRightSideCol.AddView(categoryMenu);
			_categoryScroller = new ScrollView(this);
			var categoryWrapper = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			var wrapperParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, POSTheme.CategoryColumnHeight);
			categoryWrapper.LayoutParameters = wrapperParams;
			categoryWrapper.AddView(_categoryScroller);
			menuRightSideCol.AddView(categoryWrapper);
			_categoriesCol = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			_categoriesCol.SetMinimumHeight(POSTheme.CategoryColumnHeight);

			LoadCategories(_categoriesCol);
			_categoryScroller.AddView(_categoriesCol);
			//var divider = new Space (this);
			//divider.SetMinimumHeight (POSTheme.OrderCommandDividerHeight);

			//menuRightSideCol.AddView (divider);

		}

		void CreateModifiersStructure()
		{
			var modifiersCenterCol = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			modifiersCenterCol.SetMinimumWidth(POSTheme.OrdersScreenCenterColumnWidth);
			_modifiersRightSide.AddView(modifiersCenterCol);
			var modifiersCenterScroller = new ScrollView(this);
			var modifiersCenterLayout = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			modifiersCenterScroller.AddView(modifiersCenterLayout);
			_modifiersGrid = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			modifiersCenterCol.AddView(_modifiersGrid);

			_sentItemsModifiers = POSTheme.CreateRowForGrid(this);
			_sentItemsModifiers.Visibility = ViewStates.Gone;
			modifiersCenterCol.AddView(_sentItemsModifiers);

			#region sent items
			var reorderButton = POSTheme.CreateButton(this, "Reorder", POSTheme.AddTabButtonColor);
			reorderButton.SetMinimumHeight(POSTheme.CompItemButtonHeight);
			reorderButton.SetMinimumWidth(POSTheme.CompItemButtonWidth);
			reorderButton.Click += (sender, e) => {
				var oi = Model.ReorderSelectedOrderItem();
				if (oi != null) {
					PopulateOrderItems(OrderItemsCol);
					ChangeToOrderMode();
				}
			};
			_sentItemsModifiers.AddView(reorderButton);

			compItem = POSTheme.CreateButton(this, "Comp Item", POSTheme.CompColor);
			compItem.SetMinimumHeight(POSTheme.CompItemButtonHeight);
			compItem.SetMinimumWidth(POSTheme.CompItemButtonWidth);
			compItem.Click += (sender, e) => {
				if (_vm.CompSelectedOrderItem.CanExecute(null)) {
					_vm.CompSelectedOrderItem.Execute(null);
				}
			};
			POSTheme.SetButtonEnabled(compItem, _vm.CompSelectedOrderItem.CanExecute(null));
			_sentItemsModifiers.AddView(compItem);

			undoCompItem = POSTheme.CreateButton(this, "Undo Comp", POSTheme.CancelButtonColor);
			undoCompItem.SetMinimumHeight(POSTheme.CompItemButtonHeight);
			undoCompItem.SetMinimumWidth(POSTheme.CompItemButtonWidth);
			undoCompItem.Click += (sender, e) => {
				if (_vm.UndoCompedSelectedOrderItem.CanExecute(null)) {
					_vm.UndoCompedSelectedOrderItem.Execute(null);
				}
			};
			POSTheme.SetButtonEnabled(undoCompItem, _vm.UndoCompedSelectedOrderItem.CanExecute(null));
			_sentItemsModifiers.AddView(undoCompItem);
			#endregion

			var modifiersRightSideCol = new LinearLayout(this) {
				Orientation = Orientation.Vertical
			};
			_modifiersRightSide.AddView(modifiersRightSideCol);

			var modifiersCommandsCol = POSTheme.CreateColumn(this);
			modifiersCommandsCol.SetMinimumHeight(POSTheme.CategoryColumnHeight);
			modifiersCommandsCol.SetMinimumWidth(POSTheme.CategoryButtonWidth);

			modifiersRightSideCol.AddView(modifiersCommandsCol);
			var okBut = POSTheme.CreateButton(this, "OK", POSTheme.OKButtonColor);
			okBut.SetMinHeight(POSTheme.OrdersScreenCommandBarHeight);
			okBut.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			var needsPopulate = false;
			okBut.Click += (sender, e) => {
				try {
					//get all the modifications
					if (_modGroups != null && _modGroups.Any()) {
						var mods = _modGroups.SelectMany(mg => mg.GetSelectedModifiers()).ToList();
						var didMod = false;
						Model.SetMemoOnSelectedOrderItem(_updatedMemoText);
						didMod = Model.ModifySelectedOrderItem(mods);
						if (didMod) {
							Model.OrderItemOrderingCompleted(_vm.SelectedOrderItem);
							//update the order items panel
							needsPopulate = true;
						} else {
							return;
						}
					}
					if (needsPopulate) {
						PopulateOrderItems(OrderItemsCol);
					}
					ChangeToOrderMode();
				} catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			};
			modifiersCommandsCol.AddView(okBut);
			var cancelBut = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);
			cancelBut.SetMinHeight(POSTheme.OrdersScreenCommandBarHeight);
			cancelBut.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			cancelBut.Click += (sender, e) => {
				Model.CancelModificationOnSelectedOrderItem();
				PopulateOrderItems(OrderItemsCol);
				ChangeToOrderMode();
			};
			modifiersCommandsCol.AddView(cancelBut);
			_voidDeleteButton = POSTheme.CreateButton(this, "Void", POSTheme.VoidItemBackgroundColor);
			var _managerCode = new EditText(this) {
				InputType = InputTypes.ClassNumber | InputTypes.NumberVariationPassword
			};
			var _cancelVoid = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);

			_voidDeleteButton.Click += (sender, e) => {
				if (_vm.SelectedOrderItem != null) {
					if (_vm.SelectedOrderItem.Status == OrderItemStatus.Added) {
						_vm.DeleteOrderItem.Execute(null);
						return;
					} else {
						_doWaste = false;
						//get the manager code here
						_managerCode.Visibility = ViewStates.Visible;
						_cancelVoid.Visibility = ViewStates.Visible;
						okBut.Visibility = ViewStates.Gone;
						cancelBut.Visibility = ViewStates.Gone;
						_voidDeleteButton.Visibility = ViewStates.Gone;
						_wasteButton.Visibility = ViewStates.Gone;
						_modifiersGrid.Visibility = ViewStates.Gone;
						_managerCode.RequestFocus();
						var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
						inputMgr.ShowSoftInput(_managerCode, ShowFlags.Forced);
					}
				}
			};
			_voidDeleteButton.SetMinHeight(POSTheme.OrdersScreenCommandBarHeight);
			_voidDeleteButton.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			modifiersCommandsCol.AddView(_voidDeleteButton);

			_wasteButton = POSTheme.CreateButton(this, "Waste", POSTheme.VoidItemBackgroundColor);
			_wasteButton.SetMinHeight(POSTheme.OrdersScreenCommandBarHeight);
			_wasteButton.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			_wasteButton.Click += (sender, e) => {
				if (_vm.SelectedOrderItem.Status == OrderItemStatus.Sent) {
					_doWaste = true;
					//get the manager code here
					_managerCode.Visibility = ViewStates.Visible;
					_cancelVoid.Visibility = ViewStates.Visible;
					okBut.Visibility = ViewStates.Gone;
					cancelBut.Visibility = ViewStates.Gone;
					_voidDeleteButton.Visibility = ViewStates.Gone;
					_wasteButton.Visibility = ViewStates.Gone;
					_modifiersGrid.Visibility = ViewStates.Gone;
					_managerCode.RequestFocus();
					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.ShowSoftInput(_managerCode, ShowFlags.Forced);
				}
			};
			modifiersCommandsCol.AddView(_wasteButton);

			_managerCode.ImeOptions = ImeAction.Done;
			_managerCode.Visibility = ViewStates.Gone;
			_managerCode.Hint = "passcode";
			_managerCode.EditorAction += (sender, e) => {
				if (string.IsNullOrEmpty(_managerCode.Text)) {
					return;
				}
				var Count = Model.HotItemsUnderCurrentCategory.Count();
				var didVoid = Model.VoidSelectedOrderItem(_managerCode.Text, string.Empty, _doWaste);
				if (didVoid) {
					if (Count == 1) {
						POSTheme.MarkButtonAsDisabled(fastCashButton);
						POSTheme.MarkButtonAsDisabled(closeOrderButton);
					}
					PopulateOrderItems(OrderItemsCol);
					_managerCode.Visibility = ViewStates.Gone;
					_cancelVoid.Visibility = ViewStates.Gone;

					okBut.Visibility = ViewStates.Visible;
					cancelBut.Visibility = ViewStates.Visible;
					_voidDeleteButton.Visibility = ViewStates.Visible;
					_wasteButton.Visibility = ViewStates.Visible;
					_modifiersGrid.Visibility = ViewStates.Visible;

					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.HideSoftInputFromWindow(_managerCode.WindowToken, HideSoftInputFlags.None);
					//we should also be out of modifiers, since our item doesn't exist anymore
					ChangeToOrderMode();
				}
				_managerCode.Text = string.Empty;
			};
			_managerCode.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			modifiersCommandsCol.AddView(_managerCode);

			_cancelVoid.Visibility = ViewStates.Gone;
			_cancelVoid.SetBackgroundColor(POSTheme.CancelButtonColor);
			_cancelVoid.Click += (sender, e) => {
				_managerCode.Text = string.Empty;
				okBut.Visibility = ViewStates.Visible;
				cancelBut.Visibility = ViewStates.Visible;
				_voidDeleteButton.Visibility = ViewStates.Visible;
				_wasteButton.Visibility = ViewStates.Visible;
				_modifiersGrid.Visibility = ViewStates.Visible;
				var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow(_managerCode.WindowToken, HideSoftInputFlags.None);

			};
			_cancelVoid.SetMinimumWidth(POSTheme.CategoryButtonWidth);
			_cancelVoid.SetMinHeight(POSTheme.OrdersScreenCommandBarHeight);
			modifiersCommandsCol.AddView(_cancelVoid);
		}

		ImageButton _homeButton;

		LinearLayout CreateCategoryMenu()
		{
			var categoryMenu = POSTheme.CreateRowForGrid(this);
			_homeButton = POSTheme.CreateImageButton(this, "categoryhome.png", POSTheme.CategoryHomeColor, POSTheme.CategoryNavButtonWidth, POSTheme.CategoryButtonHeight);
			_homeButton.SetMinimumHeight(POSTheme.CategoryMenuHeight);
			_homeButton.SetMinimumWidth(POSTheme.CategoryMenuWidth);
			_homeButton.Click += (sender, e) => {
				if (_selectedCategoryButton != null) {
					_selectedCategoryButton.SetBackgroundColor(POSTheme.SubCategoryColor);
					_selectedCategoryButton.SetTextColor(POSTheme.CategoryTextColor);
					_selectedCategoryButton = null;
				}
				_categoryRow.Visibility = ViewStates.Gone;
				_currentCategoryDisplay.Text = "";
				_currentCategoryDisplay.SetTextColor(POSTheme.DefaultTextColor);
				_vm.CategoryHomeClicked.Execute(null);
				/*Model.SelectedCategory = null;
				LoadCategories (_categoriesCol);
				LoadMenuItems (_menuItemsGrid);
				LoadHotItems ();
				UpdateCommands(_vm);*/
			};
			categoryMenu.AddView(_homeButton);

			return categoryMenu;
		}

		protected override void OnResume()
		{
			try {
				base.OnResume();
				if (Model != null) {
					Model.SetCreditCardProcessedCallback(card => MoveToCurrentView());
					ChangeToOrderMode();
					Model.SelectedCategory = null;
					_selectedCategoryButton = null;
					LoadMenuItems(_menuItemsGrid);
					LoadHotItems();
					PopulateOrderItems(OrderItemsCol);
				}
			} catch (Exception e) {
				Logger.Log("Failure OnResume OrderOnTab");
				Logger.HandleException(e, LogLevel.Fatal);
			}
		}


		/// <summary>
		/// Event handler for when a category is clicked
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void CategoryClicked(object sender, EventArgs e)
		{
			//get the cateogry
			var button = sender as Button;
			if (button == null) {
				//we need to find the button with this
				return;
			}
				
			var category = Model.GetCategoryByName(button.Text);
			_vm.CategoryClicked.Execute(category);
		
		}


		Dictionary<Guid, Button> _categoryButtonsDic;

		void LoadCategories(ViewGroup categoriesCol)
		{
			if (categoriesCol == null) {
				Logger.Log("LoadCategories called while categories column is null!", LogLevel.Debug);
				return;
			}
			_currentCategoryDisplay.Text = Model.SelectedCategory == null 
				? "" 
				: Model.SelectedCategory.Name;

			_currentCategoryDisplay.Visibility = Model.SelectedCategory == null
				? ViewStates.Gone
				: ViewStates.Visible;

			_categoryRow.Visibility = Model.SelectedCategory == null ? ViewStates.Invisible : ViewStates.Visible;

			try {
				if (Model.CurrentSubCategories.Any()) {
					_selectedCategoryButton = null;
					categoriesCol.RemoveAllViews();
					_categoryButtonsDic = new Dictionary<Guid, Button>();
					foreach (var category in Model.CurrentSubCategories) {
						Color color = POSTheme.SubCategoryColor;
						var button = POSTheme.CreateButton(this, category.Name, color);
						if (Model.SelectedCategory != null && category.Name == Model.SelectedCategory.Name) {
							POSTheme.MarkAsSelected(button);
						}
						button.SetMinimumWidth(POSTheme.CategoryButtonWidth);
						button.SetMinHeight(POSTheme.CategoryButtonHeight);
						button.Click += CategoryClicked;
						categoriesCol.AddView(button);
						_categoryButtonsDic.Add(category.ID, button);

						var spacer = new Space(this);
						spacer.SetBackgroundColor(Color.White);
						spacer.SetMinimumHeight(2);
						categoriesCol.AddView(spacer);

					}
				} else {
					var button = _categoryButtonsDic[Model.SelectedCategory.ID];
					POSTheme.MarkAsSelected(button);

					if (_selectedCategoryButton != null) {
						_selectedCategoryButton.SetBackgroundColor(POSTheme.SubCategoryColor);
						_selectedCategoryButton.SetTextColor(POSTheme.CategoryTextColor);
					}

					_selectedCategoryButton = button;
				}

				BringCommandsToFront();
			} catch (Exception e) {
				Logger.HandleException(e);
			}
		}

		void LoadHotItems()
		{
			Logger.Log("Loading hot items", LogLevel.Debug);

			_hotItemsContainer.RemoveAllViews();
			var hotItemsToRender = Model.HotItemsUnderCurrentCategory.Take(POSTheme.NumMenuItemColumns).ToList();

			Logger.Log("Adding " + hotItemsToRender.Count + " hot items", LogLevel.Debug);

			if (hotItemsToRender.Any()) {
				_hotItemsContainer.Visibility = ViewStates.Visible;
				foreach (var hotItem in hotItemsToRender) {
					var button = POSTheme.CreateHotItemButton(this, hotItem);
					button.Click += MenuItemClicked;
					_hotItemsContainer.AddView(button);
				}
			} else {
				_hotItemsContainer.Visibility = ViewStates.Gone;
			}

			BringCommandsToFront();
			Logger.Log("Done loading hot items", LogLevel.Debug);
		}

		LinearLayout AddHorizontalScrollingRow(ViewGroup parent)
		{
			var scroller = new HorizontalScrollView(this);
			var realRow = new LinearLayout(this){ Orientation = Orientation.Horizontal };
			scroller.AddView(realRow);
			parent.AddView(scroller);

			return realRow;
		}

		void MenuItemClicked(object sender, EventArgs args)
		{
			try {
				var button = (Button)sender;
				var item = Model.MenuItemsUnderCurrentCategory.FirstOrDefault(mi => mi.ButtonName == button.Text);
				if (item == null) {
					item = Model.HotItemsUnderCurrentCategory.FirstOrDefault(mi => mi.ButtonName == button.Text);
				}
				_vm.MenuItemClicked.Execute(item);
			} catch (Exception ex) {
				Logger.HandleException(ex);
			}
		}

		void LoadMenuItems(ViewGroup menuItemsGrid)
		{
			try {
				menuItemsGrid.RemoveAllViews();
				//var categoriesAndMenuItems = Model.GetCategoryNamesAndMenuItems(POSTheme.MaxNumMiseItems, null);
				var categoriesAndMenuItems = _vm.GetCategoryNamesAndMenuItems(POSTheme.MaxNumMiseItems, null);
				bool inSubCats = false;

				var noCatSelected = Model.SelectedCategory == null;
				foreach (var both in categoriesAndMenuItems) {
					//add a label for the category, if there is one
					var colIndex = 0;
					var currRow = POSTheme.CreateRowForGrid(this);
					menuItemsGrid.AddView(currRow);
					if (both.Item1 != null && (noCatSelected == false && both.Item1.ID != Model.SelectedCategory.ID)) {
						inSubCats = true;
						//add the category name up top
						var labelView = new SubcategoryTextView(this, both.Item1, POSTheme);
						//if we click an item, we should go to its category
						labelView.Click += (sender, e) => {
							var sctv = sender as SubcategoryTextView;
							if (sctv == null) {
								Logger.Log("Got invalid type for Subcategory click!");
							} else {
								Model.SelectedCategory = sctv.MenuItemCategory;
								//get the button for this cat
								var button = _categoryButtonsDic[sctv.MenuItemCategory.ID];
								CategoryClicked(button, null);
							}
						};
						currRow.AddView(labelView);

						currRow = AddHorizontalScrollingRow(menuItemsGrid);
						//currRow = POSTheme.CreateRowForGrid(this);
						//menuItemsGrid.AddView(currRow);
					} 
					//keep track if we've filled a row or not to keep min spacing
					var filledRow = false;

					//cycle through all the menuItems associated with this category
					foreach (var menuItem in both.Item2) {
						//get the category of the item, and the color from that
						//var cat = viewModel.GetCurrentlyDisplayedCategoryForItem(menuItem);
						//var color = cat != null ? theme.GetColorForCategory (cat.Name) : theme.UnknownCategoryColor;

						var menuItemButton = noCatSelected
							? POSTheme.CreateMiseItemButton(this, menuItem)
							: POSTheme.CreateMenuItemButton(this, menuItem, inSubCats);
						if (colIndex >= POSTheme.NumMenuItemColumns) {
							filledRow = true;
							colIndex = 0;
							if (inSubCats == false) {
								currRow = POSTheme.CreateRowForGrid(this);
								menuItemsGrid.AddView(currRow);
							}
						}
						menuItemButton.Click += MenuItemClicked;

						currRow.AddView(menuItemButton);
						colIndex++;
					} //end foreach MI

					//lets check if we got a full row or not
					if (filledRow == false) {
						while (colIndex < POSTheme.NumMenuItemColumns) {
							var space = new Space(this);
							space.SetMinimumWidth(POSTheme.MenuItemButtonWidth + POSTheme.MenuItemButtonPadding);
							currRow.AddView(space);
							colIndex++;
						}
					}
				}//end foreach cat item

				BringCommandsToFront();
			} //end try
			catch (Exception e) {
				Logger.HandleException(e, LogLevel.Fatal);
			}
		}

		IList<IMenuItemModifierGroupControl> _modGroups;

		void LoadModifiers(ViewGroup modifiersCol)
		{
			if (_vm.SelectedOrderItem == null) {
				return;
			}
			_updatedMemoText = string.Empty;
			modifiersCol.RemoveAllViews();

			#region TopRow
			var topRow = POSTheme.CreateRowForGrid(this, POSTheme.MenuItemButtonPadding);
			modifiersCol.AddView(topRow);
			var name = new TextView(this) { Text = _vm.SelectedOrderItem.Name + "   " };
			name.SetTextSize(ComplexUnitType.Pt, POSTheme.ModifiersTopTextSize);
			name.SetTextColor(POSTheme.OrderItemTextColor);
			topRow.AddView(name);

			var currentOITotalField = new TextView(this) { Text = _vm.SelectedOrderItem.Total.ToFormattedString() };
			currentOITotalField.SetTextSize(ComplexUnitType.Pt, POSTheme.ModifiersTopTextSize);
			currentOITotalField.SetTextColor(POSTheme.OrderItemTextColor);
			topRow.AddView(currentOITotalField);
			#endregion

			#region Memo row
			var memoRow = new LinearLayout(this){ Orientation = Orientation.Horizontal };
			modifiersCol.AddView(memoRow);

			var memoText = new TextView(this){ Text = _vm.SelectedOrderItem.Memo };
			memoText.SetTextSize(Android.Util.ComplexUnitType.Pt, POSTheme.OrderItemTextSize);
			memoText.SetTextColor(POSTheme.OrderItemTextColor);
			memoRow.AddView(memoText);

			var addMemoButton = new Button(this);

			//edit area!
			var editMemo = POSTheme.CreateEditText(this);
			editMemo.Text = _vm.SelectedOrderItem.Memo;
			editMemo.SetMinimumWidth(POSTheme.ModifierMemoWidth);
			editMemo.InputType = InputTypes.TextFlagCapWords;
			editMemo.SetTextSize(ComplexUnitType.Pt, POSTheme.OrderItemTextSize);
			editMemo.Visibility = ViewStates.Gone;

			editMemo.AfterTextChanged += (sender, e) => {
				Toast.MakeText(this, editMemo.Text, ToastLength.Short).Show();
				_updatedMemoText = editMemo.Text;
			};

			editMemo.FocusChange += (sender, e) => {
				Toast.MakeText(this, editMemo.Text, ToastLength.Short).Show();
				_updatedMemoText = editMemo.Text;
			};
//			
			editMemo.EditorAction += (sender, e) => {
				editMemo.Visibility = ViewStates.Gone;
				addMemoButton.Visibility = ViewStates.Visible;
				_updatedMemoText = editMemo.Text;
				memoText.Text = _updatedMemoText;
				addMemoButton.Text = string.IsNullOrEmpty(_updatedMemoText) ? "Add Memo" : "Edit Memo";
				memoText.Visibility = ViewStates.Visible;

				var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow(editMemo.WindowToken, HideSoftInputFlags.None);
			};

			memoRow.AddView(editMemo);

			addMemoButton.Text = string.IsNullOrEmpty(_vm.SelectedOrderItem.Memo) ? "Add Memo" : "Edit Memo";
			addMemoButton.Click += (sender, e) => {
				editMemo.Visibility = ViewStates.Visible;

				memoText.Visibility = ViewStates.Gone;
				addMemoButton.Visibility = ViewStates.Gone;

				editMemo.RequestFocus();

				var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
				inputMgr.ShowSoftInput(editMemo, ShowFlags.Forced);
			};
			memoRow.AddView(addMemoButton);

			#endregion



			var modifiersScroller = new ScrollView(this);
			var modifiersArea = new LinearLayout(this) { Orientation = Orientation.Vertical };
			modifiersScroller.AddView(modifiersArea);
			modifiersCol.AddView(modifiersScroller);

			_modGroups = new List<IMenuItemModifierGroupControl>();
			if (_vm.SelectedOrderItem.MenuItem.PossibleModifiers.Any()) {
				Action<ModifierButton, MenuItemModifier> modsChangedHandler = (source, mod) => {
					try {
						var mods = _modGroups.SelectMany(mg => mg.GetSelectedModifiers());
						var currentOIRunningPrice = Model.CalculateNewRunningPriceForMod(_vm.SelectedOrderItem.MenuItem, mods);
						currentOITotalField.Text = currentOIRunningPrice.ToFormattedString();
					} catch (Exception e) {
						Logger.HandleException(e);
					}
				};
				foreach (var modGroup in _vm.SelectedOrderItem.MenuItem.PossibleModifiers) {
					IMenuItemModifierGroupControl control = modGroup.Exclusive
						? (IMenuItemModifierGroupControl)new ExclusiveMenuItemModifierGroupControl(this, modGroup, POSTheme, _vm.SelectedOrderItem, modsChangedHandler, 3)
						: new MenuItemModifierGroupControl(this, modGroup, POSTheme, _vm.SelectedOrderItem, modsChangedHandler, 3);

					var ctrol = control as View;
					modifiersArea.AddView(ctrol);
					_modGroups.Add(control);
				}							
			}
			//see if we can scroll to the top here?
		}

		void PopulateOrderItems(ViewGroup orderItemsCol)
		{
			Logger.Log("Populating order items", LogLevel.Debug);
			EventHandler oiClickFunc = (sender, e) => {
				//only process if we're not already in modifier mode
				if (Model.CurrentTerminalViewTypeToDisplay == TerminalViewTypes.ModifyOrder) {
					//we're already modifying, we don't need to populate right now
					return;
				}

				//get the order item
				var realLabel = sender as OrderItemLabel;

				if (realLabel == null) {
					//error
					Console.Error.WriteLine("Unable to get OrderItemLabel");
					return;
				}


				var clickedOrderItem = Model.SelectedCheck.OrderItems.FirstOrDefault(oi => oi.ID == realLabel.OrderItemID);
				if (clickedOrderItem != null) {
					//disable all the rest, and highlight ours
					if (realLabel != null) {
						POSTheme.MarkOrderItemAsSelected(realLabel);
						_selectedOrderItemView = realLabel;
					}
					//change our button text
					if (clickedOrderItem.Status == OrderItemStatus.Ordering) {
						_voidDeleteButton.Visibility = ViewStates.Gone;
					} else {
						_voidDeleteButton.Visibility = ViewStates.Visible;
						_voidDeleteButton.Text = clickedOrderItem.Status != OrderItemStatus.Added ? "Void" : "Delete";
						_wasteButton.Visibility = clickedOrderItem.Status != OrderItemStatus.Added ? ViewStates.Visible : ViewStates.Gone;
					}

					_vm.ModifierActive = true;
					Model.SelectOrderItemToModify(clickedOrderItem);
					if (clickedOrderItem.Status == OrderItemStatus.Added) {
						LoadModifiers(_modifiersGrid);
					} 

					//display our modifiers panel
					ChangeToModifierMode();

				}
			};

			PopulateOrderItemsList(Model, orderItemsCol, oiClickFunc);
		}


		/// <summary>
		/// Changes our view to stop taking orders, and display modifiers
		/// </summary>
		void ChangeToModifierMode()
		{
			_modifiersRightSide.Visibility = ViewStates.Visible;
			_ordersRightSide.Visibility = ViewStates.Gone;

			if (_vm.IsModifyingSentOrderItem) {
				_sentItemsModifiers.Visibility = ViewStates.Visible;
				_modifiersGrid.Visibility = ViewStates.Gone;
				POSTheme.SetButtonEnabled(compItem, _vm.CompSelectedOrderItem.CanExecute(null));
				POSTheme.SetButtonEnabled(undoCompItem, _vm.UndoCompedSelectedOrderItem.CanExecute(null));
				return;
			} 
 
			_modifiersGrid.Visibility = ViewStates.Visible;
			_sentItemsModifiers.Visibility = ViewStates.Gone;
		}

		void ChangeToOrderMode()
		{
			if (_selectedOrderItemView != null) {
				_selectedOrderItemView.SetBackgroundColor(POSTheme.OrderItemListBackgroundColor);
				_selectedOrderItemView.SetTextColor(POSTheme.OrderItemTextColor);
			}

			_modifiersRightSide.Visibility = ViewStates.Gone;
			_ordersRightSide.Visibility = ViewStates.Visible;

			_modifiersGrid.Visibility = ViewStates.Gone;
		}
	}
}

