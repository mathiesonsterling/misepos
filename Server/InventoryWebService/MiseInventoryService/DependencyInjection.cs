using System;
using System.Collections.Generic;
using Funq;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Repositories;
using Mise.Core.Server;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.InventoryWebService.ServiceInterface.Services;
using Mise.Neo4J.Neo4JDAL;
using MiseInventoryService.Repositories;

namespace MiseInventoryService
{
    /// <summary>
    /// All dependency injection we need for the inventory service gets done here
    /// </summary>
    public static class DependencyInjection
    {
        public static void Configure(Container container)
        {
            //config - do this to get the connection string to our db
            container.RegisterAutoWiredAs<Config, IConfig>();

            //logging
            var logger = new ConsoleLogger();
            container.Register<ILogger>(logger);
            //container.RegisterAutoWiredAs<ConsoleLogger, ILogger>();

            //serializer
            container.RegisterAs<JsonNetSerializer, IJSONSerializer>();

            //DALs
            container.RegisterAutoWiredAs<Neo4JEntityDAL, IEntityDAL>();
            //container.RegisterAutoWiredAs<FakeInventoryServiceDAL, IEntityDAL>();
            container.RegisterAutoWiredAs<FakeEventStore, IEventStorageDAL>();

            //dto factory
            container.RegisterAutoWiredAs<EntityDataTransportObjectFactory, IEntityDataTransportObjectFactory>();

            container.RegisterAutoWiredAs<IISHostingEnvironment, IWebHostingEnvironment>();

            //repository - we will need to have this load on creation?
            container.RegisterAutoWiredAs<VendorServerRepository, IVendorRepository>();
            container.RegisterAutoWiredAs<InventoryServerRepository, IInventoryRepository>();
            container.RegisterAutoWiredAs<PurchaseOrderServerRepository, IPurchaseOrderRepository>();
            container.RegisterAutoWiredAs<EmployeeServerRepository, IEmployeeRepository>();
            container.RegisterAutoWiredAs<ReceivingOrderRepository, IReceivingOrderRepository>();
            container.RegisterAutoWiredAs<PARServerRepository, IPARRepository>();
            container.RegisterAutoWiredAs<RestaurantRepository, IRestaurantRepository>();
            container.RegisterAutoWiredAs<ApplicationInvitationServerRepository, IApplicationInvitationRepository>();
            container.RegisterAutoWiredAs<AccountServerRestaurantRepository, IAccountRepository>();

            //load repositories
            LoadRepositories(container, logger);
        }

        public static async void LoadRepositories(Container container, ILogger logger)
        {
            var repositories = new List<IRepository>
            {
                container.Resolve<IRestaurantRepository>(),
                container.Resolve<IEmployeeRepository>(),
                container.Resolve<IVendorRepository>(),
                container.Resolve<IInventoryRepository>(),
                container.Resolve<IPurchaseOrderRepository>(),
                container.Resolve<IReceivingOrderRepository>(),
                container.Resolve<IPARRepository>(),
                container.Resolve<IApplicationInvitationRepository>(),
                container.Resolve<IAccountRepository>()
            };

            foreach (var repos in repositories)
            {
                try
                {
                    await repos.Load(null).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                   logger.HandleException(e);
                    throw;
                }
            }
        }
    }
}
