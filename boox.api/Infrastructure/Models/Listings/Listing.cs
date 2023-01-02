using Dapper.Contrib.Extensions;

namespace boox.api.Infrasructure.Models.Listings
{
    [Table("Listing")]
    public class Listing
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Edition { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int Year { get; set; }
        public string? Notes { get; set; }
        public DateTime ListingDate { get; set; }
        public bool IsActive { get; set; }
    }
}