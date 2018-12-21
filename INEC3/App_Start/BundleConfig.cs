using System.Web;
using System.Web.Optimization;

namespace INEC3
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js"));
                        //"~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jqud3mapHome").Include(
                       "~/Scripts/d3/d3.v4.min.js",
                       "~/Scripts/d3/topojson-client.js"));
            bundles.Add(new ScriptBundle("~/bundles/HomeCustomjs").Include(
                       "~/Scripts/Custome/DashBoard.min.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
        }
    }
}
