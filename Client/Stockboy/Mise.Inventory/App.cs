using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using Mise.Core.Common.Events;
using Mise.Core.Repositories;
using Mise.Inventory.Services.Implementation;
using Xamarin.Forms;

using Mise.Inventory.Services;
using Mise.Inventory.ViewModels;
using Mise.Core.Services;

namespace Mise.Inventory
{
    public class App : Application
    {
        static IContainer _container;
        public static Color AccentColor { get; private set; }

        /// <summary>
        /// The restaurant the application is dealing with.  If null, we're not yet registered!
        /// </summary>
        /// <value>The restaurant I.</value>
        private static Guid? RestaurantID { get; set; }

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

            AccentColor = Resolve<IThemer>().AccentColor;

            MainPage = new NavigationPage(initialPage);

            navigationService.Navi = MainPage.Navigation;
            navigationService.CurrentPage = initialPage;

            LoadRepositoriesVoid();

            //if we've got a stored login, load it up
            var loginService = _container.Resolve<ILoginService>();
            if (loginService != null)
            {
                loginService.OnAppStarting();
            }
            appNavigation.ShowLogin();
        }

        #region View Models

        public static AboutViewModel AboutViewModel { get { return Resolve<AboutViewModel>(); } }

        public static InventoryViewModel InventoryViewModel { get { return Resolve<InventoryViewModel>(); } }

        public static ItemAddViewModel ItemAddViewModel { get { return Resolve<ItemAddViewModel>(); } }

        public static ItemFindViewModel ItemFindViewModel { get { return Resolve<ItemFindViewModel>(); } }

        public static ItemScanViewModel ItemScanViewModel { get { return Resolve<ItemScanViewModel>(); } }

        public static LoginViewModel LoginViewModel { get { return Resolve<LoginViewModel>(); } }

        public static MainMenuViewModel MainMenuViewModel { get { return Resolve<MainMenuViewModel>(); } }

        public static EmployeesManageViewModel EmployeesManageViewModel { get { return Resolve<EmployeesManageViewModel>(); } }

        public static ParViewModel PARViewModel { get { return Resolve<ParViewModel>(); } }

        public static ReceivingOrderViewModel ReceivingOrderViewModel { get { return Resolve<ReceivingOrderViewModel>(); } }

        public static ReportsViewModel ReportsViewModel { get { return Resolve<ReportsViewModel>(); } }

        public static RestaurantSelectViewModel RestaurantSelectViewModel { get { return Resolve<RestaurantSelectViewModel>(); } }

        public static SectionAddViewModel SectionAddViewModel { get { return Resolve<SectionAddViewModel>(); } }

        public static SectionSelectViewModel SectionSelectViewModel { get { return Resolve<SectionSelectViewModel>(); } }

        public static VendorAddViewModel VendorAddViewModel { get { return Resolve<VendorAddViewModel>(); } }

        public static VendorFindViewModel VendorFindViewModel { get { return Resolve<VendorFindViewModel>(); } }

        public static InventoryVisuallyMeasureItemImprovedViewModel InventoryVisuallyMeasureItemImprovedViewModel { get { return Resolve<InventoryVisuallyMeasureItemImprovedViewModel>(); } }
        public static InventoryVisuallyMeasureBottleViewModel InventoryVisuallyMeasureBottleViewModel { get { return Resolve<InventoryVisuallyMeasureBottleViewModel>(); } }
        public static UpdateParLineItemViewModel UpdateParLineItemViewModel { get { return Resolve<UpdateParLineItemViewModel>(); } }
        public static UpdateReceivingOrderLineItemViewModel UpdateReceivingOrderLineItemViewModel
        {
            get
            {
                return Resolve<UpdateReceivingOrderLineItemViewModel>();
            }
        }
        public static PurchaseOrderReviewViewModel PurchaseOrderReviewViewModel { get { return Resolve<PurchaseOrderReviewViewModel>(); } }

        public static UserRegistrationViewModel UserRegistrationViewModel { get { return Resolve<UserRegistrationViewModel>(); } }
        public static InvitationViewModel InvitationViewModel { get { return Resolve<InvitationViewModel>(); } }
        public static RestaurantRegistrationViewModel RestaurantRegistrationViewModel { get { return Resolve<RestaurantRegistrationViewModel>(); } }
        public static PurchaseOrderSelectViewModel PurchaseOrderSelectViewModel { get { return Resolve<PurchaseOrderSelectViewModel>(); } }
        public static AccountRegistrationViewModel AccountRegistrationViewModel { get { return Resolve<AccountRegistrationViewModel>(); } }
        #endregion

        public static T Resolve<T>()
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

        public async void LoadRepositoriesVoid()
        {
            await LoadRepositories();
        }

        static async Task LoadRepositories()
        {
            try
            {
                var loader = new RepositoryLoader(_container.Resolve<IEmployeeRepository>(),
                    _container.Resolve<IApplicationInvitationRepository>(), _container.Resolve<IVendorRepository>(),
                    _container.Resolve<IInventoryAppEventFactory>(), _container.Resolve<IRestaurantRepository>(),
                    _container.Resolve<IPARRepository>(), _container.Resolve<IInventoryRepository>(),
                    _container.Resolve<IReceivingOrderRepository>(), _container.Resolve<IPurchaseOrderRepository>());
                await loader.LoadRepositories(RestaurantID);
            }
            catch (Exception e)
            {
                var logger = _container.Resolve<ILogger>();
                if (logger != null)
                {
                    logger.HandleException(e);
                }
                throw;
            }
        }
    }
}