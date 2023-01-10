using Dapper.Contrib.Extensions;

namespace boox.api.Infrastructure.Models.Listings
{
    [Table("genres")]
    public class Genres
    {
        [Key]
        public int ID { get; set; }
        public string name{ get; set; }
    }
}
