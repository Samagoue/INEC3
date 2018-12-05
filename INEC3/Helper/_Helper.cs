using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Data;
using Newtonsoft.Json;
using INEC3.DbConn;
using INEC3.IdentityClass;
using Microsoft.AspNet.Identity;
using INEC3.Models.Service;
using INEC3.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using INEC3.Repository;

namespace INEC3.Helper
{
    public class _Helper
    {
        private AuthContext _context;
        private AuthRepository _authRepository;
        private AccountService _accountService;
        private inecDBContext _inecDBContext;
        private UserManager<IdentityUser> _userManager;
        private AdminService _adminservice;
        public _Helper()
        {
            _context = new AuthContext();
            _accountService = new AccountService();
            _inecDBContext = new inecDBContext();
            _authRepository = new AuthRepository();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));
            _adminservice = new AdminService();
        }
        private Sqldbconn _db = new Sqldbconn();
        public bool SendNotification()
        {
            try
            {
                DataSet dt = new DataSet();
                dt = _db.GetDatatable("proc_GetProvinceResult", "");
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalR.RealTimeMapHub>();
                hubContext.Clients.All.mapUpdate(JsonConvert.SerializeObject(dt));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ActiveSqlNotification()
        {
            try
            {
                SqlNotification objRepo = new SqlNotification();
                objRepo.GetAllMessages();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GetUserDetail()
        {
            ////var userId = User.Identity.GetUserId();
            //var username = HttpContext.Current.User.Identity.GetUserName();
            //var name = HttpContext.Current.User.Identity.Name;
            //var userId = HttpContext.Current.User.Identity.GetUserId();
            //var ab = HttpContext.Current.GetOwinContext().GetUserManager<AuthContext>();
            //var ad = RequestContext.Principal.Identity.GetUserId();

            //var claimsIdentity = User.Identity as ClaimsIdentity;
            //if (claimsIdentity != null)
            //{
            //    string email = claimsIdentity?.FindFirst(c => c.Type == "sub")?.Value;
            //}
        }

        public bool InitializeUser()
        {
            try
            {
                //Step 1 Create and add the new Role.
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new AuthContext()));
                if (!roleManager.RoleExists("User"))
                {
                    var roleToChoose = new IdentityRole("User");
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists("SuperAdmin"))
                {
                    var roleToChoose = new IdentityRole("SuperAdmin");
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }

                IdentityUser user = new IdentityUser
                {
                    UserName = "Admin@shadow.com",
                    Email = "Admin@shadow.com",
                    PasswordHash = "Admin@1234",
                    EmailConfirmed = true
                };
                var result = _userManager.Create(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    var roleresult = _userManager.AddToRole(user.Id, "SuperAdmin");
                    UserProfile userProfile = new UserProfile();
                    userProfile.FirstName = "Admin";
                    userProfile.LastName = "Admin";
                    userProfile.AspNetUsersId = Guid.Parse(user.Id);
                    IdentityResult res = _accountService.RegisterUserProfile(userProfile);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        

    }
}