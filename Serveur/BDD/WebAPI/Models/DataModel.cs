using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class DataModel
    {
        [BsonElement("LinkImg")]
        public string LinkImg { get; set; }

        [BsonElement("LinkVideo")]
        public string LinkVideo { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }
    }
}
