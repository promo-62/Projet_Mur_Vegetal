using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Socials : ICollectionModel
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PageWidget")]
        public string PageWidget { get; set; }

        [BsonElement("Widget")]
        public string Widget { get; set; }
    }
}
