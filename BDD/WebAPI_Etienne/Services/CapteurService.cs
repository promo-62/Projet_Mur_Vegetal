using System.Collections.Generic;
using System.Linq;
using CapteursApi.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CapteursApi.Services
{
    public class CapteurService
    {
        private readonly IMongoDatabase _database;

        public CapteurService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MurVegetalDb"));
            _database = client.GetDatabase("MurVegetalDb");
        }

        public List<T> Get<T>(string Collection)
        {
            return _database.GetCollection<T>(Collection).Find(template => true).ToList();
        }

        public T Get<T>(string Collection, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return _database.GetCollection<T>(Collection).Find<T>(filter).FirstOrDefault();
        }

        public ICollectionModel Create(string Collection, ICollectionModel template)
        {
            _database.GetCollection<ICollectionModel>(Collection).InsertOne(template);
            return template;
        }

        public void Update(string Collection, string id, ICollectionModel templateIn)
        {
            _database.GetCollection<ICollectionModel>(Collection).ReplaceOne(template => template.Id == id, templateIn);
        }

        public void Remove(string Collection, ICollectionModel templateIn)
        {
            _database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == templateIn.Id);
        }

        public void Remove(string Collection, string id)
        {
            _database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == id);
        }
    }
}