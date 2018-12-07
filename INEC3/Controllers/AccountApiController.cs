using INEC3.Models;
using INEC3.Models.Service;
using INEC3.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using INEC3.Helper;
using System.Web.Security;
using System.Web.Mvc;
using INEC3.IdentityClass;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace INEC3.Controllers
{
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {
        private AuthRepository _repository = null;
        private AccountService _accountService;
        private bool DevloperMode =Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DevloperMode"]);
        Base _base = new Base();
        public AccountApiController()
        {
            _repository = new AuthRepository();
            _accountService = new AccountService();
        }

        //[AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            try
            {
                userModel.UserName = userModel.Email;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                IdentityResult result = await _repository.RegisterUser(userModel);
                IHttpActionResult errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }
                if (!DevloperMode)
                {
                    IdentityUser user;
                    if (!string.IsNullOrEmpty(userModel.UserName) && !string.IsNullOrEmpty(userModel.Password))
                    {

                        user = _repository.FindUserDetailByEmail(userModel.Email);
                        FormsAuthentication.SetAuthCookie(userModel.Email, false);
                        if (!_accountService.SendEmailVerification(user.Email, user.Id))
                        {
                            return BadRequest("Email sending fail. try after some time.");
                        }

                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Login")]
        public async Task<HttpResponseMessage> LoginUser(UserLogin model)
        {
            // Invoke the "token" OWIN service to perform the login: /api/token
            // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                  new KeyValuePair<string, string>("grant_type", "password"),
                  new KeyValuePair<string, string>("username", model.Email),
                  new KeyValuePair<string, string>("password", model.Password)
                };

                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<LoginUser>(responseString);
                var responseCode = tokenServiceResponse.StatusCode;

                HttpResponseMessage response = new HttpResponseMessage();
                if (!string.IsNullOrEmpty(res.access_token))
                {
                    _base.SaveCookie("inceusername", res.displayname);
                    return Request.CreateResponse<LoginUser>(HttpStatusCode.OK, res);
                }
               else if (!string.IsNullOrEmpty(res.error_description))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, res.error_description);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Credentials");
                }
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ForgotPassword")]
        public JsonResult ForgotPassword(string email)
        {
            JsonResult res = new JsonResult();
            try
            {
                string resttoken = _repository.GeneratePasswordResetToken(email);
                
                if (!string.IsNullOrEmpty(resttoken))
                {
                    if (_accountService.ForgotPassword(resttoken,email))
                    {
                        res.Data = "Check your email and verify";
                        return res;
                    }
                }
                else
                {
                    res.ContentType = "fail";
                    res.Data = "Email not found";
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }
        
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                if (ModelState.IsValid)
                {
                    return BadRequest();
                }
                return BadRequest(ModelState);
            }
            return null;
        }

        [System.Web.Http.Route("IsInRole")]
        public bool IsInRole(string username, string role)
        {
            return _repository.IsInRole(username, role);
        }
        public UserDisplay FindUserDetailByKey(string username)
        {
            try
            {
                return _accountService.FindUserDisplay("UserName", username);
            }
            catch (Exception ex)
            {
                return null;

            }
        }
    }
}
