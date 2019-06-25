using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Tables : ICollectionModel
    {
        [BsonElement("OnScreenTime")]
        public int OnScreenTime { get; set; }

        [BsonElement("IsOnScreen")]
        public bool IsOnScreen { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("CarrousselTime")]
        public int CarrousselTime { get; set; }
    }
}
