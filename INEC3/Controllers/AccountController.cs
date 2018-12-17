using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using INEC3.Models.Service;
using INEC3.Helper;
namespace INEC3.Controllers
{
    public class AccountController : Controller
    {
        Base _base= new Base();
        AccountService accountService = new AccountService();

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
            if (Request.Cookies["inecbearer"] != null)
            {
                HttpCookie myCookie = new HttpCookie("inecbearer");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);

                HttpCookie myCookie1 = new HttpCookie("inceusername");
                myCookie1.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie1);
                

            }
            return (RedirectToAction("Index", "Home"));
        }
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Vefiryemail(string securitycode)
        {
            if (accountService.activateaccount(securitycode))
            {
                TempData["success"] = "Email verification success. Login with credentials";
                return RedirectToAction("Login");
            }
            return RedirectToAction("Index","error",new { id = "emailfail",ret="returnurl='abc'" });
        }

        public ActionResult ForgotPassword(string userId, string code)
        {

            return View();
        }
    }
}