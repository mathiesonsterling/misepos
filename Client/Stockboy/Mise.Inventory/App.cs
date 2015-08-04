using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services.Implementation;
using Mise.Inventory.ViewModels.Reports;
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

		private IInsightsService _insights;
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
            _insights = Resolve<IInsightsService>();
            if (_insights == null)
            {
                throw new Exception("Cannot resolve insights service");
            }
				

            MainPage = new NavigationPage(initialPage);

            navigationService.Navi = MainPage.Navigation;
            navigationService.CurrentPage = initialPage;

            LoadRepositoriesVoid();

            //if we've got a stored login, load it up
			AttemptToLoginSavedEmployee (appNavigation);
            
        }

        protected override async void OnSleep()
        {
            //if we have any events still trying to send, give them another try
            try
            {
				if(_insights != null){
					_insights.Track("Stockboy put to sleep", new Dictionary<string, string>());
				}
                var dal = Resolve<IClientDAL>();
                var httpClient = Resolve<IResendEventsWebService>();

                var resends = (await dal.GetUnsentEvents()).Select(dto => dto as IEntityEventBase).ToList();
                if (resends.Any())
                {
                    await httpClient.ResendEvents(resends);
                }
            }
            catch (Exception e)
            {
                try
                {
                    var logger = Resolve<ILogger>();
                    logger.HandleException(e);
                }
                catch (Exception)
                {
                    //nuke it here
                }
            }

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

        public static RestaurantSelectViewModel RestaurantSelectViewModel { get { return Resolve<RestaurantSelectViewModel>(); } }

        public static SectionAddViewModel SectionAddViewModel { get { return Resolve<SectionAddViewModel>(); } }

        public static SectionSelectViewModel SectionSelectViewModel { get { return Resolve<SectionSelectViewModel>(); } }

        public static VendorAddViewModel VendorAddViewModel { get { return Resolve<VendorAddViewModel>(); } }

        public static VendorFindViewModel VendorFindViewModel { get { return Resolve<VendorFindViewModel>(); } }

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

		public static ReportsViewModel ReportsViewModel { get { return Resolve<ReportsViewModel>(); } }
		public static SelectCompletedInventoryViewModel SelectCompletedInventoryViewModel{ get { return Resolve<SelectCompletedInventoryViewModel> (); } }
		public static ReportResultsViewModel ReportResultsViewModel{ get { return Resolve<ReportResultsViewModel> (); } }

        public static AuthorizeCreditCardViewModel AuthorizeCreditCardViewModel { get
        {
            return Resolve<AuthorizeCreditCardViewModel>();
        } }
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

		private async void AttemptToLoginSavedEmployee(IAppNavigation appNavigation)
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
                var loader = new RepositoryLoader(_container.Resolve<IEmployeeRepository>(),
                    _container.Resolve<IApplicationInvitationRepository>(), _container.Resolve<IVendorRepository>(),
                    _container.Resolve<IInventoryAppEventFactory>(), _container.Resolve<IRestaurantRepository>(),
                    _container.Resolve<IParRepository>(), _container.Resolve<IInventoryRepository>(),
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