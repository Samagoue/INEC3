using System;
using INEC3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using INEC3.Models.Service;
using System.Linq;

namespace INEC3.Repository
{
    public class AuthRepository : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AccountService _accountService;
        private readonly inecDBContext _inecDBContext;

        public AuthRepository()
        {
            _context = new ApplicationDbContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            _accountService = new AccountService();
            _inecDBContext = new inecDBContext();

        }

        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            var roleresult = _userManager.AddToRole(user.Id, UserManageRoles.User);

            if (result.Succeeded)
            {
                //if (userModel.UserProfile == null)
                //{
                //    UserProfile userProfile = new UserProfile();
                //    userProfile.AspNetUsersId = Guid.Parse(user.Id);
                //    IdentityResult res = _accountService.RegisterUserProfile(userProfile);
                //    if (!res.Succeeded)
                //        return res;
                //}
                //else
                //{
                UserProfile upf = new UserProfile();
                upf.FirstName = userModel.FirstName;
                upf.LastName = userModel.LastName;
                upf.AspNetUsersId = Guid.Parse(user.Id);
                IdentityResult res = _accountService.RegisterUserProfile(upf);
                if (!res.Succeeded)
                    return res;
                //}
            }
            return result;
        }


        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public ApplicationUser FindUserDetailByEmail(string email)
        {
            ApplicationUser user = _userManager.FindByEmail(email);
            return user;
        }

        public ApplicationUser FindUserDetailByUserName(string username)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                ApplicationUser user = userManager.FindByName(username);
                //IdentityUser user = _userManager.FindByName(username);
                return user;
            }

        }

        public bool IsInRole(string UserName, string role)
        {
            ApplicationUser user = FindUserDetailByUserName(UserName);
            if (user != null)
            {
                return _userManager.IsInRole(user.Id, role);
            }
            return false;
        }

        public string ResetPassword(string email)
        {
            string url = null;
            ApplicationUser user = FindUserDetailByEmail(email);
            if (user != null)
            {
                var randomNumber = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + DateTime.Now.Ticks;
                randomNumber = System.Text.RegularExpressions.Regex.Replace(randomNumber, "[^a-z0-9]+", "");
                if (randomNumber.Length > 15)
                    randomNumber = randomNumber.Substring(0, 15);
                user.PasswordHash = EncryptDecrypt.EncodePassword(randomNumber);
                var removsss=_userManager.RemovePassword(user.Id);
                var resss=_userManager.AddPassword(user.Id,randomNumber);
                url = "&userId=" + user.Id + "&code=" + randomNumber;
            }
            return url;
        }

        public IdentityResult CreateDefaultUser(ApplicationUser user)
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
            if (!string.Equals(oldrole, updatedrole))
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
        public IdentityResult ChangePassword(PasswordChangeModel model)
        {
            return _userManager.ChangePassword(model.UserId, model.CurrentPassword, model.NewPassword);
        }

        public IdentityResult LockUnlockUser(string userid,bool enable)
        {
            return _userManager.SetLockoutEnabled(userid, enable);
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