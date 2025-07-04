using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace street.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase StreetStoreDb;

        public MongoDbContext(IOptions<Config.MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            StreetStoreDb = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return StreetStoreDb.GetCollection<T>(name);
        }
    }
}
