using System;
using System.Web;
using Funq;
using Mise.Core.Services;
using Mise.InventoryWebService.ServiceInterface;
using ServiceStack;

namespace MiseInventoryService
{ 
    public class AppHost : AppHostBase
    {
        public AppHost() : base("Inventory Services", typeof(InventoryService).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            DependencyInjection.Configure(container);

            //add a handler for our logging of exceptions
            
            var logger = container.Resolve<ILogger>();
            if (logger != null)
            {
                UncaughtExceptionHandlers.Add((req, res, operationName, ex) =>
                {
                    logger.Error(string.Format("Error while processing ServiceStack request, OperationName:{0}", operationName));
                    logger.HandleException(ex);

                    res.StatusCode = 400;
                    throw ex;
                });
            }
        }
    }

    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var host = new AppHost();
            host.Init();
        }
    }
}