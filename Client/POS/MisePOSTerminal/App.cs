using System;
using System.Collections.Generic;
using MisePOSTerminal.ViewModels;
using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Client.ApplicationModel.Implementation;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Services;
using Mise.Core.Client.Services;
using Mise.Core.Common.Services;
using Mise.Core;
using Mise.Core.Common.Services.Implementation;

using MisePOSTerminal.Pages;
using MisePOSTerminal.Theme;

namespace MisePOSTerminal
{
	public static class App
	{
		public static bool XamarinFormsInitted{ get; set; }

		public static Page GetMainPage()
		{
			return new ViewChecksMVVM();
		}

		public static Action<TerminalViewTypes> MoveToPageFunc{ get; set; }

		/// <summary>
		/// Let's us centralize whether we do the xamarin pages, or move to another activity
		/// </summary>
		/// <param name="type">Type.</param>
		public static void MoveToPage(TerminalViewTypes type)
		{
			MoveToPageFunc.Invoke(type);
		}

		/// <summary>
		/// Moves us to a Xamarin forms page - if it's XF it MUST be in this function for now
		/// </summary>
		/// <returns>The page for type.</returns>
		/// <param name="type">Type.</param>
		public static Page GetPageForType(TerminalViewTypes type)
		{
			//we construct new everytime - Xamarin forms doesn't like when we reuse instances
			switch (type) {
			case TerminalViewTypes.ViewChecks:
				return new ViewChecksMVVM ();
			case TerminalViewTypes.ClosedChecks:
				return new ClosedChecks();
			case TerminalViewTypes.ClockInPage:
				return new ClockInPage();
			case TerminalViewTypes.EmployeePage:
				return new EmployeePage();
			default:
				throw new ArgumentException("Dont have value for type " + type);
			}
		}

		#region TerminalApplicationModel

		static ITerminalApplicationModel _appModelInstance;

		public static ITerminalApplicationModel AppModel { 
			get { 
				if (_appModelInstance == null) {
					_appModelInstance = LoadAppModel();
				}
				return _appModelInstance;
			} 
			set {
				_appModelInstance = value;
			}
		}

		static ILogger _logger;

		public static ILogger Logger {
			get {
				if (_logger == null) {
					_logger = DependencyService.Get<ILogger>();
				}
				return _logger;
			}
		}

		public static void LoadEmployeeColors(IEnumerable<Tuple<Guid, int, int, int>> items)
		{
			//ALSO load our colors for our employees, and other theme information
			//TODO combine our themes and have colors for both there?
			foreach (var emp in items) {
				Theme.LoadColorForEmployee(emp.Item1, emp.Item2, emp.Item3, emp.Item4);
			}
		}

		public static ITerminalApplicationModel LoadAppModel()
		{
			//get the repositories, services, etc
			var serial = new JsonNetSerializer();


			Logger.Log("Creating view model and loading . . . .", LogLevel.Debug);
			var unitOfWork = DependencyService.Get<IUnitOfWork>();
			var creditCardProcessor = DependencyService.Get<ICreditCardProcessorService>();
			var dal = DependencyService.Get<IClientDAL>();

			//finish injecting our DAL
			dal.Logger = Logger;
			dal.Serializer = serial;

			//var termService = new JsonRestaurantServiceClient (serial, machineID);
			var termService = new FakeDomineesRestaurantServiceClient();
			var cashDrawerService = DependencyService.Get<ICashDrawerService>();
			var ccHardwareService = DependencyService.Get<ICreditCardReaderService>();
			var res = new TerminalApplicationModel(unitOfWork, 
				          Logger, 
				          dal,
				          cashDrawerService,
				          ccHardwareService, 
				          creditCardProcessor,
                          //TODO get printer implementation
                          null,
				          new FakeNYCSalesTaxService(), 
				          termService);

			Logger.Log("View model created", LogLevel.Debug);

			return  res;
		}

		#endregion

		#region Theme

		static ITheme _themeInstance;

		public static ITheme Theme {
			get {
				if (_themeInstance == null) {
					_themeInstance = new DefaultMiseTheme();
				}
				return _themeInstance;
			}
		}

		#endregion

		#region ViewModels

		private static readonly Lazy<ViewChecksViewModel> ViewChecksInstance = new Lazy<ViewChecksViewModel>(() => new ViewChecksViewModel(Logger, AppModel)); 
	    public static ViewChecksViewModel ViewChecksViewModel
	    {
            get { return ViewChecksInstance.Value; }
	    }

           private static readonly Lazy<OrderOnCheckViewModel> OrderOnCheckInstance = new Lazy<OrderOnCheckViewModel>(() => new OrderOnCheckViewModel(Logger, AppModel));

	    public static OrderOnCheckViewModel OrderOnCheckViewModel
	    {
            	get { return OrderOnCheckInstance.Value; }
	    }


		private static readonly Lazy<DrawerOpenViewModel> DrawerOpenInstance = new Lazy<DrawerOpenViewModel>(() => new DrawerOpenViewModel(Logger, AppModel));

		public static DrawerOpenViewModel DrawerOpenViewModel {
			get { return DrawerOpenInstance.Value; }
		}

		#endregion
	}
}

