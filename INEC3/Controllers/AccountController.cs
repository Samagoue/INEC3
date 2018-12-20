using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using INEC3.Models.Service;
using INEC3.Helper;
using INEC3.Models;
namespace INEC3.Controllers
{
    public class AccountController : Controller
    {
        Base _base = new Base();
        AccountService accountService = new AccountService();

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            _base.access_token = "";
            Request.GetOwinContext().Authentication.SignOut();
            return (RedirectToAction("Login", "Account"));
        }

        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Vefiryemail(string securitycode)
        {
            if (accountService.activateaccount(securitycode))
            {
                TempData["success"] = "Email verification success. Login with credentials";
                return RedirectToAction("Login");
            }
            return RedirectToAction("Index", "error", new { id = "emailfail" });
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string userId, string code)
        {
            ForgotPasswordModel model = new ForgotPasswordModel();
            model.UserId = Guid.Parse(userId);
            model.SecurityCode = code;
            return View(model);
        }

        public void Index(string userid, string access_token, string token_type)
        {
            FormsAuthentication.SetAuthCookie(userid, false);
            _base.access_token = token_type + " " + access_token;
        }
    }
}