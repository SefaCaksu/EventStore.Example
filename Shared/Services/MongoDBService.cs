using System;
using MongoDB.Driver;
using Shared.Services.Abstractions;

namespace Shared.Services
{
    public class MongoDBService : IMongoDBService
    {

        public MongoDBService()
        {
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            IMongoDatabase database = GetDatabase();
            return database.GetCollection<T>(collectionName);
        }

        //TODO: appsettings connection taşı
        public IMongoDatabase GetDatabase(string database = "ProductDB", string connectionString = "mongodb://localhost:27017")
        {
            MongoClient mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(database);
        }
    }
}

