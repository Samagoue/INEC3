using INEC3.App_Start;
using INEC3.Models;
using INEC3.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace INEC3.Providers
{

    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private bool DevloperMode = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DevloperMode"]);

        private readonly string _publicClientId;

        public AuthorizationServerProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                ApplicationUser user = await userManager.FindByNameAsync(context.UserName);
                
                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
                if (!user.EmailConfirmed && !DevloperMode)
                {
                    context.SetError("invalid_grant", "Verify your email address.");
                    return;
                }
                if (userManager.SupportsUserLockout && userManager.IsLockedOut(user.Id) && userManager.GetAccessFailedCount(user.Id) < 5)
                {
                    context.SetError("invalid_grant", "Your account has been locked. Contact System Administrator.");
                    return;
                }
                if (user.LockoutEnabled)
                {
                    context.SetError("invalid_grant", "Your account has been locked. Contact System Administrator.");
                    return;
                }
                
                if (userManager.CheckPassword(user, context.Password))
                {
                    // Authenticate user
                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

                    //AuthenticationProperties properties = CreateProperties(user.UserName);
                    string role = userManager.GetRoles(user.Id).FirstOrDefault();
                    IDictionary<string, string> data = new Dictionary<string, string>
                                {
                                     { "userid", user.Id },
                                     { "displayname", user.UserName },
                                     { "role", role },
                                };
                    if (role == UserManageRoles.SuperAdmin)
                    {
                        data.Add("returnUrl", "/Admin/AdminIndex");
                    }
                    else if (role == UserManageRoles.User)
                    {
                        data.Add("returnUrl", "../");
                    }
                    else
                    {
                        data.Add("returnUrl", "/Admin");
                    }
                    AuthenticationProperties properties = new AuthenticationProperties(data);
                    //AuthenticationProperties properties = CreateProperties(user.UserName, user.Id);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);
                }
                else
                {
                    if (userManager.GetAccessFailedCount(user.Id) == 5 && userManager.IsLockedOut(user.Id))
                    {
                        context.SetError("invalid_grant", "Your account is locked as you have tried 5 times with incorrect password. Please contact your system administrator.");
                        return;
                    }
                    else
                    {
                        userManager.AccessFailed(user.Id);
                    }
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }


            }
            catch (Exception ex)
            {
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            context.AdditionalResponseParameters.Add("UserId", context.Identity.GetUserId());
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string id)
        //public static AuthenticationProperties CreateProperties(string userid, string displayname)
        {
            using (AuthRepository _repo = new AuthRepository())
            {
                string role = _repo.GetUserRole(id);
                IDictionary<string, string> data = new Dictionary<string, string>
                {
                     { "userid", id },
                     { "displayname", userName },
                     { "role", role },
                };
                if (role == UserManageRoles.SuperAdmin)
                {
                    data.Add("returnUrl", "/Admin/AdminIndex");
                }
                else if (role == UserManageRoles.User)
                {
                    data.Add("returnUrl", "../");
                }
                else
                {
                    data.Add("returnUrl", "/Admin");
                }
                return new AuthenticationProperties(data);
            }

        }


        public Dictionary<string, string> getproperty(string userid, string displayname)
        {
            using (AuthRepository _repo = new AuthRepository())
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                string role = _repo.GetUserRole(userid);
                d.Add("userid", userid);
                d.Add("displayname", displayname);
                d.Add("role", _repo.GetUserRole(userid));
                if (role == UserManageRoles.SuperAdmin)
                {
                    d.Add("returnUrl", "/Admin/AdminIndex");
                }
                else if (role == UserManageRoles.User)
                {
                    d.Add("returnUrl", "../");
                }
                else
                {
                    d.Add("returnUrl", "/Admin");
                }
                return d;
            }
        }
    }
}