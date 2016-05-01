using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StockboyMobileAppServiceService.Startup))]

namespace StockboyMobileAppServiceService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}