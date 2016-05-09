﻿using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Mise.Core.Entities;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities;
using Owin;

namespace StockboyMobileAppServiceService
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new StockboyMobileAppServiceInitializer());

            // To prevent Entity Framework from modifying your database schema, use a null database initializer
            // Database.SetInitializer<StockboyMobileAppServiceContext>(null);

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

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
          var allApps = Enum.GetValues(typeof(MiseAppTypes)).Cast<MiseAppTypes>();
          var ents = allApps.Select(a => new MiseApplication(a));

        foreach (var app in ents)
        {
          context.MiseApplications.Add(app);
        }

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
    }
}

