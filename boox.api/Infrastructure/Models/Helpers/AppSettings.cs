using MongoDB.Driver;

namespace boox.api.Infrastructure.Models.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public static string connectionString { get; set; }
        public static string dbName { get; set; }

        public static IMongoDatabase GetDB
        {
            get
            {
                var client = new MongoClient(connectionString);
                return client.GetDatabase(dbName);
            }
        }
    }
}
