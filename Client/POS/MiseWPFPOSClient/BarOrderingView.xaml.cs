using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mise.Core.Client.ViewModel;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payment;
using Mise.Core.ValueItems;
using MiseWPFPOSClient.Modals;

namespace MiseWPFPOSClient
{
    /// <summary>
    /// Interaction logic for OrderOnTab.xaml
    /// </summary>
    public partial class BarOrderingView : IWPFView
    {


        // private readonly IBarClientService _clientService;

        private readonly BarMainWindow _parent;
        private readonly ITerminalViewModel _viewModel;
        public BarOrderingView(BarMainWindow parent, ITerminalViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;

            _parent = parent;
        }


        #region Events
        /// <summary>
        /// Fired when a user clicks a menu item in the order.  Display the available modifiers, if any
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var item = (IOrderItem)btn.CommandParameter;

            //get our possible modifiers for the menu item
            var mods = item.MenuItem.PossibleModifiers;

            //display a modal with the mods
            var modal = new ModifierSelector(item, mods) { Owner = Application.Current.MainWindow };

            modal.ShowDialog();

            //add the modifiers to the order item
            switch (modal.Result.Result)
            {
                case ModifierSelector.ModifierSelectorResult.ResType.Added:
                    item.AddModifiers(modal.Result.Mods);
                    break;
                case ModifierSelector.ModifierSelectorResult.ResType.Delete:
                    //remove this from our list
                    var orderItemToDelete = item as IOrderItem;
                    if (orderItemToDelete != null)
                    {
                        _viewModel.SelectedTab.OrderItems.Remove(orderItemToDelete);
                    }
                    else
                    {
                        item.RemoveModifiers(modal.Result.Mods);
                        //find which thing has this, and remove it
                    }
                    break;
                case ModifierSelector.ModifierSelectorResult.ResType.Multiply:
                    var orderItem = item;
                    if (orderItem != null)
                    {
                        _viewModel.SelectedTab.OrderItems.Add((orderItem).Clone());
                    }
                    break;
            }

            UpdateView();

        }

        /// <summary>
        /// Fired when a user clicks on a category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryClicked(object sender, RoutedEventArgs e)
        {
            //get our category that was selected
            var button = (Button)sender;

            CategoryClicked(button.CommandParameter.ToString());
        }

        /// <summary>
        /// Fired when a direct menu item is clicked to order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOrdered(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var item = (IMenuItem)btn.CommandParameter;

            _viewModel.DrinkClicked(item);

            UpdateView();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_viewModel.SelectedTab != null)
                {
                    //we need to load the check
                    UpdateView();



                    if (_viewModel.SelectedTab.Customer != null)
                    {
                        txtName.Text = _viewModel.SelectedTab.Customer.FirstName + " " + _viewModel.SelectedTab.Customer.LastName;
                    }
                }

                //load our categories
                LoadCategories();

                //load our hot menu items
                LoadHotMenuItems();

                //load our admin menu items
                LoadAdminMenuItems();

            }
            catch (Exception ex)
            {
                HandleGUIException(ex);
            }
        }


        /// <summary>
        /// Cashes out this ticket, and closes it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CashClick1(object sender, RoutedEventArgs e)
        {
            //display total
            DisplayTotal(_viewModel.SelectedTab.Total);

            //open drawer

            //mark our check as paid
            _viewModel.CashButtonClicked();

            UpdateEnabledViews();

            //redirect
            _parent.SwitchToView(TerminalViewTypes.ViewTabs);
        }

        /// <summary>
        /// Sets this ticket to a tab that's open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabClick1(object sender, RoutedEventArgs e)
        {
            try
            {

                //if we don't have a name, get one from the on-screen keyboard, or the swipe
                if (_viewModel.SelectedTab.Customer == null)
                {
                    var modal = new TextEnter { Owner = Window.GetWindow(this) };
                    modal.ShowDialog();

                    var custName = modal.EnteredText;
                    if (custName == string.Empty)
                    {
                        return;
                    }
                    _viewModel.NameCurrentTab(custName);
                }

                //save our check
                _viewModel.ParkCurrentTab();
                //go back to the tab screen
                UpdateView();

                //redirect
                _parent.SwitchToView(_viewModel.CurrentTerminalViewTypeToDisplay);

            }
            catch (Exception ex)
            {
                HandleGUIException(ex);
            }
        }

        /// <summary>
        /// Clear any unsaved data on the check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearClick1(object sender, RoutedEventArgs e)
        {
            try
            {
                //roll back our check
                _viewModel.CancelOrdering();
                UpdateView();
            }
            catch (Exception ex)
            {
                HandleGUIException(ex);
            }


        }
        #endregion


        private void LoadCategories()
        {
            icCategories.ItemsSource = _viewModel.Categories
                .OrderByDescending(c => c.DisplayOrder)
                .ThenBy(c => c.Name);
        }

        private void LoadHotMenuItems()
        {
            icHotItems.ItemsSource = _viewModel.HotItems;
        }

        private void LoadAdminMenuItems()
        {
            icAdminItems.ItemsSource = _viewModel.AdminItems;
        }

        public bool CanNavigateAway()
        {
            return _viewModel.CanLeaveOrderScreen;
        }





        /// <summary>
        /// Order an item, or give a dialog for further options
        /// </summary>
        /// <param name="categoryName"></param>
        private void CategoryClicked(string categoryName)
        {
            try
            {
                var res = _viewModel.DrinkCategoryClicked(categoryName);
                if (res == CategoryClickedResultType.ShowSubCategories)
                {
                    //get our sub categories for this, and return the code to let our 
                    //pop our modal
                    var modal = new MenuItemSelector(_viewModel.CurrentCategory) { Owner = Application.Current.MainWindow };
                    modal.ShowDialog();

                    foreach (var menuItem in modal.SelectedMenuItems)
                    {
                        _viewModel.DrinkClicked(menuItem);
                    }
                }

                UpdateView();
            }
            catch (Exception ex)
            {
                HandleGUIException(ex);
            }
        }




        public IList<TerminalViewTypes> GetEnabledMenuViews()
        {
            return _viewModel.CanLeaveOrderScreen ? new List<TerminalViewTypes> { TerminalViewTypes.OrderOnTab, TerminalViewTypes.ViewTabs } : new List<TerminalViewTypes> { TerminalViewTypes.OrderOnTab };
        }



        public TerminalViewTypes View
        {
            get { return TerminalViewTypes.OrderOnTab; }
        }


        public IEntityBase GetSwitchingArgument(TerminalViewTypes destView)
        {
            return null;
        }


        public void CreditCardWasSwiped(ICreditCard card)
        {
            _viewModel.CreditCardWasSwiped(card);
        }

        public void DisplayTotal(Money total)
        {
            //fire our modal
            var modal = new TotalDisplay(total.Dollars) { Owner = Application.Current.MainWindow };
            modal.ShowDialog();
        }

        /// <summary>
        /// Loads the values from our view model to the view
        /// </summary>
        private void UpdateView()
        {
            var check = _viewModel.SelectedTab;
            //load the menu items
            if (check.OrderItems != null)
            {
                //get a flat list of the menu items, and the modifiers on the order items
                //right now this only supports one level, we'll use tree view to change that
                var list = new List<ICanTakeModifier>();
                foreach (var oi in check.OrderItems)
                {
                    list.Add(oi);
                    list.AddRange(oi.Modifiers);
                }
                icOrderedItems.ItemsSource = list;

                txtTotal.Text = "$" + check.Total.Dollars.ToString("f2");
            }

            scrOrders.ScrollToBottom();

            UpdateEnabledViews();

            UpdateTotal();
        }
        /// <summary>
        /// Updates the total displayed
        /// </summary>
        private void UpdateTotal()
        {
        }

        public void UpdateEnabledViews()
        {
            _parent.UpdateViewsAvailabe();
        }

        private static void HandleGUIException(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
