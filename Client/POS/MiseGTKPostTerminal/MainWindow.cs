using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;

using Gtk;
using Mise.Core.Services;
using Mise.Core.Services.Implementation;
using Mise.Core.Client.Services.Implementation;
using Mise.Core.Client.ViewModel;
using MiseGTKPostTerminal;
using MiseGTKPostTerminal.GTKViews;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

// ReSharper disable CheckNamespace
public partial class MainWindow : Window
// ReSharper restore CheckNamespace
{
    private readonly ITerminalApplicationModel _viewModel;
    private readonly ILogger _logger;
    private readonly DefaultTheme _theme;

    private readonly Dictionary<TerminalViewTypes, IGTKTerminalView> _views;

	private DateTime _lastUpdated = DateTime.Now;
	private bool _screenSaverOn = false;

    public MainWindow(DefaultTheme theme)
        : base(WindowType.Toplevel)
    {
        // ReSharper disable DoNotCallOverridableMethodsInConstructor
        Build();
        // ReSharper restore DoNotCallOverridableMethodsInConstructor
        _logger = new DummyLogger();
		var serviceClient = new FakeRestaurantServiceClient ();
        _viewModel = new TerminalViewModel(serviceClient);
        _theme = theme;

        var updateDel = new UpdateViewCallback(UpdateView);
        //make our view objects
        _views = new Dictionary<TerminalViewTypes, IGTKTerminalView>
            {
                {TerminalViewTypes.ViewTabs, new ViewTabs(_theme, _viewModel, this, _logger, updateDel)},
                {TerminalViewTypes.OrderOnTab, new OrderOnTabFixed(_theme, _viewModel, this, _logger, updateDel)},
				{TerminalViewTypes.PaymentScreen, new TotalDisplayScreen(_theme, _viewModel, this, _logger, updateDel)}
            };

        UpdateView();

		//start our ad screensaver
		Task.Factory.StartNew( () => {
			while(true)
			{
				if(DateTime.Now - _lastUpdated > _theme.TimeTillScreenSaver)
				{
					ShowScreenSaver();
				}
				Thread.Sleep(1000);
			}
		});
    }

	private void ShowScreenSaver()
	{
		_screenSaverOn = true;
		//TODO log when we started the screen saver, and when we stop
		if (Child != null) {
			Remove (Child);
		}
	}

    /// <summary>
    /// Called to tell us to update our view to match our model
    /// </summary>
    private void UpdateView()
    {
        //set our window colors here
        ModifyBg(StateType.Normal, _theme.WindowBackground);
        ModifyBg(StateType.Active, _theme.WindowBackground);
		WidthRequest = _theme.WindowWidth;
		HeightRequest = _theme.WindowHeight;

        if (_views.ContainsKey(_viewModel.CurrentTerminalViewTypeToDisplay) == false)
        {
            //error here!
            _logger.Log("Error, don't have a view for " + _viewModel.CurrentTerminalViewTypeToDisplay);
            return;
        }

        if (Child != null)
        {
            Remove(Child);
        }

        var view = _views[_viewModel.CurrentTerminalViewTypeToDisplay];
        view.LoadView();

		_lastUpdated = DateTime.Now;
        ShowAll();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

}
