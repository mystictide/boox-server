using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrasructure.Models.Listings;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Models.Listings;
using boox.api.Infrastructure.Data.Repo.Listings;
using boox.api.Infrastructure.Data.Interface.Listings;

namespace boox.api.Infrastructure.Managers.Listings
{
    public class ListingManager : AppSettings, IListings
    {
        private readonly IListings _repo;
        public ListingManager()
        {
            _repo = new ListingRepository();
        }

        public async Task<IEnumerable<Photos>> DeletePhoto(int ID, int ListingID, int UserID)
        {
            return await _repo.DeletePhoto(ID, ListingID, UserID);
        }

        public async Task<FilteredList<Listing>> FilteredList(FilteredList<Listing> request)
        {
            return await _repo.FilteredList(request);
        }

        public async Task<Listing>? Get(int? ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<IEnumerable<Genres>>? GetGenres()
        {
            return await _repo.GetGenres();
        }

        public async Task<Listing> ManageListing(Listing entity, int UserID)
        {
            return await _repo.ManageListing(entity, UserID);
        }

        public async Task<IEnumerable<Photos>> ManagePhotos(Photos entity, int UserID)
        {
            return await _repo.ManagePhotos(entity, UserID);
        }

        public async Task<bool>? ToggleListing(int ID, int UserID)
        {
            return await _repo.ToggleListing(ID, UserID);
        }
    }
}
