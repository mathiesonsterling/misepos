using System;
using System.Collections.Generic;
using MisePOSTerminal.ViewModels;
using Xamarin.Forms;

using Mise.Core.Entities.People;

namespace MisePOSTerminal
{	

	public partial class ViewChecksMVVM : ContentPage
	{	
		/// <summary>
		/// Moves us to another screen from here
		/// </summary>
		async void MoveToOrder(){
			_entryNewCheckName.Unfocus();
			//await Navigation.PushAsync(new NavigationPage(new OrderOnCheck()));
			//TODO use a factory to get a preloaded instance!
			await Navigation.PushModalAsync (new OrderOnCheck ());
		}

		async void MoveToNoSale(){
			_entryNoSalePasscode.Unfocus();
			await Navigation.PushModalAsync (new DrawerOpen ());
		}

		readonly ViewChecksViewModel _vm;

		/// <summary>
		/// Matches our view to the current state of our models.  allows us to have the same instance of a view reused
		/// </summary>
		public void Reload(object sender, EventArgs args){
			LoadEmployees (_vm);
			LoadChecks (_vm);
		}

		public ViewChecksMVVM ()
		{
			InitializeComponent ();

		    _vm = App.ViewChecksViewModel;
			BindingContext = _vm;

			_vm.OnLoadEmployees += LoadEmployees;
			_vm.OnLoadChecks += LoadChecks;
			_vm.OnMoveToView += calling => App.MoveToPage (calling.DestinationView);

			//setup the done events to fire into the view model, since done isn't yet supported
			_entryClockinPasscode.Completed += (sender, e) => {
				var passcode = _entryClockinPasscode.Text;
				if(_vm.ClockinDone.CanExecute(passcode)){
					_vm.ClockinDone.Execute(passcode);
				}
			};

			_entryNewCheckName.Completed += (sender, e) => {
				var custName = _entryNewCheckName.Text;
				if(_vm.NewCheckDone.CanExecute(custName)){
					_vm.NewCheckDone.Execute(custName);
				}
			};

			_entryNoSalePasscode.Completed += (sender, e) => {
				var passcode = _entryNoSalePasscode.Text;
				if(_vm.NoSaleDone.CanExecute(passcode))
				{
					_vm.NoSaleDone.Execute(passcode);
				}
			};

			_entryClockoutPasscode.Completed += (sender, e) => {
				var passcode = _entryClockoutPasscode.Text;
				if(_vm.ClockoutDone.CanExecute(passcode)){
					_vm.ClockoutDone.Execute(passcode);
				}
			};

			Appearing += Reload;
		}

		void LoadEmployees(BaseViewModel vm){
			_empStack.Children.Clear ();
			foreach (var emp in _vm.CurrentEmployees) {
				var color = App.Theme.GetColorForEmployee (emp);
				var button = new Button {
					BackgroundColor = color,
					Text = emp.DisplayName,
					//TextColor = App.Theme.TextColor,
					BorderRadius = App.Theme.BorderRadius,
					HeightRequest = App.Theme.EmployeeButtonHeight,
					CommandParameter = emp,
					Font = App.Theme.ButtonFont
				};

				if (_vm.SelectedEmployee != null && _vm.SelectedEmployee.ID == emp.ID) {
					_selectedEmployeeButton = button;
					button.BackgroundColor = App.Theme.SelectedBackgroundColor;
					button.TextColor = App.Theme.SelectedTextColor;
					if (_vm.EmployeeSelected.CanExecute (emp)) {
						_vm.EmployeeSelected.Execute (emp);
					}
				}

				button.Clicked += EmployeeButtonClicked;
				_empStack.Children.Add (button);
			}
		}
			
		void LoadChecks(BaseViewModel vm){
			_checksGrid.Children.Clear ();

			var col = 0;
			var row = 0;
			foreach (var check in _vm.Checks) {
				var color = App.Theme.GetColorForCheck (check);
				var checkButton = new Button {
					BackgroundColor = color,
					Text = check.DisplayName,
					TextColor = App.Theme.TextColor,
					BorderRadius = App.Theme.BorderRadius,
					HeightRequest = App.Theme.CheckButtonHeight,
					CommandParameter = check,
					Command = _vm.CheckClicked,
					Font = App.Theme.ButtonFont
				};
				_checksGrid.Children.Add (checkButton, col++, row);
				if (col >= App.Theme.NumChecksPerRow) {
					col = 0;
					row++;
				}
			}
		}

		Button _selectedEmployeeButton;
		Button _semiSelectedEmployeeButton;
		void EmployeeButtonClicked(object sender, EventArgs args){
			var button = sender as Button;
			if (button == null) {
				return;
			}

			var emp = button.CommandParameter as IEmployee;
			if(emp == null){
				return;
			}

			//we've got an employee, decide whether or not to turn them on!
			if(_selectedEmployeeButton == null){
				//select the new click
				_selectedEmployeeButton = button;
				_selectedEmployeeButton.BorderWidth = 0;
				_selectedEmployeeButton.BackgroundColor = App.Theme.SelectedBackgroundColor;
				_selectedEmployeeButton.TextColor = App.Theme.SelectedTextColor;

				if (_vm.EmployeeSelected.CanExecute (null)) {
					_vm.EmployeeSelected.Execute (emp);
				}
				if (_semiSelectedEmployeeButton != null) {
					//turn off the other button that's holding on!
					_semiSelectedEmployeeButton.BorderWidth = 0;
				}
			} else {
				var alreadySelectedEmp = _selectedEmployeeButton.CommandParameter as IEmployee;
				if(alreadySelectedEmp == null || alreadySelectedEmp.ID != emp.ID){
					//we are switching from another selected
					_selectedEmployeeButton.BackgroundColor = App.Theme.GetColorForEmployee (alreadySelectedEmp);
					_selectedEmployeeButton.TextColor = App.Theme.TextColor;
					_selectedEmployeeButton.BorderWidth = 0;

					_selectedEmployeeButton = button;
					_selectedEmployeeButton.BackgroundColor = App.Theme.SelectedBackgroundColor;
					_selectedEmployeeButton.TextColor = App.Theme.SelectedTextColor;
					if (_vm.EmployeeSelected.CanExecute (null)) {
						_vm.EmployeeSelected.Execute (emp);
					}
				} else {
					//we've selected again, deselect
					button.BackgroundColor = App.Theme.GetColorForEmployee (emp);
					button.BorderWidth = 5;
					button.BorderColor = App.Theme.SelectedBackgroundColor;
					button.TextColor = App.Theme.TextColor;
					_semiSelectedEmployeeButton = button;

					_selectedEmployeeButton = null;
					if (_vm.EmployeeDeselected.CanExecute (null)) {
						_vm.EmployeeDeselected.Execute (null);
					}
				}
			}
		}
	}
}

