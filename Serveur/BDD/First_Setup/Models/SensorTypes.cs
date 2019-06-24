using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SensorTypes : ICollectionModel
    {
        [BsonElement("IdSensorType")]
        public int IdSensorType { get; set; }

        [BsonElement("SamplesTypes")]
        public List<string> SamplesTypes { get; set; }

        [BsonElement("SensorIds")]
        public List<string> SensorIds { get; set; }
    }
}
