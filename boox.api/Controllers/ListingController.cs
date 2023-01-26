using boox.api.Helpers;
using Microsoft.AspNetCore.Mvc;
using boox.api.Infrasructure.Models.Helpers;
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
        [Route("filter")]
        public async Task<IActionResult> FilteredListing([FromQuery] Filter filter, [FromQuery] Listing? filterModel)
        {
            try
            {
                var model = filterModel ?? new Listing();
                filter.pageSize = 25;
                FilteredList<Listing> request = new FilteredList<Listing>()
                {
                    filter = filter,
                    filterModel = model,
                };
                var result = await new ListingManager().FilteredList(request, null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("filter/self")]
        public async Task<IActionResult> FilteredSelfListing([FromQuery] Filter filter, [FromQuery] Listing? filterModel)
        {
            try
            {
                var model = filterModel ?? new Listing();
                filter.pageSize = 25;
                FilteredList<Listing> request = new FilteredList<Listing>()
                {
                    filter = filter,
                    filterModel = model,
                };
                var result = await new ListingManager().FilteredList(request, AuthHelpers.CurrentUserID(HttpContext));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get/listing")]
        public async Task<IActionResult> GetListing([FromQuery] int? ID)
        {
            try
            {
                var result = await new ListingManager().Get(ID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> DeleteListing([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, AuthorizedAuthType))
                {
                    dynamic result = await new ListingManager().ToggleListing(ID, AuthHelpers.CurrentUserID(HttpContext));
                    if (result)
                    {
                        var filterModel = new Listing();
                        var filter = new Filter();
                        filter.pageSize = 25;
                        FilteredList<Listing> request = new FilteredList<Listing>()
                        {
                            filter = filter,
                            filterModel = filterModel,
                        };
                        result = await new ListingManager().FilteredList(request, AuthHelpers.CurrentUserID(HttpContext));
                    }
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

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Testing()
        {
            try
            {
                return Ok("hit");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
