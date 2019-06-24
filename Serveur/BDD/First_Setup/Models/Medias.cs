using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Medias : ICollectionModel
    {
        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("BeginningDate")]
        public long BeginningDate { get; set; }

        [BsonElement("EndingDate")]
        public long EndingDate { get; set; }

        [BsonElement("Video")]
        public string Video { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }
    }
}
