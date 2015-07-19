using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.POSTerminal.ViewModels;

namespace Mise.POSTerminal.Pages
{
	public partial class ClosedChecks : ContentPage
	{
		readonly ClosedChecksViewModel _vm;

		public ClosedChecks()
		{
			InitializeComponent();

			_vm = new ClosedChecksViewModel(App.Logger, App.AppModel);
			_vm.OnMoveToView += calling => {
				var realVM = calling as ClosedChecksViewModel;
				App.Navigation.PushAsync(realVM.DestinationView, true);
			};
			_vm.OnLoadClosedChecks += calling => LoadClosedChecks(grdClosedChecks);


			BindingContext = _vm;

			Appearing += (sender, e) => LoadClosedChecks(grdClosedChecks);
				
		}

		public void LoadClosedChecks(Grid grid)
		{
			//TODO we might want a xamarin table so we can get sorting in here
			//question becomes how do we get touches then?
			//get each, ordered by the status
			var row = 1;
			grid.Children.Clear();

			foreach (var check in _vm.ClosedChecks) {
				var color = App.Theme.GetColorForCheck(check);
				//make a check button
				var checkButton = new Button {
					Command = _vm.CheckClicked,
					Text = check.DisplayName,
					TextColor = App.Theme.TextColor,
					BackgroundColor = color,
					CommandParameter = check,
					Font = App.Theme.ButtonFont,
					HeightRequest = App.Theme.CheckButtonHeight
				};
				grid.Children.Add(checkButton, 0, row);

				//make a label for the status
				var statusLabel = new Label {
					Text = check.PaymentStatus.ToString(),
					TextColor = App.Theme.GetColorForPaymentStatus(check.PaymentStatus),
					Font = App.Theme.ButtonFont,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center,
				};
				grid.Children.Add(statusLabel, 1, row);

				//time closed?
				var timeLabel = new Label {
					Text = check.LastUpdatedDate.ToLocalTime().ToString("t"),
					Font = App.Theme.ButtonFont,
					XAlign = TextAlignment.Center,
					YAlign = TextAlignment.Center
				};
				grid.Children.Add(timeLabel, 2, row);
				//closed by?

				row++;
			}
		}
	}
}

