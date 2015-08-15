using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Mindscape.Raygun4Net.WebApi;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Common.Services.WebServices.FakeServices;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Services.UtilityServices;
using stockboymobileserviceService.DataObjects;
using stockboymobileserviceService.Models;

namespace stockboymobileserviceService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
            RaygunWebApiClient.Attach(config);

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            // Set default and null value handling to "Include" for Json Serializer
            config.Formatters.JsonFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            
            Database.SetInitializer(new stockboymobileserviceInitializer());
        }
    }

    public class stockboymobileserviceInitializer : ClearDatabaseSchemaIfModelChanges<stockboymobileserviceContext>
    {
        protected override void Seed(stockboymobileserviceContext context)
        {
            //set our items here!
            //await LoadSeedFromFakeWebservice(context);

            base.Seed(context);
        }
    }
}

