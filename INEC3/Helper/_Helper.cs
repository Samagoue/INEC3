using System;
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
        private ApplicationDbContext _context;
        private AuthRepository _authRepository;
        private AccountService _accountService;
        private inecDBContext _inecDBContext;
        //private UserManager<IdentityUser> _userManager;
        private AdminService _adminservice;
        private Sqldbconn _db;
        public SqlCommandText _smodel = new SqlCommandText();
        public _Helper()
        {
            _context = new ApplicationDbContext();
            _accountService = new AccountService();
            _inecDBContext = new inecDBContext();
            _authRepository = new AuthRepository();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));
            _adminservice = new AdminService();
            _db = new Sqldbconn();
        }
        //private Sqldbconn _db = new Sqldbconn();
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
                _authRepository.GetRolesList();
                //Step 1 Create and add the new Role.
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                if (!roleManager.RoleExists(UserManageRoles.SuperAdmin))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.SuperAdmin);
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists(UserManageRoles.User))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.User);
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists(UserManageRoles.ProvinceUser))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.ProvinceUser);
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists(UserManageRoles.TerritoireUser))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.TerritoireUser);
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists(UserManageRoles.CommuneUser))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.CommuneUser);
                    _context.Roles.Add(roleToChoose);
                    _context.SaveChanges();
                }
                if (!roleManager.RoleExists(UserManageRoles.PollingUser))
                {
                    var roleToChoose = new IdentityRole(UserManageRoles.PollingUser);
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
               // var result = _userManager.Create(user, user.PasswordHash);
                var result = _authRepository.CreateDefaultUser(user);
                if (result.Succeeded)
                {
                    //var roleresult = _userManager.AddToRole(user.Id, "SuperAdmin");
                    var roleresult = _authRepository.AddUserRole(user.Id, "SuperAdmin");
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