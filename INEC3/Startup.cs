using System;
using Microsoft.Owin;
using Owin;
using INEC3;

[assembly: OwinStartup(typeof(Startup))]
namespace INEC3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}