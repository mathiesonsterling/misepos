using System;
using System.Collections.Specialized;
using System.Linq;

using Xamarin.Forms;

using Mise.POSTerminal.ViewModels;
using Mise.Core.Entities.People;

namespace Mise.POSTerminal.Pages
{
	public partial class EmployeesPage : ContentPage
	{
		readonly int EMPLOYEES_PER_COLUMN = 5;
		readonly EmployeesViewModel _vm;

		public EmployeesPage()
		{
			InitializeComponent();

			_vm = new EmployeesViewModel(App.Logger, App.AppModel, App.Navigation);
			BindingContext = _vm;

			_vm.CurrentEmployees.CollectionChanged += UpdateEmployees;
		}

		void UpdateEmployees(object sender, NotifyCollectionChangedEventArgs e)
		{
			var columns = Math.Ceiling((decimal)_vm.CurrentEmployees.Count() / EMPLOYEES_PER_COLUMN);
			var grid = new Grid {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition{ Height = GridLength.Auto },
					new RowDefinition{ Height = GridLength.Auto },
					new RowDefinition{ Height = GridLength.Auto },
					new RowDefinition{ Height = GridLength.Auto },
					new RowDefinition{ Height = GridLength.Auto },
				},
				ColumnDefinitions = new ColumnDefinitionCollection(),
			};

			EmployeesGrid.Children.Clear();

//			App.Logger.Log(string.Format("We will need {0}", columns));

			for (int i = 0; i < columns; ++i) {
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			var idx = 0;
			var row = -1;
			var col = 0;
			foreach (IEmployee employee in _vm.CurrentEmployees) {
				row = idx % EMPLOYEES_PER_COLUMN;

				var button = new Button {
					Text = employee.DisplayName,
				};

				grid.Children.Add(button, col, row);

				idx++;
				if (row == 4) {
					++col;
				}
			}

			EmployeesGrid.Children.Add(grid);
		}
	}
}

