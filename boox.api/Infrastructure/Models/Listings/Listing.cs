using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace boox.api.Infrasructure.Models.Listings
{
    public class Listing
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ID { get; set; }
        public string UserID { get; set; }
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