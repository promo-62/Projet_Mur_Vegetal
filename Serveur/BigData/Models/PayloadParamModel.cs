using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class PayloadParamModel
    {
        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Size")]
        public int Size { get; set; }
    }
}
