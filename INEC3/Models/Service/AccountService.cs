using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using INEC3.Helper;
using INEC3.IdentityClass;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace INEC3.Models.Service
{
    public class AccountService
    {
        private inecDBContext db = new inecDBContext();
        private AuthContext _context;
        private UserManager<IdentityUser> _userManager;

        public AccountService()
        {
            _context = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));

        }

        public bool SendEmailVerification(string email, string securitycode)
        {
            try
            {
                string msg = Email.GetTemplateString((int)Email.EmailTemplates.WelcomeEmail);
                msg = msg.Replace("{Name}", email);
                string SiteLink = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Account/Vefiryemail?securitycode=" + securitycode;

                msg = msg.Replace("{site_address}", SiteLink);
                if (Email.SendMail(email, "Welcome to Shadow", msg))
                {
                    return true;
                }
                return false;
            }
            catch (Exception exm)
            {
                return false;
            }
        }

        public bool activateaccount(string securitycode)
        {
            IdentityUser user = _userManager.FindById(securitycode);
            if (user != null)
            {
                user.EmailConfirmed = true;
                _userManager.Update(user);
                return true;
            }
            return false;
        }

        public IdentityResult RegisterUserProfile(UserProfile userProfile)
        {
            try
            {
                userProfile.Isactive = "true";
                db.UserProfile.Add(userProfile);
                db.SaveChanges();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message.ToString());
            }
        }
    }
}