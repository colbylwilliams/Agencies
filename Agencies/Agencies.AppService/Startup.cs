using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Agencies.AppService.Startup))]

namespace Agencies.AppService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}