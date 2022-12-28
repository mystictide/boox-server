using boox.api.Helpers;
using Microsoft.AspNetCore.Mvc;
using boox.api.Infrasructure.Models.Users.Settings;
using boox.api.Infrastructure.Managers.Users;
using boox.api.Infrastructure.Models.Helpers;
using Microsoft.Extensions.Options;

namespace boox.api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IOptions<AppSettings> _settings;

        public UserController(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        [HttpGet]
        [Route("change/password")]
        public async Task<IActionResult> ChangePassword([FromQuery] string currentPassword, [FromQuery] string newPassword)
        {
            try
            {
                var result = new UserManager(_settings).ChangePassword(AuthHelpers.CurrentUserID(HttpContext, _settings.Value.Secret), currentPassword, newPassword);
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
                var result = new UserManager(_settings).UpdateEmail(AuthHelpers.CurrentUserID(HttpContext, _settings.Value.Secret), email);
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
                var result = new UserManager(_settings).ManageAddresses(entity, AuthHelpers.CurrentUserID(HttpContext, _settings.Value.Secret));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
