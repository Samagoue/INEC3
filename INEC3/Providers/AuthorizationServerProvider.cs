﻿using INEC3.Models;
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
                    context.SetError("invalid_grant", "Confirm email.");
                    return;
                }
                //property = getproperty(user.Id);
                property.Add("Email",user.Email);
                property.Add("Id", user.Id);
                property.Add("Name", user.UserName);
                //
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, "test"));
                claims.Add(new Claim(ClaimTypes.Email, "test@gmail.com"));
                var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties(property);
                    //var user = new ApplicationUser() { UserName = model.UserName  };
                    //ClaimsIdentity oAuthIdentity = await userr.GenerateUserIdentityAsync(usermanager, OAuthDefaults.AuthenticationType);
                    //ClaimsIdentity cookiesIdentity = await userr.GenerateUserIdentityAsync(usermanager, CookieAuthenticationDefaults.AuthenticationType);
                    context.Request.Context.Authentication.SignIn(authenticationProperties);

                //
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            //var props = new AuthenticationProperties(new Dictionary<string, string>
            //        {
            //            {
            //                "surname", "Smith"
            //            },
            //            {
            //                "age", "20"
            //            },
            //            {
            //            "gender", "Male"
            //            }
            //        });
            var props = new AuthenticationProperties(property);
            var ticket = new AuthenticationTicket(identity, props);
            //context.Validated(identity);
            context.Validated(ticket);
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

        public Dictionary<string, string> getproperty(string userid)
        {
            Guid useridg = Guid.Parse(userid);
            Dictionary<string, string> d = new Dictionary<string, string>();
            inecDBContext _db = new inecDBContext();
            //var uf = _db.UserProfile.Where(w => w.AspNetUsersId == useridg).FirstOrDefault();
            //if (uf != null)
            //{
                d.Add("userid", "123");
                d.Add("displayname", "Vardhik");
                d.Add("profileimg", "picgoesr here");
            //}

            return d;
        }
    }
}