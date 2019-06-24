using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class UsersAPI : ICollectionModel
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("Salt")]
        public string Salt { get; set; }

        [BsonElement("AccreditationLevel")]
        public int AccreditationLevel { get; set; }
    }
}
