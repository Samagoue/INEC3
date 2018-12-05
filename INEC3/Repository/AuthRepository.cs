using System;
using INEC3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using INEC3.IdentityClass;
using INEC3.Models.Service;

namespace INEC3.Repository
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _context;
        private UserManager<IdentityUser> _userManager;
        private AccountService _accountService;
        private inecDBContext _inecDBContext;

        public AuthRepository()
        {
            _context = new AuthContext();
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

            var roleresult = _userManager.AddToRole(user.Id, "User");

            if (result.Succeeded)
            {
                userModel.UserProfile.AspNetUsersId = Guid.Parse(user.Id);
                IdentityResult res = _accountService.RegisterUserProfile(userModel.UserProfile);
                if (!res.Succeeded)
                    return res;
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

        //public bool FindUser(string email)
        //{
        //    var result = _userManager.FindByEmail(email);
        //    if (result == null)
        //        return false;
        //    else
        //        return true;
        //}

        public void Dispose()
        {
            _context.Dispose();
            _userManager.Dispose();
            _inecDBContext.Dispose();
        }
    }
}