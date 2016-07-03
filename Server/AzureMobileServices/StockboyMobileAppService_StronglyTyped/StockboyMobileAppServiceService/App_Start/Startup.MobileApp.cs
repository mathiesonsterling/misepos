using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Owin;

namespace StockboyMobileAppServiceService
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new StockboyMobileAppServiceInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<StockboyMobileAppServiceContext>(null);

            var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseWebApi(config);
        }
    }

    public class StockboyMobileAppServiceInitializer : CreateDatabaseIfNotExists<StockboyMobileAppServiceContext>
    {
        protected override void Seed(StockboyMobileAppServiceContext context)
        {
            base.Seed(context);
            AddMiseApplications(context);
            AddShapes(context);
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }

            AddInventoryCategories(context);
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                var msg = e.Message;
                throw;
            }
        }

        private static void AddShapes(StockboyMobileAppServiceContext context)
        {
            var shapes = Mise.Core.ValueItems.Inventory.LiquidContainer.GetStandardBarSizes();

            foreach (var entShape in shapes.Select(shape => new LiquidContainer(shape)))
            {
                context.LiquidContainers.Add(entShape);
            }
        }


        private static void AddInventoryCategories(StockboyMobileAppServiceContext context)
        {
            var catService = new CategoriesService();
            var allCats = catService.GetAllCategories();

            var newAndOld = new List<Tuple<IInventoryCategory, InventoryCategory>>();
            foreach (var cat in allCats)
            {
                var prefContainer = cat.GetPreferredContainers().FirstOrDefault();
                LiquidContainer container = null;
                if (prefContainer != null)
                {
                    container =
                        context.LiquidContainers.FirstOrDefault(c => c.DisplayName != null && c.DisplayName == prefContainer.DisplayName);
                }

                var tup = new Tuple<IInventoryCategory, InventoryCategory>(cat, new InventoryCategory(cat, container));
                newAndOld.Add(tup);
            }

            //first create all, then assign their parents!
            foreach (var pair in newAndOld)
            {
                var oldParent = pair.Item1.ParentCategoryID;
                if (oldParent.HasValue)
                {
                    var parentPair = newAndOld.FirstOrDefault(p => p.Item2.EntityId == oldParent.Value);
                    if (parentPair != null)
                    {
                        pair.Item2.ParentCategory = parentPair.Item2;
                    }
                }
            }

            var newOnes = newAndOld.Select(p => p.Item2);

            foreach (var c in newOnes)
            {
                context.InventoryCategories.Add(c);
            }
        }

        private static void AddMiseApplications(StockboyMobileAppServiceContext context)
        {
            var allApps = Enum.GetValues(typeof(MiseAppTypes)).Cast<MiseAppTypes>();
            var ents = allApps.Select(a => new MiseApplication(a));

            foreach (var app in ents)
            {
                context.MiseApplications.Add(app);
            }
        }
    }
}

