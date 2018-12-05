using INEC3.Models;
using INEC3.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                var property = new Dictionary<string, string>();
                using (AuthRepository _repo = new AuthRepository())
                {
                    IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                    if (user == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }
                    else if (!user.EmailConfirmed && !DevloperMode)
                    {
                        context.SetError("invalid_grant", "Verify your email address.");
                        return;
                    }
                    //Identity Property Set
                    property = getproperty(user.Id,user.UserName);
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("UserName", context.UserName));
                    identity.AddClaim(new Claim("Email", user.Email));
                    identity.AddClaim(new Claim("UserId", user.Id));
                    var props = new AuthenticationProperties(property);
                    var ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
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

            return Task.FromResult<object>(null);
        }

        public Dictionary<string, string> getproperty(string userid,string displayname)
        {
            //Guid useridg = Guid.Parse(userid);
            Dictionary<string, string> d = new Dictionary<string, string>();
            //inecDBContext _db = new inecDBContext();
            //var uf = _db.UserProfile.Where(w => w.AspNetUsersId == useridg).FirstOrDefault();
            //if (uf != null)
            //{
            d.Add("userid", userid);
            d.Add("displayname", displayname);
            //d.Add("profileimg", "picgoesr here");
            //}

            return d;
        }
    }
}