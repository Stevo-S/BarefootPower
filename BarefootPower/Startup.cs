using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BarefootPower.Startup))]
namespace BarefootPower
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
