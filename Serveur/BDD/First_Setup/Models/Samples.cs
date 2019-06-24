using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Samples : ICollectionModel
    {
        [BsonElement("IdSensor")]
        public int IdSensor { get; set; }

        [BsonElement("IdSampleType")]
        public int IdSampleType { get; set; }

        [BsonElement("Note")]
        public string Note { get; set; }

        [BsonElement("SampleDate")]
        public long SampleDate { get; set; }

        [BsonElement("Value")]
        public int Value { get; set; }
    }
}
