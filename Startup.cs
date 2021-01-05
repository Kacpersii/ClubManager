using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClubManager.Startup))]
namespace ClubManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
