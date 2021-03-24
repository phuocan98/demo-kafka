using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Project.App.Databases
{
    public class MongoDBContext : DbContext
    {
        private readonly IMongoDatabase MongoDatabase;
        private readonly IConfiguration Configuration;
        public MongoDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
            MongoClient client = new MongoClient(Configuration["ConnectionSetting:MongoDBSettings:ConnectionStrings"]);
            MongoDatabase = client.GetDatabase(Configuration["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
        }

        public IMongoCollection<dynamic> Users => MongoDatabase.GetCollection<dynamic>("users");
    }
}
