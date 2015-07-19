using System;
using System.Collections.Generic;
using Mise.Core.Client.ApplicationModel;
using MisePOSTerminal.ViewModels;
using Xamarin.Forms;

using Mise.Core.Entities.Check;

namespace MisePOSTerminal
{
	public partial class OrderOnCheck : ContentPage
	{
		OrderOnCheckViewModel _vm;

		public OrderOnCheck()
		{
			InitializeComponent();

			_vm = App.OrderOnCheckViewModel;
			BindingContext = _vm;

			_vm.OnMoveToView += MoveToCaller;
			_vm.OnLoadCategories += LoadCategories;
			_vm.OnLoadMenuItems += LoadMenuItems;
			_vm.OnLoadOrderItems += LoadOrderItems;

			Appearing += (object sender, EventArgs e) => {
				LoadCategories(_vm);
				LoadMenuItems(_vm);
				LoadOrderItems(_vm);
			};
		}

		async void MoveToCaller(BaseViewModel vm)
		{
			App.ViewChecksViewModel.Reload(vm);
			await Navigation.PopModalAsync();
		}

		void LoadCategories(BaseViewModel vm)
		{
			//do we have subcats to load?
			//TODO if we don't have enough cats to fill this up, we should add a box view
			pnlCategories.Children.Clear();
			foreach (var cat in _vm.CurrentSubCategories) {
				var button = new Button {
					BackgroundColor = App.Theme.CategoryColor,
					Text = cat.Name,
					CommandParameter = cat,
					Command = _vm.CategoryClicked,
					HeightRequest = App.Theme.CategoryButtonHeight,
					TextColor = App.Theme.TextColor,
					BorderRadius = App.Theme.BorderRadius,
					Font = App.Theme.ButtonFont
				};

				if (_vm.SelectedCategory != null && cat.ID == _vm.SelectedCategory.ID) {
					button.BackgroundColor = App.Theme.SelectedBackgroundColor;
					button.TextColor = App.Theme.SelectedTextColor;
				}

				//add event handler
				pnlCategories.Children.Add(button);
			}
		}

		void LoadMenuItems(BaseViewModel vm)
		{
			try {
				var buttonFont = Font.SystemFontOfSize(App.Theme.MenuItemButtonFontSize);

				//TODO - determine if we need to load here, or if what we have should be good.  Do an observable on the model for this
				pnlHotItems.Children.Clear();
				foreach (var hi in _vm.HotItems) {
					var hiButton = new Button {
						BackgroundColor = App.Theme.HotItemColor,
						BorderRadius = App.Theme.BorderRadius,
						Text = hi.ButtonName,
						CommandParameter = hi,
						WidthRequest = App.Theme.MenuItemButtonWidth,
						Command = _vm.MenuItemClicked,
						Font = buttonFont,
					};
					pnlHotItems.Children.Add(hiButton);
				}

				var row = 0;
				var col = 0;
				//load the rest of our stuff
				if (_vm.MiseIsVisible) {
					//TODO determine if we need to load or not - only update when we've got an observable notice from the model?
					grdMiseItems.Children.Clear();
					foreach (var mItem in _vm.MiseItems) {
						grdMiseItems.Children.Add(
							new Button {
								BackgroundColor = App.Theme.MiseItemColor,
								BorderRadius = App.Theme.BorderRadius,
								HorizontalOptions = LayoutOptions.CenterAndExpand,
								VerticalOptions = LayoutOptions.CenterAndExpand,
								Text = mItem.ButtonName,
								CommandParameter = mItem,
								Command = _vm.MenuItemClicked,
								WidthRequest = App.Theme.MenuItemButtonWidth,
								Font = buttonFont,
							}, col++, row
						);
						if (col > 2) {
							col = 0;
							row++;
						}
					}
				} else {
					grdCategory.Children.Clear();
					var items = _vm.GetCategoryNamesAndMenuItems(12, null);
					foreach (var catAndItems in items) {
						//make our cat as a button
						var catButton = new Button {
							BorderRadius = 0,
							Text = catAndItems.Item1.Name,
							Font = buttonFont,
							CommandParameter = catAndItems.Item1,
							Command = _vm.CategoryClicked
						};
						grdCategory.Children.Add(catButton);
						if (catAndItems.Item1.ID == _vm.SelectedCategory.ID) {
							//main selected cat here

							//add each of the items
							var column = 0;
							var buttonRow = new StackLayout{ Orientation = StackOrientation.Horizontal, Spacing = 24 };
							grdCategory.Children.Add(buttonRow);
							foreach (var item in catAndItems.Item2) {
								var mi = new Button {
									Font = buttonFont,
									BackgroundColor = App.Theme.MenuItemColor,
									WidthRequest = App.Theme.MenuItemButtonWidth,
									BorderRadius = App.Theme.BorderRadius,
									Text = item.ButtonName,
									Command = _vm.MenuItemClicked,
									CommandParameter = item
								};

								buttonRow.Children.Add(mi);
								column++;
								if (column > 2) {
									column = 0;
									buttonRow = new StackLayout{ Orientation = StackOrientation.Horizontal, Spacing = 24 };
									grdCategory.Children.Add(buttonRow);
								}
							}


						} else {
							//sub category
		
							//add our items in a single horizontal scroller
							var scroller = new ScrollView{ Orientation = ScrollOrientation.Horizontal };
							grdCategory.Children.Add(scroller);

							var itemHolder = new StackLayout{ Orientation = StackOrientation.Horizontal, Spacing = 24 };
							scroller.Content = itemHolder;

							foreach (var item in catAndItems.Item2) {
								if (item != null) {
									//itemHolder.Children.Add(new Label{Text = "test", TextColor = Color.White});
									var newButt = new Button {
										WidthRequest = App.Theme.MenuItemButtonWidth,
										BorderRadius = App.Theme.BorderRadius,
										Text = item.ButtonName,
										TextColor = Color.White,
									};
									itemHolder.Children.Add(newButt);
								}
								/*var mi = new Button {
									Font = buttonFont,
									BackgroundColor = App.Theme.SubCatItemColor,
									WidthRequest = App.Theme.MenuItemButtonWidth,
									BorderRadius = App.Theme.BorderRadius,
									Text = item.ButtonName,
									//Command = _vm.OrderMenuItem,
									//CommandParameter = item
								};

								itemHolder.Children.Add (mi);*/
							}
						}
					}
				}
			} catch (Exception e) {
				App.Logger.HandleException(e);
			}

		}

		void LoadOrderItems(BaseViewModel vm)
		{
			LoadOrderItemsIntoPane(pnlOrderItems, _vm.SelectedCheck);

			//add the total as well, to the XAML area
		}

		static void LoadOrderItemsIntoPane(StackLayout container, ICheck check)
		{
			var bigFont = Font.SystemFontOfSize(App.Theme.OrderItemFontSize);
			var divWidth = (int)(container.Width * 0.9);
			foreach (var oi in check.OrderItems) {
				//all our items for this OI in one container, makes it easier to click properly
				var allOIContainer = new StackLayout {
					Orientation = StackOrientation.Vertical
				};

				//make our main label
				var nameLabel = new Label {
					TextColor = App.Theme.OrderItemFontColor,
					Text = oi.MenuItem.ButtonName,
					XAlign = TextAlignment.Start,
					HorizontalOptions = LayoutOptions.StartAndExpand,
					Font = bigFont
				};
				var priceLabel = new Label {
					TextColor = App.Theme.OrderItemFontColor,
					Text = oi.Total.ToFormattedString(),
					XAlign = TextAlignment.End,
					HorizontalOptions = LayoutOptions.EndAndExpand,
					Font = bigFont
				};
				var oiContainer = new StackLayout {
					Orientation = StackOrientation.Horizontal,
					Children = { nameLabel, priceLabel },
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				allOIContainer.Children.Add(oiContainer);

				//if we have modifiers, give new lines in our container for that
				foreach (var mod in oi.Modifiers) {
				}

				//add our divider line
				var div = new BoxView {
					BackgroundColor = App.Theme.OrderDividerLineColor,
					WidthRequest = divWidth,
					HeightRequest = 1,
				};
				allOIContainer.Children.Add(div);

				container.Children.Add(allOIContainer);
			}
		}

	}
}

