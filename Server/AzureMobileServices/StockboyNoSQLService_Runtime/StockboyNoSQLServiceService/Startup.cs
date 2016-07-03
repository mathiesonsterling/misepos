using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StockboyNoSQLServiceService.Startup))]

namespace StockboyNoSQLServiceService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}