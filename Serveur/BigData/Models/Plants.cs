using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Plants : ICollectionModel
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Temperature")]
        public string Temperature { get; set; }

        [BsonElement("Humidity")]
        public string Humidity { get; set; }

        [BsonElement("Luminosity")]
        public string Luminosity { get; set; }

        [BsonElement("PositionX")]
        public int PositionX { get; set; }

        [BsonElement("PositionY")]
        public int PositionY { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }
    }
}
