using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mise.Core.Client.ViewModel;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payment;
using Mise.Core.Entities.People;

namespace MiseWPFPOSClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BarTabView : IWPFView
    {
       
        #region Fields


        private readonly BarMainWindow _container;
        #endregion

        #region Brushes
        private Style _normalButtonStyle;
        private Style _selectedButtonStyle;
        #endregion

        #region UIControls
        private Button _selectedEmployeeButton;
        #endregion

        private readonly ITerminalViewModel _viewModel;
        #region Window Events
        public BarTabView(BarMainWindow parent, ITerminalViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;

            _container = parent;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //bind employees
            BindEmployees(_viewModel.Employees);

            //bind checks
            BindChecks(_viewModel.Tabs);

            //load styles
            _normalButtonStyle = (Style)Application.Current.TryFindResource("RoundButton");
            _selectedButtonStyle = (Style)Application.Current.TryFindResource("SelectedRoundButton");

        }

        /// <summary>
        /// Fired when a check is clicked, moves us to the order screen with this loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckClicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var check = (ICheck)button.CommandParameter;

            _viewModel.TabClicked(check);
            /*
            if (check == null)
            {
                _logger.Log("Invalid check");
                return;
            }*/

            //move to the tab screen, with this check
            SwitchToOrdering(check);
        }

        /// <summary>
        /// Fired when we select an employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmployeeSelected(object sender, RoutedEventArgs e)
        {
            //determine which item did this
            var button = (Button)sender;
            var empID = (string)button.CommandParameter;

            var emp = _viewModel.Employees.FirstOrDefault(em => em.EmployeeID == empID);
            //see if we need to verify the employee first
            if (_viewModel.RequireEmployeeSignIn)
            {
                //launch modal, and find out if we signed in correctly
            }
            _viewModel.EmployeeClicked(emp);

            icNotifications.ItemsSource = _viewModel.Notifications ?? new List<IEmployeeNotification>();

            //button.Background = _selectedButtonBrush;
            button.Style = _selectedButtonStyle;

            //uncolor the others
            if (_selectedEmployeeButton != null && _selectedEmployeeButton.Equals(button) == false)
            {
                //_selectedEmployeeButton.Background = _normalButtonBrush;
                _selectedEmployeeButton.Style = _normalButtonStyle;
            }

            _selectedEmployeeButton = button;

            //update our checks to only display for this server
            BindChecks(_viewModel.Tabs);

            //update available buttons
            UpdateEnabledViews();
        }

        #endregion

        private void BindChecks(IEnumerable<ICheck> checks)
        {
            icChecks.ItemsSource = checks;
        }

        private void BindEmployees(IEnumerable<IEmployee> employees)
        {
            icEmployees.ItemsSource = employees.Select(em => new { Name = em.FirstName + " " + em.LastName, em.EmployeeID });
        }

        /// <summary>
        /// Switches us to the ordering view
        /// </summary>
        /// <param name="check"></param>
        private void SwitchToOrdering(ICheck check)
        {
            _container.SwitchToView(TerminalViewTypes.OrderOnTab);
        }

        public bool CanNavigateAway()
        {
            return true;
        }


        public IList<TerminalViewTypes> GetEnabledMenuViews()
        {
            return _viewModel.SelectedEmployee == null ? new List<TerminalViewTypes> { TerminalViewTypes.ViewTabs } : new List<TerminalViewTypes> { TerminalViewTypes.ViewTabs, TerminalViewTypes.OrderOnTab };
        }

        public void UpdateEnabledViews()
        {
            _container.UpdateViewsAvailabe();
        }

        IEntityBase IWPFView.GetSwitchingArgument(TerminalViewTypes destView)
        {
            return GetSwitchingArgument(destView);
        }

        public TerminalViewTypes View { get { return TerminalViewTypes.ViewTabs; } }


        public IEntityBase GetSwitchingArgument(TerminalViewTypes destView)
        {
            if (destView == TerminalViewTypes.OrderOnTab && _viewModel.SelectedEmployee != null)
            {
                //return the employee
                return _viewModel.SelectedEmployee;
            }

            return null;
        }

        public void CreditCardWasSwiped(ICreditCard card)
        {
            //if we have a tab for this name, get it

            //otherwise, we create a new one, and go to the ordering screen
        }
    }
}
