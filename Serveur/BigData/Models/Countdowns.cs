using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Countdowns : ICollectionModel
    {
        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("EndingDateEvent")]
        public long EndingDateEvent { get; set; }

        [BsonElement("BeginningDateEvent")]
        public long BeginningDateEvent { get; set; }

        [BsonElement("EndingDateCountdown")]
        public long EndingDateCountdown { get; set; }

        [BsonElement("Posititon")]
        public int Position { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }
    }
}
