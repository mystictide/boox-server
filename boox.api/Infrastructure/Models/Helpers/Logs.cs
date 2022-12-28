using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace boox.api.Infrasructure.Models.Helpers
{
    public class Logs
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ID { get; set; }
        public int UserID { get; set; }
        public string? Message { get; set; }
        public string? Source { get; set; }
        public int Line { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
