using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace INEC3.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {

            if (username == "admin" && password == "123456")
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Session["UserID"] = username;
                return (RedirectToAction("Index", "Admin"));
                //return View();

            }

            ViewBag.Message = "Invalid Username or Password !";
            return View();
        }

        public ActionResult Logoff()
        {
            Session["UserID"] = null;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return (RedirectToAction("Index", "Home"));
        }

    }
}