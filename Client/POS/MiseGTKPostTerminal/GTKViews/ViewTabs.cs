using System;
using System.Linq;
using Gtk;
using Mise.Core.Entities.Check;
using Mise.Core.Services;
using Mise.Core.Client.ViewModel;
using Mise.Core.Entities;

namespace MiseGTKPostTerminal.GTKViews
{
    class ViewTabs : IGTKTerminalView
    {
        /// <summary>
        /// Application we're rendering to
        /// </summary>
        private readonly Window _window;
        private readonly ITerminalApplicationModel _viewModel;
        private readonly DefaultTheme _theme;
        private readonly ILogger _logger;
        private readonly UpdateViewCallback _updateView;
        public ViewTabs(DefaultTheme theme, ITerminalApplicationModel viewModel, Window window, ILogger logger, UpdateViewCallback updateView)
        {
            _theme = theme;
            _viewModel = viewModel;
            _window = window;
            _logger = logger;
            _updateView = updateView;
        }

        public TerminalViewTypes Type { get{return TerminalViewTypes.ViewTabs;} }

        public void LoadView()
        {
            LoadTabView();
        }

        private void LoadTabView()
        {
            var mainDiv = new HBox();
            //color the main box here?
            _window.Add(mainDiv);


            //make the employee container
            var employeeBox = new VBox { BorderWidth = _theme.EmployeeBorderOnTabScreenWidth };
            mainDiv.PackStart(employeeBox, false, true, 0);

            //make the buttons for each employee
            var emps = _viewModel.CurrentEmployees;
            foreach (var emp in emps)
            {
                var empButton = new Button
                {
                    Label = emp.DisplayName,
                    HeightRequest = 100,
                    WidthRequest = 150
                };

                empButton.ModifyFg(StateType.Normal, _theme.ButtonTextColor);

                if (emp == _viewModel.SelectedEmployee)
                {
                    //mark it as selected here!
                    empButton.ModifyBg(StateType.Normal, _theme.SelectedEmployeeButtonColor);
                    empButton.ModifyBg(StateType.Active, _theme.SelectedEmployeeButtonColor);
                    empButton.ModifyBg(StateType.Prelight, _theme.SelectedEmployeeButtonColor);

                }
                empButton.Clicked += EmpButtonOnClicked;

                employeeBox.PackStart(empButton);
            }

			//add the add button here?
			if (_viewModel.SelectedEmployee != null) {
				var addButton = new Button { Label = "+", HeightRequest = 100, WidthRequest = 150 };
				addButton.ModifyFg (StateType.Normal, _theme.ButtonTextColor);
				addButton.ModifyBg (StateType.Normal, _theme.CashButtonColor);
				addButton.Clicked += CreateTabClicked;
				employeeBox.PackStart (addButton);
			}

            var tabBox = LoadTabBox();

            mainDiv.PackStart(tabBox, true, true, 0);
            mainDiv.ShowAll();
        }

        /// <summary>
        /// Loads our tab into the GUI so they can be selected
        /// </summary>
        /// <returns></returns>
        private Table LoadTabBox()
        {
            //make the tab container
            var numTabs = _viewModel.Tabs.Count();
            var sq = Math.Sqrt(numTabs);
            var over = sq - ((int)sq) > 0;
            var numSlots = (uint)sq;
            if (over)
            {
                numSlots++;
            }
            //var numSlots = _theme.NumberOfTabColumns;
            var tabBox = new Table(numSlots, numSlots, true);

            //add our current tabs
            uint rowPos = 1;
            uint colPos = 1;

            foreach (var tab in _viewModel.Tabs)
            {
                var tabButton = new Button { Label = tab.DisplayName };

                tabButton.Clicked += TabButtonOnClicked;
                tabButton.WidthRequest = 200;
                tabButton.HeightRequest = 100;

				switch (tab.PaymentStatus) {
				case CheckPaymentStatus.PaymentPending:
						tabButton.ModifyBg (StateType.Normal, _theme.PaymentPendingColor);
					break;
				case CheckPaymentStatus.PaymentRejected:
						tabButton.ModifyBg (StateType.Normal, _theme.PaymentRejectedColor);
					break;
				}

                //determine row posit
                tabBox.Attach(tabButton, colPos - 1, colPos, rowPos - 1, rowPos, AttachOptions.Expand, AttachOptions.Expand,
                              _theme.PaddingBetweenTabs, _theme.PaddingBetweenTabs);

                //update our next spot
                if (colPos >= numSlots)
                {
                    colPos = 1;
                    rowPos++;
                }
                else
                {
                    colPos++;
                }
            }
            return tabBox;
        }

        #region Events
        private void TabButtonOnClicked(object sender, EventArgs eventArgs)
        {
            var button = sender as Button;
            if (button == null)
            {
                _logger.Log("TabButtonClicked - can't handle object of type " + sender.GetType());
                return;
            }

            var tab = _viewModel.Tabs.FirstOrDefault(t => t.DisplayName == button.Label);
            if (tab == null)
            {
                _logger.Log("Unable to find tab " + button.Label, LogLevel.Error);
                return;
            }
            _viewModel.CheckClicked(tab);

            _updateView();
        }

		private void CreateTabClicked(object sender, EventArgs eventArgs)
		{
			_viewModel.CreateTabClicked ();
			_updateView ();
		}

        private void EmpButtonOnClicked(object sender, EventArgs eventArgs)
        {
            var button = sender as Button;
            if (button == null)
            {
                _logger.Log("EmpButtonClicked - can't handle object of type " + sender.GetType());
                return;
            }
            //find the employee
            var employee = _viewModel.CurrentEmployees.FirstOrDefault(e => e.DisplayName == button.Label);
            if (employee == null)
            {
                _logger.Log("Can't find employee " + button.Label);
                return;
            }

            //check if we need a sign in!
            if (_viewModel.RequireEmployeeSignIn)
            {
                //display modal and check the result
            }

            _viewModel.EmployeeClicked(employee);

            _updateView();
        }
        #endregion
    }
}
