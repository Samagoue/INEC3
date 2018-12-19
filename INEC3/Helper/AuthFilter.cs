using System.Web.Mvc;
using System.Web.Routing;

namespace INEC3.Helper
{
    public class AuthFilter : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //Check Session is Empty Then set as Result is HttpUnauthorizedResult 
            //if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Request.Cookies["inecbearer"])))
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
                //filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Account", action = "Login" }));
            }
        }

         
        }
    }
