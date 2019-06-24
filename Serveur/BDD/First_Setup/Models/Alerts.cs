using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Alerts : ICollectionModel
    {
        [BsonElement("IdSensor")]
        public int IdSensor { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("DateAlert")]
        public long DateAlert { get; set; }

        [BsonElement("IsWorking")]
        public bool IsWorking { get; set; }

        [BsonElement("AlertReason")]
        public string AlertReason { get; set; }
    }
}
