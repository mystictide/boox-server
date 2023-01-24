using Dapper.Contrib.Extensions;
using boox.api.Infrastructure.Models.Listings;

namespace boox.api.Infrasructure.Models.Listings
{
    [Table("listing")]
    public class Listing
    {
        [Key]
        public int? ID { get; set; }
        public int UserID { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Edition { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int Year { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        [Write(false)]
        public IEnumerable<Photos>? Photos { get; set; }
        [Write(false)]
        public IEnumerable<Genres>? Genre { get; set; }
        [Write(false)]
        public string? Genres { get; set; }
    }
}