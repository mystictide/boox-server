using Dapper.Contrib.Extensions;

namespace boox.api.Infrastructure.Models.Listings
{
    [Table("photos")]
    public class Photos
    {
        [Key]
        public int? ID { get; set; }
        public int? ListingID { get; set; }
        public string? Path { get; set; }

        [Write(false)]
        public string data { get; set; }
    }
}