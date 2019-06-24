using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class Sensors : ICollectionModel
    {
        [BsonElement("IdSensor")]
        public int IdSensor { get; set; }

        [BsonElement("IdSensorType")]
        public int IdSensorType { get; set; }

        [BsonElement("Project")]
        public List<string> Project { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("SensorDate")]
        public long SensorDate { get; set; }

        [BsonElement("LastSampleDate")]
        public long LastSampleDate { get; set; }
        
        [BsonElement("BatteryLevel")]
        public List<int> BatteryLevel { get; set; }

        [BsonElement("Battery")]
        public bool Battery { get; set; }

        [BsonElement("SleepTime")]
        public int SleepTime { get; set; }

        [BsonElement("Action")]
        public List<ActionModel> Action { get; set; }

        [BsonElement("Version")]
        public int Version { get; set; }

        [BsonElement("TimeOut")]
        public int TimeOut { get; set; }

        [BsonElement("IsWorking")]
        public bool IsWorking { get; set; }
    }

}