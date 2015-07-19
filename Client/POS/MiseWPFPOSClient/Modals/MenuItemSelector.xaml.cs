using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mise.Core.Entities.Menu;

namespace MiseWPFPOSClient.Modals
{
    /// <summary>
    /// Interaction logic for CategorySelector.xaml
    /// </summary>
    public partial class MenuItemSelector
    {

        private readonly IMenuItemCategory _category;

        private bool _locked;

        private readonly List<IMenuItem> _selectedMenuItems;
        public IList<IMenuItem> SelectedMenuItems { get { return _selectedMenuItems; } }

        public MenuItemSelector(IMenuItemCategory category)
        {
            _category = category;
            _selectedMenuItems = new List<IMenuItem>();
            _locked = false;
            InitializeComponent();
        }

        /// <summary>
        /// Fired when a user clicks on a category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClicked(object sender, RoutedEventArgs e)
        {
            //get our category that was selected
            var button = (Button)sender;

            //get the menu item or category that was selected
            if (button.CommandParameter is IMenuItem)
            {
                _selectedMenuItems.Add((IMenuItem)button.CommandParameter);
                if (!_locked)
                {
                    Close();
                }
            }
            else
            {
                var category = button.CommandParameter as IMenuItemCategory;
                if (category != null)
                {
                    var items = GetItemsFromCategory(category);
                    LoadItems(items);
                }
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //get our source - it's the menuItems, plus whatever sub categories we have
            var items = GetItemsFromCategory(_category);
            LoadItems(items);
        }


        private void LoadItems(IEnumerable<object> source)
        {
            icMenuItems.ItemsSource = source;
        }

        private static IEnumerable<object> GetItemsFromCategory(IMenuItemCategory category)
        {
            var res = new List<object>();

            //get the menu items
            var menuItems = category.MenuItems;
            res.AddRange(menuItems);
            var subCats = category.SubCategories;
            res.AddRange(subCats);

            return res;
        }


        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleLock(object sender, RoutedEventArgs e)
        {
            _locked = !_locked;
            //btnToggleLock.Content = _locked ? "Unlock" : "Lock";
        }



    }
}
