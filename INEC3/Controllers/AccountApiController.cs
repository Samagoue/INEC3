using INEC3.Models;
using INEC3.Models.Service;
using INEC3.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace INEC3.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {
        private AuthRepository _repository = null;
        private AccountService _accountService;
        private bool DevloperMode =Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["DevloperMode"]);
        public AccountApiController()
        {
            _repository = new AuthRepository();
            _accountService = new AccountService();
        }

        //[AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            try
            {
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

                        user = _repository.FindUserDetail(userModel.Email);
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

        //[HttpPost]
        //[Route("Login")]
        //public async Task<IdentityUser> LoginAsync(UserLogin userlogin)
        //{
        //    IdentityUser user;
        //    if (!string.IsNullOrEmpty(userlogin.UserName) && !string.IsNullOrEmpty(userlogin.Password))
        //    {
        //        user = await _repository.FindUser(userlogin.UserName, userlogin.Password);
        //        return user;
        //    }
        //    else
        //    {
        //        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        //        response.Content = new StringContent("Enter Username or Password");
        //    }
        //    return null;
        //}


        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
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
                    //FormsAuthentication.SetAuthCookie(res.displayname,true);
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
    }
}
