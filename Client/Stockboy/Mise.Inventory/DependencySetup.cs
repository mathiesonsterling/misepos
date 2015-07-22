using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using Autofac;

using Mise.Core;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services.WebServices;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.ViewModels;
using Mise.Core.Common.Services.Implementation.FakeServices;
using Mise.Core.Common.Services.Implementation;
using System.ServiceModel;
using Mise.Core.Entities;
using Mise.Core.Common;


namespace Mise.Inventory
{
	public class DependencySetup
	{
		public static ILogger Logger{get;protected set;}

		/// <summary>
		/// Creates an instance of the AutoFac container
		/// </summary>
		/// <returns>A new instance of the AutoFac container</returns>
		/// <remarks>
		/// https://github.com/autofac/Autofac/wiki
		/// </remarks>
		public IContainer CreateContainer()
		{
			var cb = new ContainerBuilder();

			RegisterDepenencies(cb);

			return cb.Build();
		}

		public static BuildLevel GetBuildLevel(){
			#if DEBUG
			return BuildLevel.Demo;
			#else
			return BuildLevel.QA;
			#endif
		}

		private static IInventoryApplicationWebService GetWebService(IJSONSerializer serial){
			var level = GetBuildLevel ();

			if(level == BuildLevel.Demo){
				return new FakeInventoryWebService();
			}

			Uri uri = null;
			switch(level){
			case BuildLevel.Debugging:
				uri = new Uri ("http://localhost:43499");
				break;
			case BuildLevel.Development:
				uri = new Uri ("http://miseinventoryservicedev.azurewebsites.net");
				break;
			case BuildLevel.QA:
				uri = new Uri ("http://miseinventoryserviceqa.azurewebsites.net");
				break;
			case BuildLevel.Production:
				uri = new Uri ("http://miseinventoryserviceprod.azurewebsites.net");
				break;
			default:
				throw new ArgumentException ();
			}
				
			var webService = new HttpWebServiceClient (uri, "betaDevice", serial, Logger);
			return webService;
		}

		/// <summary>
		/// Registers the depenencies.
		/// </summary>
		/// <param name="cb">Cb.</param>
		protected virtual void RegisterDepenencies(ContainerBuilder cb)
		{
		    cb.RegisterType<XamarinInsightsService>().As<IInsightsService>();

			var serial = new JsonNetSerializer ();
			cb.RegisterInstance (serial).As<IJSONSerializer>();

			cb.RegisterType<XamarinFormsSimpleKeyValueStorage> ().As<IClientKeyValueStorage> ().SingleInstance ();

			// Logger
			if(Logger == null){
				Logger = new DummyLogger ();
			}
			cb.RegisterInstance (Logger).As<ILogger>().SingleInstance ();

			var webService = GetWebService (serial);
			cb.RegisterInstance(webService).As<IInventoryEmployeeWebService>();
			cb.RegisterInstance(webService).As<IInventoryRestaurantWebService>();
			cb.RegisterInstance(webService).As<IVendorWebService>();
			cb.RegisterInstance(webService).As<IPARWebService>();
			cb.RegisterInstance(webService).As<IInventoryWebService>();
			cb.RegisterInstance(webService).As<IReceivingOrderWebService>();
			cb.RegisterInstance(webService).As<IPurchaseOrderWebService>();
			cb.RegisterInstance (webService).As<IApplicationInvitationWebService> ();
			cb.RegisterInstance (webService).As<IAccountWebService> ();

			// DAL
			cb.RegisterType<MemoryClientDAL>().As<IClientDAL>().SingleInstance();

			//Event factory
			//TODO - do we have a restaurant?  if not, use a fake ID and go to register
			var eventFactory = new InventoryAppEventFactory("testDevice", MiseAppTypes.StockboyMobile);
			cb.RegisterInstance(eventFactory).As<IInventoryAppEventFactory>().SingleInstance();

			// Repositories
			cb.RegisterType<ClientEmployeeRepository>().As<IEmployeeRepository>().SingleInstance();
			cb.RegisterType<ClientVendorRepository>().As<IVendorRepository>().SingleInstance();
			cb.RegisterType<ClientRestaurantRepository>().As<IRestaurantRepository>().SingleInstance();
			cb.RegisterType<ClientPARRepository>().As<IPARRepository>().SingleInstance();
			cb.RegisterType<ClientReceivingOrderRepository>().As<IReceivingOrderRepository>().SingleInstance();
			cb.RegisterType<ClientPurchaseOrderRepository>().As<IPurchaseOrderRepository>().SingleInstance();
			cb.RegisterType<ClientInventoryRepository>().As<IInventoryRepository>().SingleInstance();
			cb.RegisterType<ClientApplicationInvitationRepository> ().As<IApplicationInvitationRepository> ().SingleInstance ();
		    cb.RegisterType<ClientRestaurantAccountRepository>().As<IAccountRepository>().SingleInstance();

            //Repository loader
		    cb.RegisterType<RepositoryLoader>().As<IRepositoryLoader>().SingleInstance();

			// Services
			cb.RegisterType<AppNavigation>().As<IAppNavigation>().SingleInstance();
			cb.RegisterType<DefaultThemer>().As<IThemer>().SingleInstance();
			cb.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
			cb.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
			cb.RegisterType<PageFactory>().As<IPageFactory>().SingleInstance();
			cb.RegisterType<VendorService>().As<IVendorService>().SingleInstance();
			cb.RegisterType<InventoryService>().As<IInventoryService> ().SingleInstance ();
			cb.RegisterType<ReceivingOrderService>().As<IReceivingOrderService>().SingleInstance ();
			cb.RegisterType<BeverageItemService> ().As<IBeverageItemService> ().SingleInstance ();
			cb.RegisterType<PARService> ().As<IPARService> ().SingleInstance ();
			cb.RegisterType<DummyDeviceLocationService>().As<IDeviceLocationService>().SingleInstance();
			cb.RegisterType<PurchaseOrderService> ().As<IPurchaseOrderService> ().SingleInstance ();
			cb.RegisterType<CategoriesService> ().As<ICategoriesService> ().SingleInstance ();

			// View Models
			cb.RegisterType<AboutViewModel>().SingleInstance();
			cb.RegisterType<InventoryViewModel>().SingleInstance();
			cb.RegisterType<ItemAddViewModel>().SingleInstance();
			cb.RegisterType<ItemFindViewModel>().SingleInstance();
			cb.RegisterType<ItemScanViewModel>().SingleInstance();
			cb.RegisterType<LoginViewModel>().SingleInstance();
			cb.RegisterType<MainMenuViewModel>().SingleInstance();
			cb.RegisterType<EmployeesManageViewModel>().SingleInstance();
			cb.RegisterType<ParViewModel>().SingleInstance();
			cb.RegisterType<ReceivingOrderViewModel>().SingleInstance();
		    cb.RegisterType<UpdateReceivingOrderLineItemViewModel>().SingleInstance();
			cb.RegisterType<ReportsViewModel>().SingleInstance();
			cb.RegisterType<RestaurantSelectViewModel>().SingleInstance();
			cb.RegisterType<SectionAddViewModel>().SingleInstance();
			cb.RegisterType<SectionSelectViewModel>().SingleInstance();
			cb.RegisterType<VendorAddViewModel>().SingleInstance();
			cb.RegisterType<VendorFindViewModel>().SingleInstance();
			cb.RegisterType<InventoryVisuallyMeasureBottleViewModel>().SingleInstance ();
			cb.RegisterType<UpdateParLineItemViewModel> ().SingleInstance ();
			cb.RegisterType<PurchaseOrderReviewViewModel> ().SingleInstance ();
			cb.RegisterType<UserRegistrationViewModel>().SingleInstance();
			cb.RegisterType<InvitationViewModel>().SingleInstance();
			cb.RegisterType<RestaurantRegistrationViewModel> ().SingleInstance ();
			cb.RegisterType<PurchaseOrderSelectViewModel>().SingleInstance();
			cb.RegisterType<AccountRegistrationViewModel> ().SingleInstance ();
		}
	}
}