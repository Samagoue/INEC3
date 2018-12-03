using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using INEC3.Helper;

namespace INEC3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        _Helper _helper = new _Helper();
        string connString = ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Start SqlDependency with application initialization
            //SqlDependency.Start(connString);
            //DefaultUser InitialLize
            _helper.InitializeUser();
        }
        protected void Application_End()
        {
            //SqlDependency.Stop(ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString);
        }

    }

}


