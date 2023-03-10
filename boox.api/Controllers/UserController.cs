using boox.api.Helpers;
using Microsoft.AspNetCore.Mvc;
using boox.api.Infrastructure.Managers.Users;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private static int AuthorizedAuthType = 1;

        [HttpGet]
        [Route("change/password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().ChangePassword(AuthHelpers.CurrentUserID(HttpContext), currentPassword, newPassword); return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("update/email")]
        public async Task<IActionResult> UpdateEmail([FromQuery] string email)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().UpdateEmail(AuthHelpers.CurrentUserID(HttpContext), email);
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("manage/addresses")]
        public async Task<IActionResult> ManageAddresses([FromBody] UserAddresses entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().ManageAddresses(entity, AuthHelpers.CurrentUserID(HttpContext));
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("delete/address")]
        public async Task<IActionResult> DeleteAddress([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new UserManager().DeleteAddress(ID, AuthHelpers.CurrentUserID(HttpContext));
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Authorization failed");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
