using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
    public partial class EULAPage : BasePage
    {
        public EULAPage()
        {
            InitializeComponent();
         
        }

        #region implemented abstract members of BasePage

        public override Mise.Inventory.ViewModels.BaseViewModel ViewModel {
            get {
                return App.EULAViewModel;
            }
        }

        public override string PageName {
            get {
                return "EULAPage";
            }
        }

        #endregion
    }
}

