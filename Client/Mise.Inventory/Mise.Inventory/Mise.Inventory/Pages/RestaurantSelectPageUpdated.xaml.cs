﻿using Mise.Core.ValueItems;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
    public partial class RestaurantSelectPageUpdated
    {
        private ListView _lv;

        public RestaurantSelectPageUpdated()
        {
            InitializeComponent();
        }

        #region implemented abstract members of BasePage

        public override BaseViewModel ViewModel => App.RestaurantSelectViewModel;

        public override string PageName => "RestaurantSelectPage";

        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as RestaurantSelectViewModel;
            if (vm != null)
            {
                if (_lv == null)
                {
                    var template = new DataTemplate(typeof(TextCell));
                    template.SetBinding(TextCell.TextProperty, "FullName");
                    _lv = new ListView
                    {
                        ItemTemplate = template,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };

                    _lv.ItemTapped += async (sender, e) => {
                        var selName = e.Item as BusinessName;
                        ((ListView)sender).SelectedItem = null;
                        if (selName != null)
                        {
                            await vm.SelectRestaurant(selName);
                        }
                    };
                    stckMain.Children.Add(_lv);
                }
                _lv.ItemsSource = vm.PossibleRestaurantNames;
            }
        }
    }
}
