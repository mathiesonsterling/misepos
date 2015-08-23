using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiseReporting.Startup))]
namespace MiseReporting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
