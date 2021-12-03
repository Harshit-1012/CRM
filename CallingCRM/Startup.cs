using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CallingCRM.Startup))]
namespace CallingCRM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
