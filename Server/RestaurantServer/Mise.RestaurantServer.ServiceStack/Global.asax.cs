using System;
using System.Configuration;
using System.Web;
using Funq;
//using Mise.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.Implementation;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
//using Mise.Database.SQLite;
using Mise.RestaurantServer.ServiceStack.Services;
using ServiceStack;
using Mise.Core.Common.Services.DAL;

namespace Mise.RestaurantServer.ServiceStack
{
    public class Global : HttpApplication
    {

        public class RestaurantServerAppHost : AppHostBase
        {
            public static IRestaurantServerDAL RestaurantServerDAL { get; private set; }
            public static ILogger Logger { get; private set; }
            public static Guid? RestaurantID { get; private set; }

            public static IMiseAdminServer MiseAdminServer { get; private set; }

            public RestaurantServerAppHost() : base("RestaurantServer", typeof(HelloService).Assembly)
            {
            }

            public override void Configure(Container container)
            {
                Plugins.Add(new RequestLogsFeature
                {
                    EnableSessionTracking = false,
                    RequiredRoles = null
                });
                var onAzure = false;
                if (ConfigurationManager.AppSettings["RunningOnAzure"] != null)
                {
                    onAzure = bool.Parse(ConfigurationManager.AppSettings["RunningOnAzure"]);
                }
                RestaurantID = null;
                RestaurantServerDAL = null;
                MiseAdminServer = null;
                var serizlier = new JsonNetSerializer();
                Logger = new SystemTraceLogger();
                if (onAzure)
                {
                    var connString = ConfigurationManager.AppSettings["StorageConnectionString"];
                    //RestaurantServerDAL = new AzureStorageDAL(serizlier, connString, Logger);
                    MiseAdminServer = new FakeMiseAdminServer();
                }
                else
                {
                    //construct our path depending on our OS
                    //var dal = new SQLiteRestaurantServerDAL(Logger, serizlier, "c:\\db\\restaurantServerDB.db");
                    //dal.Init();
                    //RestaurantServerDAL = dal;
                    MiseAdminServer = new FakeMiseAdminServer();
                }

                Logger.Log("Logger intialized in Bootstrapper", LogLevel.Debug);
                var serial = new JsonNetSerializer();
                container.Register<IJSONSerializer>(serial);
                container.Register(RestaurantServerDAL);
                container.Register(Logger);

                var menuRepos = new MenuRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
                Logger.Log("Loading Menu Repository", LogLevel.Debug);
                menuRepos.Load();
                Logger.Log("Menu Repository loaded", LogLevel.Debug);
                container.Register<IMenuRepository>(menuRepos);

                var restaurantRepos = new RestaurantRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
                Logger.Log("Loading Restaurant Repository", LogLevel.Debug);
                restaurantRepos.Load(RestaurantID);
                Logger.Log("Restaurant Repository loaded", LogLevel.Debug);
                container.Register<IRestaurantRepository>(restaurantRepos);

                var checkRepos = new CheckRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
                Logger.Log("Loading Check Repository", LogLevel.Debug);
                checkRepos.Load(RestaurantID);
                Logger.Log("Check Repository loaded", LogLevel.Debug);
                container.Register<ICheckRepository>(checkRepos);

                var employeeRepos = new EmployeeRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
                Logger.Log("Loading Employee Repository", LogLevel.Debug);
                employeeRepos.Load(RestaurantID);
                Logger.Log("Employee Repository loaded", LogLevel.Debug);
               // container.Register(employeeRepos);
                container.Register<IEmployeeRepository>(employeeRepos);

                var menuRulesRepos = new MenuRulesRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
                menuRulesRepos.Load(RestaurantID);
                Logger.Log("MenuRules repository loaded", LogLevel.Debug);
                //container.Register(menuRulesRepos);

                container.Register<IMenuRulesRepository>(menuRulesRepos);
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            var appHost = new RestaurantServerAppHost();
            appHost.Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}