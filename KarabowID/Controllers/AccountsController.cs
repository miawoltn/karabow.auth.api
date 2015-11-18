using KarabowID.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using KarabowID.Models;
using System.Net.Mail;
using System.Net;

namespace KarabowID.Controllers
{
    [RoutePrefix("accounts")]
    public class AccountsController : BaseApiController
    {
       // [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Authorize]
        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
                
            }

            return NotFound();

        }

       // [Authorize]
        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user_name = this.RequestContext.Principal.Identity.Name;

            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [AllowAnonymous]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(CreateUserBindingModel createUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = createUserModel.Username,
                Email = createUserModel.Email,
                FullName = createUserModel.FullName,
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));
            
            try
            {
                await this.AppUserManager.SendEmailAsync(user.Id, "Confirm your account", @"Please confirm your account by clicking <a href="" + callbackUrl"" + "">here</a>");

            }
            catch(Exception ex)
            {
                return Ok("Email not sent :" + ex);
            }
            
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
           // return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<HttpResponseMessage> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                var response = Request.CreateResponse(HttpStatusCode.BadRequest, ModelState, "application/json");
                response.ReasonPhrase = "Bad request";
                return response;
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);
            
            if (result.Succeeded)
            {
                //redirect the user to a success page after a successful email confirmation
                var response = Request.CreateResponse(HttpStatusCode.Redirect, ModelState, "application/json");
                response.Headers.Location = new Uri("http://localhost:20204/UI/Email_Confirmation?confirmed="+result.Succeeded.ToString().ToLower());
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }


        [Authorize(Roles = "Admin")]
        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {

            //Only SuperAdmin or Admin can delete users (Later when implement roles)

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }

        [Authorize(Roles="Admin")]
        [Route("user/{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id,[FromBody] string[] rolesToAssign)
        {
            var appuser = await this.AppUserManager.FindByIdAsync(id);

            if(appuser == null)
            {
                return NotFound();
            }

            var currentRoles = await this.AppUserManager.GetRolesAsync(appuser.Id);

            var rolesNotExist = rolesToAssign.Except(this.AppRoleManager.Roles.Select(x => x.Name)).ToArray();
 
            if(rolesNotExist.Count() > 0)
            {
                ModelState.AddModelError("", string.Format("Roles '{0}' does not exist in the system", string.Join(",", rolesNotExist)));
                return BadRequest(ModelState);
            }

            IdentityResult removeResult = await this.AppUserManager.RemoveFromRolesAsync(appuser.Id, currentRoles.ToArray());

            if(!removeResult.Succeeded)
            {
                ModelState.AddModelError("","Failed to remove user from roles");
                return BadRequest(ModelState);
            }

            IdentityResult addResult = await this.AppUserManager.AddToRolesAsync(appuser.Id, rolesToAssign);

            if(!addResult.Succeeded)
            {
                ModelState.AddModelError("","Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();

        }
    }
}