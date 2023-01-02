using boox.api.Helpers;
using Microsoft.AspNetCore.Mvc;
using boox.api.Infrasructure.Models.Users.Settings;
using boox.api.Infrastructure.Managers.Users;

namespace boox.api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("change/password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            try
            {
                var result = new UserManager().ChangePassword(AuthHelpers.CurrentUserID(HttpContext), currentPassword, newPassword);
                return Ok(result);
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
                var result = new UserManager().UpdateEmail(AuthHelpers.CurrentUserID(HttpContext), email);
                return Ok(result);
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
                var result = new UserManager().ManageAddresses(entity, AuthHelpers.CurrentUserID(HttpContext));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
