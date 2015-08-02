using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiseAdminWebsite.Startup))]
namespace MiseAdminWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
