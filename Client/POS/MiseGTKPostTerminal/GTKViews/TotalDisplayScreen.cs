using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Mise.Core.Client.ViewModel;
using Mise.Core.Services;
namespace MiseGTKPostTerminal.GTKViews
{
	public class TotalDisplayScreen :IGTKTerminalView
	{
		#region IGTKTerminalView implementation

		public void LoadView ()
		{
			var mainDiv = new HBox();
			//color the main box here?
			_window.Add(mainDiv);

            //display this total

            //wait the amount of time we should, then update to close this window
		    Task.Factory.StartNew(() =>
		        {
		            Thread.Sleep(_theme.DisplayCashDrawerTime);
		            _viewModel.GoToHomeScreen();

		            _updateView();
		        });
		}

		public TerminalViewTypes Type {
			get {
				return TerminalViewTypes.PaymentScreen;
			}
		}

		#endregion

		private readonly Window _window;
		private readonly ITerminalApplicationModel _viewModel;
		private readonly DefaultTheme _theme;
		private readonly ILogger _logger;
		private readonly UpdateViewCallback _updateView;

		public TotalDisplayScreen (DefaultTheme theme, ITerminalApplicationModel viewModel, Window window, ILogger logger, UpdateViewCallback updateView)
		{
			_theme = theme;
			_viewModel = viewModel;
			_window = window;
			_logger = logger;
			_updateView = updateView;
		}
	}
}

