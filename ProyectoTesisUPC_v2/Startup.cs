using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProyectoTesisUPC_v2.Startup))]
namespace ProyectoTesisUPC_v2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
