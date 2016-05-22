using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace StockboyForms.Pages
{
    public partial class AdminMenuPage : BasePage
    {
        public AdminMenuPage()
        {
            InitializeComponent();
        }

        #region implemented abstract members of BasePage

        public override BaseViewModel ViewModel {
            get {
                return App.AdminMenuViewModel;
            }
        }

        public override string PageName {
            get {
                return "AdminMenu";
            }
        }

        #endregion
    }
}

