﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Mise.Database.AzureDefinitions;
using Mise.Database.AzureDefinitions.Entities.Restaurant;
using Mise.Database.AzureDefinitions.ValueItems;
using StockboyMobileAppServiceService.Models;
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
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (var todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            try
            {
                var restId = Guid.NewGuid();
                var restaurant = new Restaurant
                {
                    RestaurantID = restId,
                    EntityId = restId,
                    Id = restId.ToString(),
                    EmailsToSendReportsTo = new List<EmailAddress>
                    {
                        new EmailAddress {Value = "mathieson@misepos.com"}
                    },
                    Name = new BusinessName {FullName = "Matty's Test", ShortName = "Test"},
                };
                context.Set<Restaurant>().Add(restaurant);

                //here just to fire an ex
                context.SaveChanges();
                base.Seed(context);
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }
    }
}

