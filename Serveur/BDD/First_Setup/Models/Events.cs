using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Events : ICollectionModel
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("EventDate")]
        public long EventDate { get; set; }

        [BsonElement("BeginningDate")]
        public long BeginningDate { get; set; }

        [BsonElement("EventImage")]
        public string EventImage { get; set; }

        [BsonElement("EndingDate")]
        public long EndingDate { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("Position")]
        public int Position { get; set; }
    }
}
