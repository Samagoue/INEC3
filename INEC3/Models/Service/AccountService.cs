using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using INEC3.Helper;
using Microsoft.AspNet.Identity;
using INEC3.Repository;
using Microsoft.AspNet.Identity.EntityFramework;

namespace INEC3.Models.Service
{
    public class AccountService : IDisposable
    {
        private inecDBContext db;//= new inecDBContext();
        string constring = ConfigurationManager.ConnectionStrings["inecConn"].ToString();
        public SqlCommandText _smodel = new SqlCommandText();
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountService()
        {
            db = new inecDBContext();
            _context = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
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
            ApplicationUser user = _userManager.FindById(securitycode);
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
                db.UserProfiles.Add(userProfile);
                db.SaveChanges();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(ex.Message.ToString());
            }
        }

        public List<UserDisplay> GetUserList()
        {
            List<UserDisplay> user = new List<UserDisplay>();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand(_smodel.vw_UserProfile, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserDisplay u = new UserDisplay();
                        u.UserId = Convert.ToString(rdr["Id"]);
                        u.Email = Convert.ToString(rdr["Email"]);
                        u.UserName = Convert.ToString(rdr["UserName"]);
                        u.Name = Convert.ToString(rdr["FirstName"]);
                        u.Role = Convert.ToString(rdr["RoleName"]);
                        u.EmailConfirmed = Convert.ToBoolean(rdr["EmailConfirmed"]);
                        u.Isactive = Convert.ToBoolean(rdr["Isactive"]);
                        user.Add(u);
                    }
                    con.Close();

                }

            }
            return user;
        }
        public UserDisplay FindUserDisplay(string key, string value)
        {
            UserDisplay user = new UserDisplay();
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand(_smodel.vw_UserProfileWhere + " " + key + "='" + value + "'", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        user.UserId = Convert.ToString(rdr["Id"]);
                        user.UserName = Convert.ToString(rdr["UserName"]);
                        user.Email = Convert.ToString(rdr["Email"]);
                        user.Name = Convert.ToString(rdr["FirstName"]);
                        user.Role = Convert.ToString(rdr["RoleName"]);
                        user.EmailConfirmed = Convert.ToBoolean(rdr["EmailConfirmed"]);
                        user.Isactive = Convert.ToBoolean(rdr["Isactive"]);
                    }
                    con.Close();

                }

            }
            return user;
        }

        public bool RestPasswordsendmail(string resttoken, string email)
        {
            try
            {

                string msg = Email.GetTemplateString((int)Email.EmailTemplates.ForgotPassword);
                msg = msg.Replace("{Name}", email);
                string SiteLink = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Account/ForgotPassword?action=resetpassword" + resttoken;

                msg = msg.Replace("{ResetLink}", SiteLink);
                if (Email.SendMail(email, "Shadow Email Verify", msg))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            db.Dispose();
            _context.Dispose();
            _userManager.Dispose();
        }
    }

}