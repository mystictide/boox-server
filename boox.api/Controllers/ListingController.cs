using boox.api.Helpers;
using Microsoft.AspNetCore.Mvc;
using boox.api.Infrasructure.Models.Listings;
using boox.api.Infrastructure.Models.Listings;
using boox.api.Infrastructure.Managers.Listings;

namespace boox.api.Controllers
{
    [ApiController]
    [Route("listing")]
    public class ListingController : Controller
    {
        private IWebHostEnvironment _env;
        private static int AuthorizedAuthType = 1;

        public ListingController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("get/genres")]
        public async Task<IActionResult> GetGenres()
        {
            try
            {
                var result = await new ListingManager().GetGenres();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("manage")]
        public async Task<IActionResult> ManageListing([FromBody] Listing entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new ListingManager().ManageListing(entity, AuthHelpers.CurrentUserID(HttpContext));
                    return Ok(result);
                }
                return StatusCode(500, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("upload/photo")]
        public async Task<IActionResult> SavePhoto([FromBody] Photos entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    dynamic result = "";
                    if (entity.data.Length > 0)
                    {
                        entity.Path = await CustomHelpers.SaveListingPhoto(entity.ListingID.Value, _env.ContentRootPath, entity.data);
                        if (entity.Path != null)
                        {
                            result = await new ListingManager().ManagePhotos(entity, AuthHelpers.CurrentUserID(HttpContext));
                        }
                        else
                        {
                            return StatusCode(401, "Failed to save image");
                        }
                    }
                    return Ok(result);
                }
                return StatusCode(500, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("delete/photo")]
        public async Task<IActionResult> DeletePhoto([FromQuery] int ID, [FromQuery] int ListingID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    var result = await new ListingManager().DeletePhoto(ID, ListingID, AuthHelpers.CurrentUserID(HttpContext));
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
