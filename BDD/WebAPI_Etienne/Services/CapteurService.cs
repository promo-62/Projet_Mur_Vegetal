using System;
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

        // récupère tous les éléments de {collection} avec les filtres {filter} et la {projection}
        public List<T> Get<T>(string Collection, string filter = "{}", string projection = "{}") 
        {            
            return _database.GetCollection<T>(Collection)
                .Find(filter)
                .Project<T>(projection)
                .ToList();
        }
        
        // récupère le dernier relevé pour chaque capteur
        public List<T> GetDerniersReleves<T>(string Collection, string group, string projection = "{}", string sort = "{}") // récupère la liste de tous les éléments de la collection 
        {
            return _database.GetCollection<T>(Collection)
                .Aggregate<T>()
                .Sort(sort)
                .Group<T>(group)
                .Project<T>(projection)
                .ToList();
        }

        public T GetById<T>(string Collection, string id) // récupère l'élément d'{id} dans la {collection}
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
            //_database.GetCollection<ICollectionModel>(Collection).ReplaceOne(template => template.Id == id, templateIn);
        }

        public void Remove(string Collection, ICollectionModel templateIn)
        {
            _database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == templateIn.Id);
        }

        public void Remove(string Collection, string id)
        {
            //_database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == id);
        }

        public void getUser(string username) 
        {
            Console.WriteLine(_database.GetCollection<ICollectionModel>("Users").Find("{ \"username\" : \"" + username + "\"}") );
        }
    }
}