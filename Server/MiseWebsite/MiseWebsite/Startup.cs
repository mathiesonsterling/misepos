using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiseWebsite.Startup))]
namespace MiseWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
