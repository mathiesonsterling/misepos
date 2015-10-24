using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Mise.Core.Client.Services;
using Mise.Core.Common.Events;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.ViewModels;
using Mise.Inventory.ViewModels.Reports;
using Xamarin.Forms;

namespace Mise.Inventory
{
    public class App : Application
    {
        static IContainer _container;
        public static Color AccentColor { get; set; }

        /// <summary>
        /// The restaurant the application is dealing with.  If null, we're not yet registered!
        /// </summary>
        /// <value>The restaurant I.</value>
        private static Guid? RestaurantID { get; set; }

        public static string DeviceID{ get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="Mise.Inventory.App"/> class.
        /// </summary>
        public App(DependencySetup setup)
        {
            RestaurantID = null;
            _container = setup.CreateContainer();

            var appNavigation = Resolve<IAppNavigation>();
            var pageFactory = Resolve<IPageFactory>();
            var initialPage = pageFactory.GetPage(appNavigation.DefaultPage);

            var navigationService = Resolve<INavigationService>() as NavigationService;
            if (navigationService == null)
            {
                throw new Exception("Cannot resolve navigation service");
            }
            var insights = Resolve<IInsightsService>();
            if (insights == null)
            {
                throw new Exception("Cannot resolve insights service");
            }
				

            MainPage = new NavigationPage(initialPage);

            navigationService.Navi = MainPage.Navigation;
            navigationService.CurrentPage = initialPage;

			LoadDeviceID ();



            LoadRepositoriesVoid();

            //if we've got a stored login, load it up
			AttemptToLoginSavedEmployee (appNavigation);
            
        }
			

		static async void LoadDeviceID ()
		{
			//get the device ID
			var kVService = Resolve<IClientKeyValueStorage> ();
			var item = kVService.GetID ("DEVICE_ID");
			if (item == null) {
				item = Guid.NewGuid ();
				await kVService.SetID ("DEVICE_ID", item.Value);
			}
			//set the ev factory
			var evFactory = Resolve<IInventoryAppEventFactory> ();
			evFactory.SetDeviceID (item.ToString ());
			DeviceID = item.ToString ();
		}

        protected override async void OnSleep()
        {
			try{
				//TODO fire a sync if we're online and need one

				//check if we have unsaved work!
				var reposLoader = GetLoader();
				await reposLoader.SaveOnSleep();
			} catch(Exception e){
				var logger = _container.Resolve<ILogger>();
				logger?.HandleException(e);
			}
        }

        #region View Models

        public static AboutViewModel AboutViewModel => Resolve<AboutViewModel>();

        public static InventoryViewModel InventoryViewModel => Resolve<InventoryViewModel>();

        public static ItemAddViewModel ItemAddViewModel => Resolve<ItemAddViewModel>();

        public static ItemFindViewModel ItemFindViewModel => Resolve<ItemFindViewModel>();

        public static ItemScanViewModel ItemScanViewModel => Resolve<ItemScanViewModel>();

        public static LoginViewModel LoginViewModel => Resolve<LoginViewModel>();

        public static MainMenuViewModel MainMenuViewModel => Resolve<MainMenuViewModel>();

        public static EmployeesManageViewModel EmployeesManageViewModel => Resolve<EmployeesManageViewModel>();

        public static ParViewModel ParViewModel => Resolve<ParViewModel>();

        public static ReceivingOrderViewModel ReceivingOrderViewModel => Resolve<ReceivingOrderViewModel>();

        public static RestaurantSelectViewModel RestaurantSelectViewModel => Resolve<RestaurantSelectViewModel>();

        public static SectionAddViewModel SectionAddViewModel => Resolve<SectionAddViewModel>();

        public static SectionSelectViewModel SectionSelectViewModel => Resolve<SectionSelectViewModel>();

        public static VendorAddViewModel VendorAddViewModel => Resolve<VendorAddViewModel>();

        public static VendorFindViewModel VendorFindViewModel => Resolve<VendorFindViewModel>();

        public static InventoryVisuallyMeasureBottleViewModel InventoryVisuallyMeasureBottleViewModel => Resolve<InventoryVisuallyMeasureBottleViewModel>();
        public static UpdateParLineItemViewModel UpdateParLineItemViewModel => Resolve<UpdateParLineItemViewModel>();

        public static UpdateReceivingOrderLineItemViewModel UpdateReceivingOrderLineItemViewModel => Resolve<UpdateReceivingOrderLineItemViewModel>();
        public static PurchaseOrderReviewViewModel PurchaseOrderReviewViewModel => Resolve<PurchaseOrderReviewViewModel>();

        public static UserRegistrationViewModel UserRegistrationViewModel => Resolve<UserRegistrationViewModel>();
        public static InvitationViewModel InvitationViewModel => Resolve<InvitationViewModel>();
        public static RestaurantRegistrationViewModel RestaurantRegistrationViewModel => Resolve<RestaurantRegistrationViewModel>();
        public static PurchaseOrderSelectViewModel PurchaseOrderSelectViewModel => Resolve<PurchaseOrderSelectViewModel>();
        public static AccountRegistrationViewModel AccountRegistrationViewModel => Resolve<AccountRegistrationViewModel>();

        public static ReportsViewModel ReportsViewModel => Resolve<ReportsViewModel>();
		public static ReportsByInventoryViewModel ReportsByInventoryViewModel => Resolve<ReportsByInventoryViewModel>();
        public static SelectCompletedInventoryViewModel SelectCompletedInventoryViewModel => Resolve<SelectCompletedInventoryViewModel> ();
        public static ReportResultsViewModel ReportResultsViewModel => Resolve<ReportResultsViewModel> ();

        public static AuthorizeCreditCardViewModel AuthorizeCreditCardViewModel => Resolve<AuthorizeCreditCardViewModel>();
		public static RestaurantLoadingViewModel RestaurantLoadingViewModel => Resolve<RestaurantLoadingViewModel>();
		public static SettingsViewModel SettingsViewModel => Resolve<SettingsViewModel>();
		public static ChangePasswordViewModel ChangePasswordViewModel => Resolve<ChangePasswordViewModel>();
		#endregion

        private static T Resolve<T>()
        {
            try
            {
                return _container.Resolve<T>();

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Error resolving type {0}:  {1}", typeof(T).FullName, ex);

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

		private static async void AttemptToLoginSavedEmployee(IAppNavigation appNavigation)
		{
			var loginService = _container.Resolve<ILoginService>();
			if (loginService != null) {
				var hasSaved = await loginService.LoadSavedEmployee ();
				if(hasSaved){
					await MainMenuViewModel.OnAppearing ();
					await appNavigation.ShowMainMenu ();
				} 
				else{
					await appNavigation.ShowLogin();
				}
			}
		}        

		private async void LoadRepositoriesVoid()
        {
            await LoadRepositories();
        }

        static async Task LoadRepositories()
        {
            try
            {
				var loader = GetLoader();
                await loader.LoadRepositories(RestaurantID);
            }
            catch (Exception e)
            {
                var logger = _container.Resolve<ILogger>();
                logger?.HandleException(e);
                throw;
            }
        }

		static IRepositoryLoader GetLoader(){
			return new RepositoryLoader(
				_container.Resolve<ILogger>(),
				_container.Resolve<IEmployeeRepository>(),
				_container.Resolve<IApplicationInvitationRepository>(), _container.Resolve<IVendorRepository>(),
				_container.Resolve<IInventoryAppEventFactory>(), _container.Resolve<IRestaurantRepository>(),
				_container.Resolve<IParRepository>(), _container.Resolve<IInventoryRepository>(),
				_container.Resolve<IReceivingOrderRepository>(), _container.Resolve<IPurchaseOrderRepository>(),
				_container.Resolve<IBeverageItemService>());
		}
    }
}