using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using Android.Text;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Services;
using Mise.Core.Entities.People;
using System.Collections.Generic;
using MisePOSTerminal.ViewModels;
using MiseAndroidPOSTerminal.AndroidControls;

namespace MiseAndroidPOSTerminal.AndroidViews
{
	[Activity(Label = "misePOS Terminal")]
	public class ViewChecks : BasePOSView
	{
		ViewChecksViewModel _vm;
		LinearLayout _tabsScreen;
		LinearLayout _employeeCol;
		LinearLayout _employeeOptionsCol;
	
		List<EmployeeButton> _empButtons;

		/// <summary>
		/// Populate the employees
		/// </summary>
		/// <param name="employeeCol"></param>
		/// <returns></returns>
		void LoadEmployeePanel(ViewGroup employeeCol)
		{
			employeeCol.RemoveAllViews();
			_empButtons = new List<EmployeeButton>(); 
			foreach (var emp in Model.CurrentEmployees) {
				var empButton = new EmployeeButton(this, emp, POSTheme);
				empButton.Click += EmployeeClicked;
				employeeCol.AddView(empButton);

				ColorEmployeeButton(empButton);
				_empButtons.Add(empButton);
			}
		}

		void ColorEmployeeButton(EmployeeButton empButton)
		{
			if (Model.SelectedEmployee != null && Model.SelectedEmployee.ID == empButton.Employee.ID) {
				empButton.SetSelected();
			} else {
				if (Model.LastSelectedEmployee != null && Model.LastSelectedEmployee.ID == empButton.Employee.ID) {
					empButton.SetLastSelected();
				} else {
					empButton.SetUnselected();
				}
			}
		}

		void EmployeeClicked(object sender, EventArgs e)
		{
			if(_vm.EnteringText){
				return;
			}

			var button = sender as EmployeeButton;
			if (button == null) {
				Logger.Log("Invalid object of type " + sender.GetType() + " given to EmployeeClicked");
				return;
			}

			var emp = button.Employee;
			Model.EmployeeClicked(emp);

			//color all our buttons here
			foreach (var empButton in _empButtons) {
				ColorEmployeeButton(empButton);
			}

			//load the tabs
			LoadTabsPanel(_tabsScreen, Model);
			LoadEmployeeOptions(_employeeOptionsCol, Model);
		}

		/// <summary>
		/// Populate the tabs on our view model
		/// </summary>
		/// <param name="tabsScreen"></param>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		void LoadTabsPanel(ViewGroup tabsScreen, ITerminalApplicationModel viewModel)
		{
			tabsScreen.RemoveAllViews();
			var colCount = 0;

			var thisRow = POSTheme.CreateRowForGrid (this);
			tabsScreen.AddView (thisRow);
			foreach (var check in viewModel.OpenChecks)
			{
				if (colCount >= POSTheme.NumTabColumns) {
					colCount = 0;
					thisRow = POSTheme.CreateRowForGrid(this);
					tabsScreen.AddView(thisRow);
				}
				var button = new CheckButton (this, POSTheme, check);
			    button.Click += (sender, e) => {
					if(false == Model.CanOpenChecks){
						return;
					}
					var cbutton = sender as CheckButton;
					if (cbutton != null) {
						viewModel.CheckClicked(cbutton.Check);

						//signal to switch views
						MoveToCurrentView();
					}
				};
				thisRow.AddView(button);

				colCount++;
			}
		}

		void LoadEmployeeOptions(ViewGroup optionsCol, ITerminalApplicationModel viewModel)
		{
			optionsCol.RemoveAllViews();

			var cc = POSTheme.CreateButton (this, "CC", POSTheme.CreditButtonColor);
			var addTab = POSTheme.CreateButton(this, "New Tab", POSTheme.AddTabButtonColor);
			var clockout = POSTheme.CreateButton(this, "Clock Out", POSTheme.ClockOutButtonBackground);
			var clockoutCancel = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);
			var noSale = POSTheme.CreateButton(this, "No Sale", POSTheme.NoSaleButtonColor);
			var noSaleCancel = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);
			var allTabs = POSTheme.CreateButton(this, "All Tabs", POSTheme.CloseOrderButtonColor);
			var fastCash = POSTheme.CreateButton(this, "Fast Cash", POSTheme.CashButtonColor);

			optionsCol.Visibility = ViewStates.Visible;

			var newNameField = POSTheme.CreateEditText(this);
			newNameField.Focusable = true;
			newNameField.InputType = InputTypes.TextFlagCapWords;
			newNameField.Visibility = ViewStates.Gone;
			newNameField.Hint = "name";

			var cancelAddTab = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);
			cancelAddTab.SetMinimumHeight(POSTheme.AddTabButtonHeight);
			cancelAddTab.SetMinimumWidth(POSTheme.AddTabConfirmButtonWidth);
			cancelAddTab.Visibility = ViewStates.Gone;

			cc.Click += (object sender, EventArgs e) => {
				if (_vm != null) {
					if (_vm.StartCreditCardReader.CanExecute (null)) {
						_vm.StartCreditCardReader.Execute (null);
					}
					//we should probably update the button here to reflect the reading status
					if (_vm.CreditCardReaderReading) {
						//mark it as reading so we know
						cc.SetBackgroundColor (POSTheme.CancelButtonColor);
					} else {
						cc.SetBackgroundColor (POSTheme.CreditButtonColor);
					}
					POSTheme.SetButtonEnabled (cc, _vm.StartCreditCardReader.CanExecute (null));
				}
			};

			POSTheme.SetButtonEnabled (cc, _vm.StartCreditCardReader.CanExecute (null));

			addTab.SetMinimumHeight(POSTheme.AddTabButtonHeight);
			addTab.SetMinimumWidth(POSTheme.EmployeeOptionsColumnWidth);
			addTab.Click += (sender, e) => {
				if(_vm.EnteringText == false){
					_vm.EnteringText = true;
					try {
						newNameField.Visibility = ViewStates.Visible;
						cancelAddTab.Visibility = ViewStates.Visible;
						addTab.Visibility = ViewStates.Gone;
						clockout.Visibility = ViewStates.Gone;
						noSale.Visibility = ViewStates.Gone;
						allTabs.Visibility = ViewStates.Gone;
						fastCash.Visibility = ViewStates.Gone;
						var focus = newNameField.RequestFocus();
		
						var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
						inputMgr.ShowSoftInput(newNameField, ShowFlags.Forced);

						if (focus == false) {
							Logger.Log("Failed to get focus for new name!");
						}
					} catch (Exception ex) {
						Logger.HandleException(ex);
					}
				}
			};
			POSTheme.MarkButtonAsDisabled(addTab);

			newNameField.EditorAction += (sender, e) => {
				if (string.IsNullOrEmpty(newNameField.Text)) {
					return;
				}
				newNameField.Visibility = ViewStates.Gone;
				cancelAddTab.Visibility = ViewStates.Gone;
				addTab.Visibility = ViewStates.Visible;
				clockout.Visibility = ViewStates.Visible;
				clockout.Visibility = ViewStates.Visible;
				noSale.Visibility = ViewStates.Visible;
				allTabs.Visibility = ViewStates.Visible;
				fastCash.Visibility = ViewStates.Visible;
				viewModel.CreateNewCheck (newNameField.Text);

				var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow(newNameField.WindowToken, HideSoftInputFlags.None);

				newNameField.Text = string.Empty;
				_vm.EnteringText = false;
				MoveToCurrentView ();
			};
			cancelAddTab.Click += (sender, e) => {
				_vm.EnteringText = false;
				var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow (newNameField.WindowToken, HideSoftInputFlags.None);

				newNameField.Text = string.Empty;

				newNameField.Visibility = ViewStates.Gone;
				cancelAddTab.Visibility = ViewStates.Gone;
				addTab.Visibility = ViewStates.Visible;
				clockout.Visibility = ViewStates.Visible;
				allTabs.Visibility = ViewStates.Visible;
				fastCash.Visibility = ViewStates.Visible;
				noSale.Visibility = ViewStates.Visible;
			};

			optionsCol.AddView (cc);

			optionsCol.AddView(addTab);
			optionsCol.AddView(newNameField);

			var addButtLayout = new LinearLayout(this){ Orientation = Orientation.Horizontal };
			addButtLayout.AddView(cancelAddTab);
			optionsCol.AddView(addButtLayout);


			var noSalePasswordField = POSTheme.CreateEditText(this); 
			noSalePasswordField.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationPassword;
			noSalePasswordField.Hint = "passcode";

			noSale.SetBackgroundColor(POSTheme.NoSaleButtonColor);
			noSale.SetTextColor(POSTheme.DefaultTextColor);
			noSale.SetMinHeight(POSTheme.EmpOptionsButtonHeight);
			noSale.SetMinWidth(POSTheme.EmployeeOptionsColumnWidth);
			noSale.Click += (sender, e) => {
				if(_vm.EnteringText == false){
					_vm.EnteringText = true;
					clockout.Visibility = ViewStates.Gone;
					noSaleCancel.Visibility = ViewStates.Visible;
					noSalePasswordField.Visibility = ViewStates.Visible;
					addTab.Visibility = ViewStates.Gone;
					noSale.Visibility = ViewStates.Gone;
					allTabs.Visibility = ViewStates.Gone;
					fastCash.Visibility = ViewStates.Gone;
					noSalePasswordField.RequestFocus();

					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.ShowSoftInput(noSalePasswordField, ShowFlags.Forced);
				}
			};
			POSTheme.MarkButtonAsDisabled(noSale);
			optionsCol.AddView(noSale);

			noSalePasswordField.Visibility = ViewStates.Gone;
			noSalePasswordField.EditorAction += (sender, e) => {
				if (noSalePasswordField.Text == string.Empty) {
					return;
				}
				var noSaleRes = Model.NoSale(noSalePasswordField.Text);
				if (noSaleRes) {
					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.HideSoftInputFromWindow(noSalePasswordField.WindowToken, HideSoftInputFlags.None);

					clockout.Visibility = ViewStates.Visible;
					clockoutCancel.Visibility = ViewStates.Gone;
					noSalePasswordField.Visibility = ViewStates.Gone;
					addTab.Visibility = ViewStates.Visible;
					noSale.Visibility = ViewStates.Visible;
					allTabs.Visibility = ViewStates.Visible;
					fastCash.Visibility = ViewStates.Visible;
					noSaleCancel.Visibility = ViewStates.Gone;
					_vm.EnteringText = false;
					MoveToCurrentView ();
				}
				noSalePasswordField.Text = string.Empty;
			};
			optionsCol.AddView(noSalePasswordField);

			noSaleCancel.SetMinimumHeight(POSTheme.ClockOutButtonHeight);
			noSaleCancel.SetMinimumWidth(POSTheme.EmployeeOptionsColumnWidth);
			noSaleCancel.SetTextColor(POSTheme.DefaultTextColor);
			noSaleCancel.SetBackgroundColor(POSTheme.CancelButtonColor);
			noSaleCancel.Click += (sender, e) => {
				_vm.EnteringText = false;
				var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow (noSalePasswordField.WindowToken, HideSoftInputFlags.None);

				clockout.Visibility = ViewStates.Visible;
				clockoutCancel.Visibility = ViewStates.Gone;
				noSalePasswordField.Visibility = ViewStates.Gone;
				addTab.Visibility = ViewStates.Visible;
				noSale.Visibility = ViewStates.Visible;
				allTabs.Visibility = ViewStates.Visible;
				fastCash.Visibility = ViewStates.Visible;
				noSaleCancel.Visibility = ViewStates.Gone;
			};
			noSaleCancel.Visibility = ViewStates.Gone;
			optionsCol.AddView(noSaleCancel);

			allTabs.SetMinimumHeight (POSTheme.AddTabButtonHeight);
			allTabs.SetMinimumWidth (POSTheme.AddTabConfirmButtonWidth);
			allTabs.Click += (sender, e) => MoveToView (TerminalViewTypes.ClosedChecks);
			POSTheme.MarkButtonAsDisabled (allTabs);
			optionsCol.AddView (allTabs);

			fastCash.SetMinimumHeight(POSTheme.AddTabButtonHeight);
			fastCash.SetMinimumWidth(POSTheme.AddTabButtonWidth);
			POSTheme.MarkButtonAsDisabled(fastCash);
			optionsCol.AddView(fastCash);

			var clockoutPasswordField = POSTheme.CreateEditText(this);
			clockoutPasswordField.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationPassword;
			clockoutPasswordField.Visibility = ViewStates.Gone;
			clockoutPasswordField.Hint = "passcode";
			optionsCol.AddView(clockoutPasswordField);

				
			clockout.SetMinimumHeight(POSTheme.ClockOutButtonHeight);
			clockout.SetMinimumWidth(POSTheme.EmployeeOptionsColumnWidth);
			clockout.Click += (sender, e) => {
				if(_vm.EnteringText == false){
					_vm.EnteringText = true;
					clockoutCancel.Visibility = ViewStates.Visible;
					clockout.Visibility = ViewStates.Gone;
					addTab.Visibility = ViewStates.Gone;
					noSale.Visibility = ViewStates.Gone;
					fastCash.Visibility = ViewStates.Gone;
					allTabs.Visibility = ViewStates.Gone;

					clockoutPasswordField.Visibility = ViewStates.Visible;

					clockoutPasswordField.RequestFocus();

					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.ShowSoftInput(clockoutPasswordField, ShowFlags.Forced);
				}
			};


			clockoutPasswordField.EditorAction += (sender, e) => {
				if (string.IsNullOrEmpty(clockoutPasswordField.Text)) {
					return;
				}

				var clockoutRes = viewModel.EmployeeClockout(clockoutPasswordField.Text, viewModel.SelectedEmployee);
				clockoutPasswordField.Text = string.Empty;
				if (clockoutRes) {
					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.HideSoftInputFromWindow(clockoutPasswordField.WindowToken, HideSoftInputFlags.None);

					clockout.Visibility = ViewStates.Visible;
					clockoutCancel.Visibility = ViewStates.Gone;
					clockoutPasswordField.Visibility = ViewStates.Gone;
					addTab.Visibility = ViewStates.Visible;
					noSale.Visibility = ViewStates.Visible;
					_vm.EnteringText = false;
					LoadAll ();
				}

				clockoutPasswordField.Text = string.Empty;
			};
			optionsCol.AddView(clockout);

			clockoutCancel.SetMinimumHeight(POSTheme.ClockOutButtonHeight);
			clockoutCancel.SetMinimumWidth(POSTheme.EmployeeOptionsColumnWidth);
			clockoutCancel.SetBackgroundColor(POSTheme.CancelButtonColor);
			clockoutCancel.Click += (sender, e) => {
				_vm.EnteringText = false;
				var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow (clockoutPasswordField.WindowToken, HideSoftInputFlags.None);

				clockout.Visibility = ViewStates.Visible;
				clockoutCancel.Visibility = ViewStates.Gone;
				clockoutPasswordField.Visibility = ViewStates.Gone;
				addTab.Visibility = ViewStates.Visible;
				noSale.Visibility = ViewStates.Visible;
				fastCash.Visibility = ViewStates.Visible;
				allTabs.Visibility = ViewStates.Visible;
			};
			clockoutCancel.Visibility = ViewStates.Gone;
			optionsCol.AddView(clockoutCancel);

			if (Model.SelectedEmployee != null) {
				POSTheme.MarkButtonAsEnabled(allTabs);
				POSTheme.MarkButtonAsEnabled(fastCash);
				POSTheme.MarkButtonAsEnabled(clockout);
				POSTheme.MarkButtonAsEnabled(noSale);
				POSTheme.MarkButtonAsEnabled(addTab);
			} else {
				POSTheme.MarkButtonAsDisabled(allTabs);
				POSTheme.MarkButtonAsDisabled(fastCash);
				POSTheme.MarkButtonAsDisabled(clockout);
				POSTheme.MarkButtonAsDisabled(noSale);
				POSTheme.MarkButtonAsDisabled(addTab);
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			//TODO move as much over to the view model as we can
			_vm = new ViewChecksViewModel (Logger, Model);
			ViewModel = _vm;

			//main layout splits in 3
			var layout = new LinearLayout(this) { Orientation = Orientation.Horizontal };

			//employees on left
			var leftCol = new LinearLayout(this){ Orientation = Orientation.Vertical };
			var loginButton = POSTheme.CreateButton(this, "Clock In", POSTheme.LoginButtonBackground);
			var cancelLogin = POSTheme.CreateButton(this, "Cancel", POSTheme.CancelButtonColor);
			var loginPasscodeEditText = POSTheme.CreateEditText(this);
			loginPasscodeEditText.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationPassword;
			loginPasscodeEditText.Visibility = ViewStates.Gone;
			loginPasscodeEditText.Hint = "passcode";
			leftCol.AddView(loginPasscodeEditText);

			loginPasscodeEditText.EditorAction += (sender, e) => {
				if (string.IsNullOrEmpty(loginPasscodeEditText.Text)) {
					return;
				}

				//see if our passcode is correct!
				var clockInSuccess = Model.EmployeeClockin(loginPasscodeEditText.Text);

				if (clockInSuccess) {
					_vm.EnteringText = false;
					cancelLogin.Visibility = ViewStates.Gone;
					loginButton.Visibility = ViewStates.Visible;
					loginPasscodeEditText.Visibility = ViewStates.Gone;
					_employeeCol.Visibility = ViewStates.Visible;
					_employeeOptionsCol.Visibility = ViewStates.Visible;
					_tabsScreen.Visibility = ViewStates.Visible;
					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.HideSoftInputFromWindow(loginPasscodeEditText.WindowToken, HideSoftInputFlags.None);

					//reload employees and tabs
					LoadAll();
				}

				loginPasscodeEditText.Text = string.Empty;
			};


			loginButton.SetMinHeight(POSTheme.LoginButtonHeight);
			loginButton.SetMinWidth(POSTheme.LoginButtonWidth);
			loginButton.Click += (sender, e) => {
				if(_vm.EnteringText == false){
					_vm.EnteringText = true;
					loginPasscodeEditText.Visibility = ViewStates.Visible;
					cancelLogin.Visibility = ViewStates.Visible;
					loginButton.Visibility = ViewStates.Gone;
					_employeeCol.Visibility = ViewStates.Gone;
					_tabsScreen.Visibility = ViewStates.Invisible;
					_employeeOptionsCol.Visibility = ViewStates.Invisible;
					loginPasscodeEditText.RequestFocus();

					var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
					inputMgr.ShowSoftInput(loginPasscodeEditText, ShowFlags.Forced);
				}
			};
			leftCol.AddView(loginButton);

			cancelLogin.SetMinHeight(POSTheme.LoginButtonHeight);
			cancelLogin.SetMinWidth(POSTheme.LoginButtonWidth);
			cancelLogin.Visibility = ViewStates.Gone;
			cancelLogin.Click += (sender, e) => {
				_vm.EnteringText = false;
				loginPasscodeEditText.Text = string.Empty;
				loginPasscodeEditText.Visibility = ViewStates.Gone;
				cancelLogin.Visibility = ViewStates.Gone;
				loginButton.Visibility = ViewStates.Visible;
				_employeeCol.Visibility = ViewStates.Visible;
				_tabsScreen.Visibility = ViewStates.Visible;
				_employeeOptionsCol.Visibility = ViewStates.Visible;

				var inputMgr = GetSystemService(InputMethodService) as InputMethodManager;
				inputMgr.HideSoftInputFromWindow(loginPasscodeEditText.WindowToken, HideSoftInputFlags.None);
			};
			leftCol.AddView(cancelLogin);

			var empScroller = new ScrollView(this);
			_employeeCol = POSTheme.CreateColumn(this);
			empScroller.AddView(_employeeCol);
			leftCol.AddView(empScroller);
			layout.AddView(leftCol);

			//tabs in the middle
			var middleCol = new LinearLayout(this){ Orientation = Orientation.Vertical };

			_tabsScreen = new LinearLayout(this) { Orientation = Orientation.Vertical };
			_tabsScreen.SetMinimumWidth(POSTheme.NumTabColumns * (POSTheme.MenuItemButtonWidth + POSTheme.ButtonPadding) + 20);
			var scroller = new ScrollView(this);
			scroller.AddView(_tabsScreen);

			middleCol.AddView(scroller);
			layout.AddView(middleCol);

			//options on right
			var empOptionsScroller = new ScrollView(this);
			_employeeOptionsCol = new LinearLayout(this){ Orientation = Orientation.Vertical };
			_employeeOptionsCol.SetMinimumWidth(POSTheme.EmployeeOptionsColumnWidth);
			empOptionsScroller.AddView(_employeeOptionsCol);
			layout.AddView(empOptionsScroller);

			SetContentView(layout);

			LastUpdated = DateTime.Now;
		}

		/// <summary>
		/// If true, our tabs are outdated and should be updated
		/// </summary>
		protected override void OnResume()
		{
			base.OnResume();
			Model.SetCreditCardProcessedCallback(checkProcessed => {
				Logger.Log("Callback to ViewTabs on credit card processed", LogLevel.Debug);
				if (_tabsScreen != null) {
					bool posted = _tabsScreen.Post(() => LoadTabsPanel(_tabsScreen, Model));
					if (posted == false) {
						Logger.Log("Error while attempting to update panel", LogLevel.Debug);
					}
				}
			});
			LoadAll();
		}

		void LoadAll()
		{
			LoadEmployeePanel(_employeeCol);
			LoadTabsPanel(_tabsScreen, Model);
			LoadEmployeeOptions(_employeeOptionsCol, Model);
		}
	}
}

