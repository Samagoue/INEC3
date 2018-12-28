using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using INEC3.Helper;
using INEC3.Controllers;
using System.Web.Hosting;

namespace INEC3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        _Helper _helper = new _Helper();
        private Timer aTimer;

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
            HttpContext.Current.Application["SqlVersion"] = 0;
            // Create a timer with a two second interval.
            aTimer = new Timer(5000);
            // Hook up the Elapsed event for the timer.
            aTimer.Elapsed += CheckSqlLogNotification;
            aTimer.Enabled = true;
        }
        protected void Application_End()
        {
            //SqlDependency.Stop(ConfigurationManager.ConnectionStrings["inecConn"].ConnectionString);
        }
        public async void CheckSqlLogNotification(Object source, ElapsedEventArgs e)
        {
            if (Application["SqlVersion"] != null)
            {
                int ver = Convert.ToInt32(Application["SqlVersion"]);
                int newver = await _helper.CheckAndSendSqlChange(ver);
                if (ver != newver)
                    Application["SqlVersion"] = newver;
            }
        }
    }

}


