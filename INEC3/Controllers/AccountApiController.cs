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
using System.Security.Claims;

namespace INEC3.Controllers
{
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {
        private AuthRepository _repository = null;
        private AccountService _accountService;
        private bool DevloperMode = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DevloperMode"]);
        Base _base = new Base();
        public AccountApiController()
        {
            _repository = new AuthRepository();
            _accountService = new AccountService();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
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
                var isexist = _repository.FindUserDetailByUserName(userModel.UserName);
                if (isexist != null)
                {
                    return BadRequest("email is already registered.");
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
                string resttoken = _repository.ResetPassword(email);

                if (!string.IsNullOrEmpty(resttoken))
                {
                    if (_accountService.RestPasswordsendmail(resttoken, email))
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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize]
        [System.Web.Http.Route("PasswordChange")]
        public JsonResult PasswordChange(PasswordChangeModel obj)
        {
            JsonResult res = new JsonResult();
            try
            {
                string UserId = "";
                var claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    UserId = claimsIdentity?.FindFirst(c => c.Type == "UserId")?.Value;
                }
                obj.UserId = UserId;
                if (string.IsNullOrEmpty(UserId) || !ModelState.IsValid)
                {
                    res.ContentType = "fail";
                    res.Data = "Invalid request";
                    return res;
                }
                else
                {
                    var result = _repository.ChangePassword(obj);
                    if (result.Succeeded)
                    {
                        res.Data = "Password change successfully";
                    }
                    else
                    {
                        res.ContentType = "fail";
                        res.Data = result.Errors;
                    }
                }
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ResetPasswordChange")]
        public JsonResult ResetPasswordChange(PasswordChangeModel obj)
        {
            JsonResult res = new JsonResult();
            try
            {
                
                if (string.IsNullOrEmpty(obj.UserId) || !ModelState.IsValid)
                {
                    res.ContentType = "fail";
                    res.Data = "Invalid request";
                    return res;
                }
                else
                {
                    var result = _repository.ChangePassword(obj);
                    if (result.Succeeded)
                    {
                        res.Data = "Password change successfully";
                    }
                    else
                    {
                        res.ContentType = "fail";
                        res.Data = result.Errors;
                    }
                }
            }
            catch (Exception ex)
            {
                res.ContentType = "error";
                res.Data = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return res;
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
        public UserDisplay FindUserDetailByKey(string key, string value)
        {
            try
            {
                return _accountService.FindUserDisplay(key, value);
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
                _accountService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
