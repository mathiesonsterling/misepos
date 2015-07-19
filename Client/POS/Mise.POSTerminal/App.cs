using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Core;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Client.ApplicationModel.Implementation;
using Mise.Core.Client.Services;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.POSTerminal.Pages;
using Mise.POSTerminal.Services;
using Mise.POSTerminal.Theme;
using Mise.POSTerminal.ViewModels;

namespace Mise.POSTerminal
{
	public class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mise.POSTerminal.App"/> class.
		/// </summary>
		/// <param name="creditCardReaderService">Credit card reader service.</param>
		/// <param name="dal">The Dal.</param>
		/// <param name="logger">The Logger.</param>
		public App(ICreditCardReaderService creditCardReaderService, IClientDAL dal, ILogger logger = null)
		{
			NavigationPage.SetHasNavigationBar(this, false);	

			App.CreditCardReaderService = creditCardReaderService;
			App.ClientDAL = dal;

			if (logger != null) {
				_logger = logger;
			}

			MainPage = new NavigationPage(new EmployeesPage());

			Navigation.Navi = MainPage.Navigation;
			Navigation.CurrentPage = MainPage;
		}

		public static ICreditCardReaderService CreditCardReaderService;
		public static IClientDAL ClientDAL;

		static NavigationService _navigationService;

		public static NavigationService Navigation {
			get {
				if (_navigationService == null) {
					_navigationService = new NavigationService();
				}
				return _navigationService;
			}
			set {
				_navigationService = value;
			}
		}

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

		public static ITerminalApplicationModel LoadAppModel()
		{
			Logger.Log("Creating app model and loading . . . .", LogLevel.Debug);
			//get the repositories, services, etc
			var unitOfWork = DependencyService.Get<IUnitOfWork>();
			var creditCardProcessor = DependencyService.Get<ICreditCardProcessorService>();
			creditCardProcessor.Logger = Logger;

			//var termService = new JsonRestaurantServiceClient (serial, machineID);
			var termService = new FakeDomineesRestaurantServiceClient();
			var cashDrawerService = DependencyService.Get<ICashDrawerService>();
			var res = new TerminalApplicationModel(unitOfWork, 
				          Logger, 
				          App.ClientDAL,
				          cashDrawerService,
				          App.CreditCardReaderService, 
				          creditCardProcessor,
						  //TODO get printer implementation
				          null,
				          new FakeNYCSalesTaxService(), 
				          termService);

			Logger.Log("View model created", LogLevel.Debug);

			return  res;
		}

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

		// TODO: Find another place for this to live CEB

		public static void LoadEmployeeColors(IEnumerable<Tuple<Guid, int, int, int>> items)
		{
			//ALSO load our colors for our employees, and other theme information
			//TODO combine our themes and have colors for both there?
			foreach (var emp in items) {
				Theme.LoadColorForEmployee(emp.Item1, emp.Item2, emp.Item3, emp.Item4);
			}
		}

		#endregion

		#region ViewModels

		private static readonly Lazy<ViewChecksViewModel> ViewChecksInstance = new Lazy<ViewChecksViewModel>(() => new ViewChecksViewModel(Logger, AppModel));

		public static ViewChecksViewModel ViewChecksViewModel {
			get { return ViewChecksInstance.Value; }
		}

		private static readonly Lazy<OrderOnCheckViewModel> OrderOnCheckInstance = new Lazy<OrderOnCheckViewModel>(() => new OrderOnCheckViewModel(Logger, AppModel));

		public static OrderOnCheckViewModel OrderOnCheckViewModel {
			get { return OrderOnCheckInstance.Value; }
		}


		private static readonly Lazy<DrawerOpenViewModel> DrawerOpenInstance = new Lazy<DrawerOpenViewModel>(() => new DrawerOpenViewModel(Logger, AppModel));

		public static DrawerOpenViewModel DrawerOpenViewModel {
			get { return DrawerOpenInstance.Value; }
		}

		#endregion

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnSleep()
		{
			base.OnSleep();
		}

		protected override void OnResume()
		{
			base.OnResume();
		}
	}
}
