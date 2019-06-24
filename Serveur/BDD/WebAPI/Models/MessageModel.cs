using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class MessageModel
    {
        [BsonElement("TypeMessage")]
        public int TypeMessage { get; set; }

        [BsonElement("Message")]
        public List<PayloadParamModel> PayloadParam { get; set; }
    }
}
