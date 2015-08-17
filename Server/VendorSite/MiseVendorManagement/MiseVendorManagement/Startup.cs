using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiseVendorManagement.Startup))]
namespace MiseVendorManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
