using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using INEC3;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using INEC3.App_Start;
using INEC3.Providers;

[assembly: OwinStartup(typeof(INEC3.Startup))]
namespace INEC3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureAuth(app);

            WebApiConfig.Register(config);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            app.MapSignalR();
        }

        

        //private void ConfigureOAuth(IAppBuilder app)
        //{
        //    OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
        //    {
        //        AllowInsecureHttp = true,
        //        TokenEndpointPath = new PathString("/token"),
        //        AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
        //        Provider = new AuthorizationServerProvider()
        //    };

        //    // Token Generation
        //    app.UseOAuthAuthorizationServer(OAuthServerOptions);
        //    app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        //}
    }
}