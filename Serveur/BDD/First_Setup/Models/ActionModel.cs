using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ActionModel
    {
        [BsonElement("ToDo")]
        public int ToDo { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
