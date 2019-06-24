using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Screens : ICollectionModel
    {
        [BsonElement("OnDate")]
        public long OnDate { get; set; }

        [BsonElement("OffDate")]
        public long OffDate { get; set; }

        [BsonElement("Delay")]
        public int Delay { get; set; }
    }
}
