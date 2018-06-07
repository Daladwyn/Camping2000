using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Camping2000.Startup))]
namespace Camping2000
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
