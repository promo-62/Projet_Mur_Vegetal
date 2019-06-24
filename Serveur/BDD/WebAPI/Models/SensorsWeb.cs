using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SensorsWeb : ICollectionModel
    {
        [BsonElement("IdSensor")]
        public int IdSensor { get; set; }

        [BsonElement("IdSensorType")]
        public int IdSensorType { get; set; }

        [BsonElement("Project")]
        public List<string> Project { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
