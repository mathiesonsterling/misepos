using System;
using Android.App;
using Android.Runtime;
using  Mise.Core.Client.ApplicationModel;
using Mise.Core.Client.ApplicationModel.Implementation;
using MiseAndroidPOSTerminal.AndroidViews;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Services;
using Mise.Core.Common.Services.Implementation;
using MiseAndroidPOSTerminal.Services;
using Mise.Core;
using Mise.Core.Common.Services.Implementation.Serialization;
using Xamarin;
using System.IO;

using Mise.AndroidCommon.Services;
using Mise.Core.Client.Services;
using Mise.Core.ValueItems;
using Mise.AndroidCommon;

namespace MiseAndroidPOSTerminal
{
	[Application(Debuggable = true, Label="MisePOS Terminal", ManageSpaceActivity = typeof(ViewChecks))]
	public class POSTerminalApplication : Application
	{ 
		public static POSTerminalApplication Current { get; private set; }

		//TODO replace this with a Lazy instance?
		ITerminalApplicationModel _applicationModelInstance;
		object _vmLock = new object ();

		object _themeLock = new object ();
		IMiseAndroidTheme _themeInstance;

		public ILogger Logger{get;private set;}
		public IUnitOfWork UnitOfWork{ get; private set;}
		public ICreditCardProcessorService CreditCardProcessor{ get; private set;}

		public void CreateApplicationModel ()
		{
			Logger.Log ("Creating view model and loading . . . .", LogLevel.Debug);
			UnitOfWork = new AndroidUnitOfWork {Logger = Logger};
			CreditCardProcessor = new FakeCreditCardProcessingService (Logger);
		
			var serial = new JsonNetSerializer();



			var machineID = Android.Provider.Settings.Secure.GetString(ContentResolver, 
				Android.Provider.Settings.Secure.AndroidId); 
			var termService = new FakeDomineesRestaurantServiceClient ();
			var dal = new AndroidSQLLiteDAL (BaseContext, Logger, serial);

			//TODO figure out what credit card service we should be using
			var creditCardService = CreditCardReaderServiceFactory.GetCreditCardReaderService(CreditCardReaderType.AudioReader, Logger);

			//var printerService = new EpsonPrinterService(Logger, "", this.ApplicationContext);
			IPrinterService printerService = null;

			_applicationModelInstance = new TerminalApplicationModel (UnitOfWork, 
				Logger, 
				dal,
			    new AndroidCashDrawerService(),
				creditCardService, 
				CreditCardProcessor,
				printerService,
			    new FakeNYCSalesTaxService(), 
				termService);

		}

		public ITerminalApplicationModel ApplicationModel{ 
			get { 
				if (_applicationModelInstance == null) {
					lock (_vmLock) {
						if (_applicationModelInstance == null) {
							CreateApplicationModel ();
						}
					}
				}
				return _applicationModelInstance;
			} 
		}

		public IMiseAndroidTheme POSTheme{
			get{ 
				if (_themeInstance == null) {
					lock (_themeLock) {
						if (_themeInstance == null) {
							_themeInstance = new DefaultTheme ();
						}
					}
				}
				return _themeInstance;
			}
		}
			

		public POSTerminalApplication (IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			Current = this;
		}

		public override void OnCreate ()
		{
			try
			{
				//TODO update
				//Xamarin.Insights.Initialize("8abca5f28c73a3faefd9eddb5d2053ba819ba11d", this);
				Logger = new AndroidClientLogger ();
				base.OnCreate ();

				var sqliteFilename = "TaskDB.db3";
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var path = Path.Combine(libraryPath, sqliteFilename);
			
				var serial = new JsonNetSerializer();
				//EntitiesDB = new EntitiesDatabase(conn, serial);
			}
			catch(Exception e) {
				//TODO log this
				if (Logger != null) {
					Logger.HandleException (e, LogLevel.Fatal);
				} else {
					if (e != null) {
						Console.Error.WriteLine (e.Message);
					}
				}
			}
		}
	}
}

