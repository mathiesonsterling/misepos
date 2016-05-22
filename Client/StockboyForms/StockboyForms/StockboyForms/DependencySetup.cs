
using Autofac;

using Mise.Core;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.Services;
using Mise.Core.Client.Services.Implementation;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.ViewModels;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Common;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Services;
using Mise.Inventory.ViewModels.Reports;
using Mise.Inventory.Services.Implementation.WebServiceClients.Azure;

namespace StockboyForms
{
	public class DependencySetup
	{
		public static ILogger Logger{get;protected set;}
		protected ISQLite SqlLiteConnection{ get; set; }

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
			return BuildLevel.Production;
			#else
			return BuildLevel.Production;
			#endif
		}

		protected static AzureServiceLocation GetWebServiceLocation(){
			var level = GetBuildLevel ();

			return AzureServiceLocator.GetAzureMobileServiceLocation (level);
		}

		protected static void RegisterWebService (ContainerBuilder cb, IInventoryWebService webService)
		{
			cb.RegisterInstance (webService).As<IInventoryEmployeeWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IInventoryRestaurantWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IVendorWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IParWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IInventoryWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IReceivingOrderWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IPurchaseOrderWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IApplicationInvitationWebService> ().SingleInstance ();
			cb.RegisterInstance (webService).As<IAccountWebService> ().SingleInstance ();
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

			// DAL
			if (SqlLiteConnection != null) {
				cb.RegisterType<SQLiteClietDAL> ().As<IClientDAL> ().SingleInstance ();
			} else {
				cb.RegisterType<MemoryClientDAL> ().As<IClientDAL> ().SingleInstance ();
			}

			//Event factory - rstaurant will be set later
			var eventFactory = new InventoryAppEventFactory("testDevice", MiseAppTypes.StockboyMobile);
			cb.RegisterInstance(eventFactory).As<IInventoryAppEventFactory>().SingleInstance();

			// Repositories
			cb.RegisterType<ClientEmployeeRepository>().As<IEmployeeRepository>().SingleInstance();
			cb.RegisterType<ClientVendorRepository>().As<IVendorRepository>().SingleInstance();
			cb.RegisterType<ClientRestaurantRepository>().As<IRestaurantRepository>().SingleInstance();
			cb.RegisterType<ClientParRepository>().As<IParRepository>().SingleInstance();
			cb.RegisterType<ClientReceivingOrderRepository>().As<IReceivingOrderRepository>().SingleInstance();
			cb.RegisterType<ClientPurchaseOrderRepository>().As<IPurchaseOrderRepository>().SingleInstance();
			cb.RegisterType<ClientInventoryRepository>().As<IInventoryRepository>().SingleInstance();
			cb.RegisterType<ClientApplicationInvitationRepository> ().As<IApplicationInvitationRepository> ().SingleInstance ();
		    cb.RegisterType<ClientRestaurantAccountRepository>().As<IAccountRepository>().SingleInstance();

            //Repository loader
		    cb.RegisterType<RepositoryLoader>().As<IRepositoryLoader>().SingleInstance();

			// Services
			cb.RegisterType<AppNavigation>().As<IAppNavigation>().SingleInstance();
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
			cb.RegisterType<ReportsService> ().As<IReportsService> ().SingleInstance ();
			cb.RegisterType<FunFactService> ().As<IFunFactService> ().SingleInstance ();
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
			cb.RegisterType<ReportsByInventoryViewModel> ().SingleInstance ();
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
			cb.RegisterType<ReportResultsViewModel> ().SingleInstance ();
			cb.RegisterType<SelectCompletedInventoryViewModel> ().SingleInstance ();
			cb.RegisterType<AuthorizeCreditCardViewModel> ().SingleInstance ();
			cb.RegisterType<RestaurantLoadingViewModel> ().SingleInstance ();
			cb.RegisterType<SettingsViewModel> ().SingleInstance ();
			cb.RegisterType<ChangePasswordViewModel> ().SingleInstance ();
			cb.RegisterType<AccountRegistrationWithCreditCardViewModel> ().SingleInstance ();
            cb.RegisterType<EULAViewModel>().SingleInstance();
            cb.RegisterType<AdminMenuViewModel>().SingleInstance();
		}
	}
}