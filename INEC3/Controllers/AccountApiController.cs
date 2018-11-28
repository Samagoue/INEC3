using INEC3.Models;
using INEC3.Models.Service;
using INEC3.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace INEC3.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {
        private AuthRepository _repository = null;
        private AccountService _accountService;
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
                IdentityUser user;
                if (!string.IsNullOrEmpty(userModel.UserName) && !string.IsNullOrEmpty(userModel.Password))
                {
                    //user = await _repository.FindUser(userModel.UserName, userModel.Password);
                    user =  _repository.FindUserDetail(userModel.Email);
                    if (!_accountService.SendEmailVerification(user.UserName, user.Id))
                    {
                        return BadRequest("Eamil sending fail. try after some time.");
                    }

                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IdentityUser> LoginAsync(UserLogin userlogin)
        {
            IdentityUser user;
            if (!string.IsNullOrEmpty(userlogin.UserName) && !string.IsNullOrEmpty(userlogin.Password))
            {
                user = await _repository.FindUser(userlogin.UserName, userlogin.Password);
                return user;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                response.Content = new StringContent("Enter Username or Password");
            }
            return null;
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
