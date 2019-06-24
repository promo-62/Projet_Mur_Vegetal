using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SamplesWeb : ICollectionModel
    {
        [BsonElement("IdSensor")]
        public int IdSensor { get; set; }

        [BsonElement("Value")]
        public int Value { get; set; }
    }
}
