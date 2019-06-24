using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebAPI.Services
{
    public class WebService
    {
        private readonly IMongoDatabase _database;

        public WebService(IConfiguration config)
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

        // récupère le dernier relevé pour chaque capteur avec la {projection} et un tri {sort}
        public List<T> GetDerniersReleves<T>(string Collection, string group, string projection = "{}", string sort = "{}")
        {
            return _database.GetCollection<T>(Collection)
                .Aggregate<T>()
                .Sort(sort)
                .Group<T>(group)
                .Project<T>(projection)
                .ToList();
        }

        // récupère l'élément d'{id} dans la {collection}
        public T GetById<T>(string Collection, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            return _database.GetCollection<T>(Collection).Find<T>(filter).FirstOrDefault();
        }

        // insère un élément dans la {collection}
        public ICollectionModel Create(string Collection, ICollectionModel template)
        {
            _database.GetCollection<ICollectionModel>(Collection).InsertOne(template);
            return template;
        }

        // met à jour tous les champs de l'élément avec l'{id} dans la {collection}
        public void Update(string Collection, string id, ICollectionModel templateIn)
        {
            _database.GetCollection<ICollectionModel>(Collection).ReplaceOne(template => template.Id == id, templateIn);
        }

        // supprime l'élément {templateIn} dans la {collection}
        public void Remove(string Collection, ICollectionModel templateIn)
        {
            _database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == templateIn.Id);
        }

        // supprime l'élément avec l'{id} dans la {collection}
        public void Remove(string Collection, string id)
        {
            _database.GetCollection<ICollectionModel>(Collection).DeleteOne(template => template.Id == id);
        }

    }
}