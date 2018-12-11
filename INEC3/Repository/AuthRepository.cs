using System;
using INEC3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using INEC3.IdentityClass;
using INEC3.Models.Service;
using System.Linq;

namespace INEC3.Repository
{
    public class AuthRepository : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AccountService _accountService;
        private readonly inecDBContext _inecDBContext;

        public AuthRepository()
        {
            _context = new ApplicationDbContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context));
            _accountService = new AccountService();
            _inecDBContext = new inecDBContext();

        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            var roleresult = _userManager.AddToRole(user.Id, UserManageRoles.PollingUser);

            if (result.Succeeded)
            {
                if (userModel.UserProfile == null)
                {
                    UserProfile userProfile = new UserProfile();
                    userProfile.AspNetUsersId = Guid.Parse(user.Id);
                    IdentityResult res = _accountService.RegisterUserProfile(userProfile);
                    if (!res.Succeeded)
                        return res;
                }
                else
                {
                    userModel.UserProfile.AspNetUsersId = Guid.Parse(user.Id);
                    IdentityResult res = _accountService.RegisterUserProfile(userModel.UserProfile);
                    if (!res.Succeeded)
                        return res;
                }
            }
            return result;
        }


        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public IdentityUser FindUserDetailByEmail(string email)
        {
            IdentityUser user = _userManager.FindByEmail(email);
            return user;
        }

        public IdentityUser FindUserDetailByUserName(string username)
        {
            IdentityUser user = _userManager.FindByName(username);
            return user;
        }

        public bool IsInRole(string UserName, string role)
        {
            IdentityUser user = FindUserDetailByUserName(UserName);
            if (user != null)
            {
                return _userManager.IsInRole(user.Id, role);
            }
            return false;
        }

        public string GeneratePasswordResetToken(string email)
        {
            string url = null;
            IdentityUser user = FindUserDetailByEmail(email);
            if (user != null)
            {
                //var provider = new DpapiDataProtectionProvider("SampleAppName");
                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>());
                //userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("ASP.NET Identity"));
                //string code=_userManager.GeneratePasswordResetToken(user.Id);
                url = "&userId=" + user.Id + "&code=" + user.SecurityStamp;
            }
            return url;
        }

        public IdentityResult CreateDefaultUser(IdentityUser user)
        {
            var result = _userManager.Create(user, user.PasswordHash);
            return result;
        }
        public IdentityResult AddUserRole(string userid, string role)
        {
            var result = _userManager.AddToRole(userid, "SuperAdmin");
            return result;
        }
        public IdentityResult ChangeUserRole(string userid, string updatedrole)
        {
            IdentityResult res = new IdentityResult();
            var oldrole = _userManager.GetRoles(userid).FirstOrDefault();
            if (!string.Equals(oldrole,updatedrole))
            {
                res = _userManager.RemoveFromRole(userid, oldrole);
                if (res.Succeeded)
                {
                    res = _userManager.AddToRole(userid, updatedrole);
                }
                return res;
            }
            return res;
        }
        public string GetUserRole(string userid)
        {
            return _userManager.GetRoles(userid).FirstOrDefault();
        }

    public void GetRolesList()
    {
        var result = _context.Roles.GetEnumerator();
        var c = result;
    }

    public void Dispose()
    {
        _context.Dispose();
        _userManager.Dispose();
        _inecDBContext.Dispose();
    }
}
}