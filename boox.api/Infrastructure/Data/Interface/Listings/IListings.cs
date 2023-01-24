using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrasructure.Models.Listings;
using boox.api.Infrastructure.Models.Listings;

namespace boox.api.Infrastructure.Data.Interface.Listings
{
    public interface IListings
    {
        Task<Listing>? Get(int? ID);
        Task<IEnumerable<Genres>>? GetGenres();
        Task<bool>? ToggleListing(int ID, int UserID);
        Task<Listing> ManageListing(Listing entity, int UserID);
        Task<IEnumerable<Photos>> ManagePhotos(Photos entity, int UserID);
        Task<IEnumerable<Photos>> DeletePhoto(int ID, int ListingID, int UserID);
        Task<FilteredList<Listing>> FilteredList(FilteredList<Listing> request, int? UserID);
    }
}
