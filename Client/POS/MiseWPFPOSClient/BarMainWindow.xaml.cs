using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mise.Core.Client.Mono.ViewModel.Implementation;
using Mise.Core.Client.Services.Implementation;
using Mise.Core.Client.ViewModel;
using Mise.Core.Entities.Base;
using Mise.Core.Services.Implementation;


namespace MiseWPFPOSClient
{
    /// <summary>
    /// Interaction logic for BarMainWindow.xaml
    /// </summary>
    public partial class BarMainWindow
    {

        private readonly ITerminalViewModel _viewModel;
        private IWPFView _currentPage;
        private readonly List<Button> _menuButtons;


        #region Brushes
        private Style _normalButtonStyle;
        private Style _selectedButtonStyle;
        #endregion

        public BarMainWindow()
        {
            InitializeComponent();
            var logger = new DummyLogger();
            _viewModel = new TerminalViewModel(new FakeRestaurantServiceClient());
            ShowsNavigationUI = false;
            _menuButtons = new List<Button> { btnHome, btnOrder };
        }

        #region Events
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //load styles
            _normalButtonStyle = (Style)Application.Current.TryFindResource("RoundButton");
            _selectedButtonStyle = (Style)Application.Current.TryFindResource("SelectedRoundButton");

            DisplayCurrentView();
        }

        private void HomeClicked(object sender, RoutedEventArgs e)
        {
            if (_currentPage == null || _currentPage.CanNavigateAway())
            {
                SwitchToView(TerminalViewTypes.ViewTabs);
            }
        }

        private void OrderClicked(object sender, RoutedEventArgs e)
        {
            if (_currentPage == null || _currentPage.CanNavigateAway())
            {
                SwitchToView(TerminalViewTypes.OrderOnTab);
            }
        }
        #endregion

        internal void DisplayCurrentView()
        {
            Page dest = null;
            switch (_viewModel.CurrentTerminalViewTypeToDisplay)
            {
                case TerminalViewTypes.OrderOnTab:
                    dest = new BarOrderingView(this, _viewModel);
                    break;
                case TerminalViewTypes.ViewTabs:
                    dest = new BarTabView(this, _viewModel);
                    break;

            }
            _currentPage = dest as IWPFView;
            MarkButtonAsSelectedForView(_viewModel.CurrentTerminalViewTypeToDisplay);
            mainFrame.Navigate(_currentPage);
            UpdateViewsAvailabe();
        }

        /// <summary>
        /// Switches the display of this window to the specified view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="arg"></param>
        internal void SwitchToView(TerminalViewTypes view)
        {


            Page dest = null;
            switch (view)
            {
                case TerminalViewTypes.ViewTabs:
                    dest = new BarTabView(this, _viewModel);
                    break;
                case TerminalViewTypes.OrderOnTab:
                    dest = new BarOrderingView(this, _viewModel); 
                    break;
                    // throw new Exception("Attempted to switch to invalid view " + view.ToString());
            }
            _currentPage = dest as IWPFView;
            MarkButtonAsSelectedForView(view);
            mainFrame.Navigate(_currentPage);
            UpdateViewsAvailabe();
        }

        private void MarkButtonAsSelectedForView(TerminalViewTypes view)
        {
            var ons = new List<Button>();
            switch (view)
            {
                case TerminalViewTypes.ViewTabs:
                    ons.Add(btnHome);
                    break;
                case TerminalViewTypes.OrderOnTab:
                    ons.Add(btnOrder);
                    break;
            }
            ons.ForEach(b => b.Style = _selectedButtonStyle);
            var offs = _menuButtons.Except(ons).ToList();
            offs.ForEach(b => b.Style = _normalButtonStyle);
        }



        internal void UpdateViewsAvailabe()
        {
            if (_currentPage == null) return;
            var listViews = _currentPage.GetEnabledMenuViews().ToList();
            var onButtons = new List<Button>();

            listViews.ForEach(v =>
                                  {
                                      switch (v)
                                      {
                                          case TerminalViewTypes.OrderOnTab:
                                              onButtons.Add(btnOrder);
                                              break;
                                          case TerminalViewTypes.ViewTabs:
                                              onButtons.Add(btnHome);
                                              break;
                                      }
                                  }
                );

            var offs = _menuButtons.Except(onButtons).ToList();

            offs.ForEach(b => b.IsEnabled = false);
            onButtons.ForEach(b => b.IsEnabled = true);
        }


    }
}
