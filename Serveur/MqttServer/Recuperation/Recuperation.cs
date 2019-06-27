// Libs System
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

// Libs MQTTnet
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Exceptions;
using MQTTnet.Protocol;
using MQTTnet.Server;

// Libs Certificates
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

// Libs MongoDB
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

// Libs JSON
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace test
{
    class Program
    {
        static void callbackRpi(IMqttClient mqttClient, String configJson, int state, int versionProtocol1, int versionProtocol2, int typeMessage, int numberMessage, int id, int standby, int timeout, Boolean isAction, List<int> actions){
            Console.WriteLine("INFO: Creating Rpi Callback with state " + state);

            // Creating the Callback Message for the Rpi
            JObject jsonMessage = new JObject();
            jsonMessage.Add("VERSION_PROTOCOL_1", versionProtocol1);
            jsonMessage.Add("VERSION_PROTOCOL_2", versionProtocol2);
            jsonMessage.Add("TYPE_MESSAGE", typeMessage);
            jsonMessage.Add("NUMBER_MESSAGE", numberMessage);
            jsonMessage.Add("ID_1", (id-id%256)/256);
            jsonMessage.Add("ID_2", id%256);
            if(state >= 0) jsonMessage.Add("STATE", state); // If state is below 0, we're not putting any state, which is useful for an id request
            jsonMessage.Add("STANDBY_DELAY_1", (standby-standby%256)/256);
            jsonMessage.Add("STANDBY_DELAY_2", standby%256);
            jsonMessage.Add("TIMEOUT_1", (timeout-timeout%256)/256);
            jsonMessage.Add("TIMEOUT_2", timeout%256);
            jsonMessage.Add("SENDING", 0);

            if(actions.Count == 0 && isAction){ // No actions found
                Console.WriteLine("WARNING: No Actions in the Database for Sensor " + id);
                Console.WriteLine("");
                jsonMessage.Add("PARAMETER_0", 0);
            }else if(actions.Count > 0 && isAction){
                Console.WriteLine("INFO: " + actions.Count + " Actions values found");
                Console.WriteLine("");
                for(int i = 0; i < actions.Count; i++){
                    jsonMessage.Add("PARAMETER_" + i, actions[i]);
                }
            }

            // Sending to Rpi
            Console.WriteLine("###   Sending to Rpi   ###");

            String topic;
            switch(typeMessage){
                case 1:
                    topic = "Server/DemandeID/Rpi";
                    break;
                case 2:
                    topic = "Server/EnvoiInfos/Rpi";
                    break;
                default:
                    topic = "Server/DemandeAction/Rpi";
                    break;
            }

            // Message Building
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Protocol.MongoToMQTT(jsonMessage.ToString(), configJson))
                .WithExactlyOnceQoS()
                .Build();

            // Sending Message
            mqttClient.PublishAsync(message);
        }

        static async Task Main(string[] args)
        {
            // ============ MongoDB ===========

            // Initialising MongoDB Client
            var mongoClient = new MongoClient();
            var database = mongoClient.GetDatabase("MurVegetalDb"); // Database Name

            string configJson;

            // JSON CONFIG RECUPERATION TEMPORE
            using(StreamReader reader = new StreamReader("Config.json")){
                configJson = reader.ReadToEnd();
            }

            // ============= MQTT =============

            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            // VERSION TLS
            
                X509Certificate ca_crt = new X509Certificate("DigiCertCA.crt");
                var tlsOptions = new MqttClientOptionsBuilderTlsParameters();
                tlsOptions.SslProtocol = System.Security.Authentication.SslProtocols.Tls;
                tlsOptions.Certificates = new List<IEnumerable<byte>>() { ca_crt.Export(X509ContentType.Cert).Cast<byte>() };
                tlsOptions.UseTls = true;
                tlsOptions.AllowUntrustedCertificates = true;

                var options = new MqttClientOptionsBuilder()
                .WithTcpServer("10.34.160.10", 8883)
                .WithCredentials("admin", "admin")
                .WithClientId("MQTTServer")
                .WithTls(tlsOptions)
                .Build();
            

            // VERSION NON TLS
            /*
                // Use TCP connection.
                var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) // Port is optional
                .Build();
            */

            // On Disconnect from Server
            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            // On Connect from Server
            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("Rpi/#").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            // Connecting to Server
            await mqttClient.ConnectAsync(options);

            // ========= MESSAGE RECU =========

            // Receiving Messages
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine();
                Console.WriteLine("____________________________________");
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                var messageJson = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                switch(e.ApplicationMessage.Topic){

                    case "Rpi/DemandeID/Server": {
                            Console.WriteLine("###   ID ASKED   ###");

                            Console.WriteLine("### Parsing JSON ###");

                            // Parsing JSON in new format
                            string jsonString = Protocol.MQTTToMongo(messageJson, configJson);
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Sensors");

                            var filter = Builders<BsonDocument>.Filter.Exists("IdSensor"); // Filter documents which contains property "IdSensor"
                            var sort = Builders<BsonDocument>.Sort.Descending("IdSensor"); // Sort documents in descending order by property "IdSensor"
                            
                            var sensorsCount = 0;

                            if(collectionCapteurs.Find(filter).ToList().Count == 0){
                                Console.WriteLine("INFO: No Sensor in DataBase");
                                sensorsCount = 0;
                            }else{
                                var biggestIdDoc = collectionCapteurs.Find(filter).Sort(sort).First(); // Documents with biggest id
                                sensorsCount = biggestIdDoc["IdSensor"].ToInt32() +1;
                            }
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 1){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Bad Message on this queue

                                return; 
                            }else{
                                // Default Values

                                int timeout = 10; // In seconds
                                int standby = 30; // In seconds

                                // Creating the "Capteurs" Collection's value
                                var formatCapteurs = new BsonDocument
                                {
                                    { "IdSensor", sensorsCount },
                                    { "IdSensorType", ( (int) json.Property("COMPONENT_TYPE_1").Value * 256 + (int) json.Property("COMPONENT_TYPE_2").Value) },
                                    { "Project", new BsonArray {} },
                                    { "Name", "" },
                                    { "Description", "" },
                                    { "SensorDate", timestamp },
                                    { "LastSampleDate", 0 },
                                    { "BatteryLevel", new BsonArray {} },
                                    { "Battery", true },
                                    { "SleepTime", 30 },
                                    { "Action", new BsonArray {} },
                                    { "Version", ( (int) json.Property("VERSION_1").Value * 256 + (int) json.Property("VERSION_2").Value) },
                                    { "TimeOut", timeout },
                                    { "IsWorking", true }
                                };

                                // Sending to MongoDB
                                Console.WriteLine("### Sending to MongoDB ###");

                                Console.WriteLine("INFO: Adding Sensor to DataBase");

                                collectionCapteurs.InsertOne(formatCapteurs); 

                                // Creating the Callback Message for the Rpi
                                callbackRpi(mqttClient, configJson, -1, (int) json.Property("VERSION_PROTOCOL_1").Value, (int) json.Property("VERSION_PROTOCOL_2").Value, 1, (int) json.Property("NUMBER_MESSAGE").Value, sensorsCount, standby, timeout, false, new List<int>{});
                            }

                            Console.WriteLine("###      OK      ###");
                        }
                        break;

                    case "Rpi/EnvoiInfos/Server": {
                            Console.WriteLine("### DATA RECEIVE ###");

                            Console.WriteLine("### Parsing JSON ###");

                            // Parsing JSON in new format
                            string jsonString = Protocol.MQTTToMongo(messageJson, configJson);
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Sensors");
                            var collectionReleve = database.GetCollection<BsonDocument>("Samples");
                            var collectionSensorTypes = database.GetCollection<BsonDocument>("SensorTypes");
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 2){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Bad Message on this queue

                                return; 
                            }else{

                                // Get "Sensors" Value from DataBase
                                var id = (int) json.Property("ID_1").Value * 256 + (int) json.Property("ID_2").Value;
                                var filterID = Builders<BsonDocument>.Filter.Eq("IdSensor", id);
                                
                                if(collectionCapteurs.Find(filterID).ToList().Count == 0){
                                    Console.WriteLine("ERROR: Sensor ID not present in the DataBase");
                                    // Error : sensor doesn't exist in the database
                                    callbackRpi(mqttClient, configJson, 2, (int) json.Property("VERSION_PROTOCOL_1").Value, (int) json.Property("VERSION_PROTOCOL_2").Value, 2, (int) json.Property("NUMBER_MESSAGE").Value, id, 30, 10, false, new List<int>{});
                            
                                    return;
                                }else{
                                    var documentCapteur = collectionCapteurs.Find(filterID).First();

                                    // Default Values
                                    var filterStandby = Builders<BsonDocument>.Filter.Exists("SleepTime") & filterID;
                                    int standby = 30;
                                    if(collectionCapteurs.Find(filterStandby).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No SleepTime value for Sensor with ID " + id);
                                    }else{
                                        standby = documentCapteur["SleepTime"].ToInt32();
                                    }

                                    var filterTimeOut = Builders<BsonDocument>.Filter.Exists("TimeOut") & filterID;
                                    int timeout = 10;
                                    if(collectionCapteurs.Find(filterTimeOut).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No TimeOut value for Sensor with ID " + id);
                                    }else{
                                        timeout = documentCapteur["TimeOut"].ToInt32();
                                    }

                                    // Creating the BsonArray of Datas
                                    List<String> dataArray = new List<String>( ((String) json.Property("DATA").Value).Split(',') );

                                    // Getting filter by idSensorType to determine if it exist in the DataBase
                                    var filterIdSensorType = Builders<BsonDocument>.Filter.Exists("IdSensorType") & filterID;
                                    int idsensortype = -1;
                                    if(collectionCapteurs.Find(filterIdSensorType).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No IdSensorType value for Sensor with ID " + id);
                                    }else{
                                        idsensortype = documentCapteur["IdSensorType"].ToInt32();
                                    }
                                    var filterType = Builders<BsonDocument>.Filter.Eq("IdSensorType", idsensortype);
                                    var filterSamplesTypes = Builders<BsonDocument>.Filter.Exists("SamplesTypes") & filterType;

                                    List<BsonDocument> samplesToDB = new List<BsonDocument>(); // Samples to send in the database

                                    // Check if there is the sensor type in the database
                                    if(collectionSensorTypes.Find(filterType).ToList().Count == 0 ||collectionSensorTypes.Find(filterSamplesTypes).ToList().Count == 0){
                                        // Error : type doesn't exist un the database
                                        if(idsensortype != -1) Console.WriteLine("WARNING: IdSensorType " + idsensortype + " not present in the DataBase");
                                        if(idsensortype != -1 && collectionSensorTypes.Find(filterSamplesTypes).ToList().Count == 0) Console.WriteLine("WARNING: No SamplesTypes value for SensorTypes with ID " + idsensortype);

                                        // Creating the Samples Collection's value
                                        for(int i = 0; i < dataArray.Count; i++){
                                            samplesToDB.Add(new BsonDocument
                                            {
                                                { "IdSensor", ( (int) json.Property("ID_1").Value * 256 + (int) json.Property("ID_2").Value) },
                                                { "IdSampleType", "undefined" },
                                                { "Note", "" },
                                                { "SampleDate", timestamp },
                                                { "Value", int.Parse(dataArray[i]) }
                                            });
                                        }

                                    }else{
                                        BsonDocument documentSensorTypes = collectionSensorTypes.Find(filterType).First();

                                        List<BsonValue> samplesTypes = documentSensorTypes["SamplesTypes"].AsBsonArray.ToList();

                                        // Creating the Samples Collection's value
                                        for(int i = 0; i < dataArray.Count; i++){
                                            samplesToDB.Add(new BsonDocument
                                            {
                                                { "IdSensor", ( (int) json.Property("ID_1").Value * 256 + (int) json.Property("ID_2").Value) },
                                                { "IdSampleType", (i >= samplesTypes.Count) ? "undefined" : samplesTypes[i] },
                                                { "Note", "" },
                                                { "SampleDate", timestamp },
                                                { "Value", int.Parse(dataArray[i]) }
                                            });
                                        }
                                    }

                                    // Sending to MongoDB
                                    Console.WriteLine("### Sending to MongoDB ###");

                                    // Updating Sensor
                                    // Check if Battery array exist already, if not creating one
                                    var filterBatteryLevel = Builders<BsonDocument>.Filter.Exists("BatteryLevel") & filterID;
                                    BsonArray batterieNewArray;
                                    if(collectionCapteurs.Find(filterBatteryLevel).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No BatteryLevel value for Sensor with ID " + id);
                                        batterieNewArray = new BsonArray();
                                    }else{
                                        batterieNewArray = documentCapteur["BatteryLevel"].AsBsonArray;
                                    }

                                    batterieNewArray.Add((int) json.Property("BATTERY").Value);

                                    var update1 = Builders<BsonDocument>.Update.Set("LastSampleDate", timestamp);
                                    var update2 = Builders<BsonDocument>.Update.Set("BatteryLevel", batterieNewArray);
                                    var update3 = Builders<BsonDocument>.Update.Set("Battery", ( (int) json.Property("BATTERY").Value > 10 ) ? true : false );
                                    var update4 = Builders<BsonDocument>.Update.Set("IsWorking", true);

                                    collectionCapteurs.UpdateOne(filterID, update1);
                                    collectionCapteurs.UpdateOne(filterID, update2);
                                    collectionCapteurs.UpdateOne(filterID, update3);
                                    collectionCapteurs.UpdateOne(filterID, update4);

                                    // Sending new Data
                                    for(int i = 0; i < samplesToDB.Count; i++){
                                        collectionReleve.InsertOne(samplesToDB[i]); 
                                    }

                                    callbackRpi(mqttClient, configJson, 0, (int) json.Property("VERSION_PROTOCOL_1").Value, (int) json.Property("VERSION_PROTOCOL_2").Value, 2, (int) json.Property("NUMBER_MESSAGE").Value, id, standby, timeout, false, new List<int>{});
                                }
                            }
                            
                            Console.WriteLine("###      OK      ###");
                        }
                        break;

                    case "Rpi/DemandeAction/Server": {
                            Console.WriteLine("### ACTION ASKED ###");

                            Console.WriteLine("### Parsing JSON ###");

                            // Parsing JSON in new format
                            string jsonString = Protocol.MQTTToMongo(messageJson, configJson);
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Sensors");
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 3){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Bad Message on this queue

                                return; 
                            }else{

                                // Get "Sensors" Value from DataBase
                                var id = (int) json.Property("ID_1").Value * 256 + (int) json.Property("ID_2").Value;
                                var filterID = Builders<BsonDocument>.Filter.Eq("IdSensor", id);
                                
                                if(collectionCapteurs.Find(filterID).ToList().Count == 0){
                                    Console.WriteLine("ERROR: Sensor ID not present in the DataBase");
                                    // Error : sensor doesn't exist un the database
                                    callbackRpi(mqttClient, configJson, 2, (int) json.Property("VERSION_PROTOCOL_1").Value, (int) json.Property("VERSION_PROTOCOL_2").Value, 3, (int) json.Property("NUMBER_MESSAGE").Value, id, 10, 30, false, new List<int>{});
                            
                                    return;
                                }else{
                                    var documentCapteur = collectionCapteurs.Find(filterID).First();
                                    
                                    // Default Values
                                    var filterStandby = Builders<BsonDocument>.Filter.Exists("SleepTime") & filterID;
                                    int standby = 30;
                                    if(collectionCapteurs.Find(filterStandby).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No SleepTime value for Sensor with ID " + id);
                                    }else{
                                        standby = documentCapteur["SleepTime"].ToInt32();
                                    }

                                    var filterTimeOut = Builders<BsonDocument>.Filter.Exists("TimeOut") & filterID;
                                    int timeout = 10;
                                    if(collectionCapteurs.Find(filterTimeOut).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No TimeOut value for Sensor with ID " + id);
                                    }else{
                                        timeout = documentCapteur["TimeOut"].ToInt32();
                                    }

                                    // Sending to MongoDB
                                    Console.WriteLine("###   Getting Action   ###");

                                    // Getting Actions
                                    var filterAction = Builders<BsonDocument>.Filter.Exists("Action") & filterID;
                                    BsonValue[] actionArray = {};
                                    if(collectionCapteurs.Find(filterAction).ToList().Count == 0){ // Check Existence of this value in the document
                                        Console.WriteLine("WARNING: No Action value for Sensor with ID " + id);
                                    }else{
                                        actionArray = documentCapteur["Action"].AsBsonArray.ToArray();
                                    }
                                    
                                    List<int> actionList = new List<int>();
                                    for(int i = 0; i < actionArray.Length; i++){
                                        int dataAction = ( (BsonDocument)actionArray[i] )["Data"].ToInt32();
                                                                                
                                        actionList.Add(dataAction);
                                    }

                                    // Creating the Callback Message for the Rpi
                                    callbackRpi(mqttClient, configJson, 0, (int) json.Property("VERSION_PROTOCOL_1").Value, (int) json.Property("VERSION_PROTOCOL_2").Value, 3, (int) json.Property("NUMBER_MESSAGE").Value, id, standby, timeout, true, actionList);
                                }
                            }
                            
                            Console.WriteLine("###      OK      ###");
                        }
                        break;

                    default: {
                            Console.WriteLine("Topic not reccognised, skipping...");
                        }
                        break;

                }

            });

            // Waiting Input to Quit ...
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
