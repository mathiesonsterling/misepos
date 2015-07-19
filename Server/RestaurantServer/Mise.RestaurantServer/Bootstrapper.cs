using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Mise.Azure;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Server.Repositories;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Server.Services.Implementation;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.RestaurantServer.Services;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Common;

namespace Mise.RestaurantServer
{
  
  public static class Bootstrapper
  {
    public static IUnityContainer Initialise()
    {
      var container = BuildUnityContainer();

      GlobalConfiguration.Configuration.DependencyResolver = new Microsoft.Practices.Unity.WebApi.UnityDependencyResolver(container);
      return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
      var container = new UnityContainer();
      // register all your components with the container here
      RegisterTypes(container);

      return container;
    }
	public static IRestaurantServerDAL RestaurantServerDAL { get; private set; }
    public static ILogger Logger { get; private set; }
    public static Guid? RestaurantID { get; private set; }

    public static IMiseAdminServer MiseAdminServer { get; private set; }

    public static void RegisterTypes(IUnityContainer container)
    {
        var onAzure = false;
        if (ConfigurationManager.AppSettings["RunningOnAzure"] != null)
        {
            onAzure = bool.Parse(ConfigurationManager.AppSettings["RunningOnAzure"]);
        }
        RestaurantID = null;
        RestaurantServerDAL  = null;
        Logger = null;
        MiseAdminServer  = null;
        var serizlier = new JsonNetSerializer();
        Logger = new SystemTraceLogger();
        if (onAzure)
        {
            var connString = ConfigurationManager.AppSettings["StorageConnectionString"];
		    RestaurantServerDAL = new MemoryRestaurantServerDAL ();
            //RestaurantServerDAL = new AzureStorageDAL(serizlier, connString, Logger);
            MiseAdminServer = new FakeMiseAdminServer();
        } else 
        {
            var restID = Guid.Parse(ConfigurationManager.AppSettings["RestaurantID"]);
            RestaurantID = restID;
            //TODO need a persistant one here that can run on Mono/Raspberry Pi!
            RestaurantServerDAL = new MemoryRestaurantServerDAL();
            MiseAdminServer = new FakeMiseAdminServer();
        }

        Logger.Log("Logger intialized in Bootstrapper", LogLevel.Debug);
        var serial = new JsonNetSerializer();
        container.RegisterInstance(typeof (IRestaurantServerDAL), RestaurantServerDAL);
        container.RegisterInstance(typeof(ILogger), Logger);
        container.RegisterInstance(typeof (IJSONSerializer), serial);

        var menuRepos = new MenuRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
        Logger.Log("Loading Menu Repository", LogLevel.Debug);
        menuRepos.Load();
        Logger.Log("Menu Repository loaded", LogLevel.Debug);
        container.RegisterInstance(typeof(IMenuRepository), menuRepos);

        var restaurantRepos = new RestaurantRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
        Logger.Log("Loading Restaurant Repository", LogLevel.Debug);
        restaurantRepos.Load(RestaurantID);
        Logger.Log("Restaurant Repository loaded", LogLevel.Debug);
        container.RegisterInstance(typeof (IRestaurantRepository), restaurantRepos);

        var checkRepos = new CheckRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
        Logger.Log("Loading Check Repository", LogLevel.Debug);
        checkRepos.Load(RestaurantID);
        Logger.Log("Check Repository loaded", LogLevel.Debug);
        container.RegisterInstance(typeof (ICheckRepository), checkRepos);

        var employeeRepos = new EmployeeRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
        Logger.Log("Loading Employee Repository", LogLevel.Debug);
        employeeRepos.Load(RestaurantID);
        Logger.Log("Employee Repository loaded", LogLevel.Debug);
        container.RegisterInstance(typeof (IEmployeeRepository), employeeRepos);

        var menuRulesRepos = new MenuRulesRestaurantServerRepository(MiseAdminServer, RestaurantServerDAL, Logger, RestaurantID);
        menuRulesRepos.Load(RestaurantID);
        Logger.Log("MenuRules repository loaded", LogLevel.Debug);
        container.RegisterInstance(typeof (IMenuRulesRepository), menuRulesRepos);
    }
  }
}