using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class ProtocolVersions : ICollectionModel
    {
        [BsonElement("Version")]
        public int Version { get; set; }

        [BsonElement("Message")]
        public List<MessageModel> Message { get; set; }
    }
}
