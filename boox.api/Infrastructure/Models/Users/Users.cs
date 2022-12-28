using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrasructure.Models.Users
{
    public class Users
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? ID { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? AuthType { get; set; }

        [BsonIgnore]
        public string? Token { get; set; }
        [BsonIgnore]
        public IEnumerable<UserAddresses>? Addresses { get; set; }
    }
}
