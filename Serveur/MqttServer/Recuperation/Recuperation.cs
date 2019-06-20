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
        static async Task Main(string[] args)
        {
            // ============ MongoDB ===========

            // Initialising MongoDB Client
            var mongoClient = new MongoClient();
            var database = mongoClient.GetDatabase("ISENption"); // Database Name

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
                .WithClientId("gbb")
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
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Capteurs");

                            var filter = Builders<BsonDocument>.Filter.Exists("idCapteur"); // Filter documents which contains property "idCapteur"
                            var sort = Builders<BsonDocument>.Sort.Descending("idCapteur"); // Sort documents in descending order by property "idCapteur"

                            var biggestIdDoc = collectionCapteurs.Find(filter).Sort(sort).First(); // Documents with biggest id
                            var sensorsCount = biggestIdDoc["idCapteur"].ToInt32() +1;
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 1){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Mauvais message, on renvoit une erreur

                                return; 
                            }else{
                                // Default Values

                                int timeout = 10; // In seconds
                                int standby = 30; // In seconds

                                // Creating the "Capteurs" Collection's value
                                var formatCapteurs = new BsonDocument
                                {
                                    { "idCapteur", sensorsCount },
                                    { "typeCapteur", ( (int) json.Property("COMPONENT_TYPE_1").Value + (int) json.Property("COMPONENT_TYPE_2").Value * 255) },
                                    { "projet", new BsonArray {} },
                                    { "nom", "" },
                                    { "description", "" },
                                    { "dateCapteur", timestamp },
                                    { "dateDernierReleve", 0 },
                                    { "niveauBatterie", new BsonArray {} },
                                    { "batterie", true },
                                    { "delaiVeille", 30 },
                                    { "Action", new BsonArray {} },
                                    { "version", ( (int) json.Property("VERSION_1").Value + (int) json.Property("VERSION_2").Value * 255) },
                                    { "fonctionne", true },
                                    { "timeout", timeout }
                                };

                                // Sending to MongoDB
                                Console.WriteLine("### Sending to MongoDB ###");

                                collectionCapteurs.InsertOne(formatCapteurs); 

                                // Creating the Callback Message for the Rpi
                                JObject jsonMessage = new JObject();
                                jsonMessage.Add("VERSION_PROTOCOL_1", json.Property("VERSION_PROTOCOL_1").Value);
                                jsonMessage.Add("VERSION_PROTOCOL_2", json.Property("VERSION_PROTOCOL_2").Value);
                                jsonMessage.Add("TYPE_MESSAGE", "01");
                                jsonMessage.Add("NUMBER_MESSAGE", json.Property("NUMBER_MESSAGE").Value);
                                jsonMessage.Add("ID_1", sensorsCount%256);
                                jsonMessage.Add("ID_2", (sensorsCount-sensorsCount%256)/256);
                                jsonMessage.Add("STANDBY_DELAY_1", standby%256);
                                jsonMessage.Add("STANDBY_DELAY_2", (standby-standby%256)/256);
                                jsonMessage.Add("TIMEOUT_1", timeout%256);
                                jsonMessage.Add("TIMEOUT_2", (timeout-timeout%256)/256);
                                jsonMessage.Add("SENDING", 0);

                                // Sending to Rpi
                                Console.WriteLine("###   Sending to Rpi   ###");

                                // Message Building
                                var message = new MqttApplicationMessageBuilder()
                                    .WithTopic("Server/DemandeID/Rpi")
                                    .WithPayload(Protocol.MongoToMQTT(jsonMessage.ToString(), configJson))
                                    .WithExactlyOnceQoS()
                                    .Build();

                                // Sending Message
                                mqttClient.PublishAsync(message);
                            }

                            Console.WriteLine("###      OK      ###");
                        }
                        break;

                    case "Rpi/EnvoiInfos/Server": {
                            Console.WriteLine("### DATA RECEIVE ###");

                            Console.WriteLine("### Parsing JSON ###");

                            // Parsing JSON in new format
                            string jsonString = Protocol.MQTTToMongo(messageJson, configJson);
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Capteurs");
                            var collectionReleve = database.GetCollection<BsonDocument>("Releve");
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 2){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Mauvais message, on renvoit une erreur

                                return; 
                            }else{

                                // Get "Capteurs" Value from DataBase
                                var id = (int) json.Property("ID_1").Value + (int) json.Property("ID_2").Value * 255;
                                var filterID = Builders<BsonDocument>.Filter.Eq("idCapteur", id);
                                
                                if(collectionCapteurs.Find(filterID).ToList().Count == 0){
                                    Console.WriteLine("ERROR: Sensor ID not present in the DataBase");
                                    // Error : sensor doesn't exist un the database

                                        /*
                                        _____________________________
                                        ERROR NOT GENERATED UNTIL NOW

                                        */
                                    return;
                                }else{
                                    var documentCapteur = collectionCapteurs.Find(filterID).First();

                                    // Default Values
                                    int standby = documentCapteur["delaiVeille"].ToInt32();
                                    int timeout = documentCapteur["timeout"].ToInt32(); // In seconds

                                    // Creating the BsonArray of Datas
                                    List<String> strArray = new List<String>( ((String) json.Property("DATA").Value).Split(',') );
                                    BsonArray dataArray = new BsonArray {};
                                    for(int i = 0; i < strArray.Count; i++){
                                        dataArray.Add(int.Parse(strArray[i]));
                                    }
                                    // Creating the "Releve" Collection's value
                                    var formatReleve = new BsonDocument
                                    {
                                        { "idCapteur", ( (int) json.Property("ID_1").Value + (int) json.Property("ID_2").Value * 255) },
                                        { "note", "" },
                                        { "dateReleves", timestamp },
                                        { "valeur", dataArray },
                                        { "typeCapteur", documentCapteur["typeCapteur"] },
                                        { "fiabilite", ""}
                                    };

                                    // Sending to MongoDB
                                    Console.WriteLine("### Sending to MongoDB ###");

                                    // Updating Sensor
                                    BsonArray batterieNewArray = documentCapteur["niveauBatterie"].AsBsonArray;
                                    batterieNewArray.Add((int) json.Property("BATTERY").Value);

                                    var update1 = Builders<BsonDocument>.Update.Set("dateDernierReleve", timestamp);
                                    var update2 = Builders<BsonDocument>.Update.Set("niveauBatterie", batterieNewArray);
                                    var update3 = Builders<BsonDocument>.Update.Set("batterie", ( (int) json.Property("BATTERY").Value > 10 ) ? true : false );
                                    var update4 = Builders<BsonDocument>.Update.Set("fonctionne", true);

                                    collectionCapteurs.UpdateOne(filterID, update1);
                                    collectionCapteurs.UpdateOne(filterID, update2);
                                    collectionCapteurs.UpdateOne(filterID, update3);
                                    collectionCapteurs.UpdateOne(filterID, update4);

                                    // Sending new Data
                                    collectionReleve.InsertOne(formatReleve); 

                                    // Creating the Callback Message for the Rpi
                                    JObject jsonMessage = new JObject();
                                    jsonMessage.Add("VERSION_PROTOCOL_1", json.Property("VERSION_PROTOCOL_1").Value);
                                    jsonMessage.Add("VERSION_PROTOCOL_2", json.Property("VERSION_PROTOCOL_2").Value);
                                    jsonMessage.Add("TYPE_MESSAGE", "02");
                                    jsonMessage.Add("NUMBER_MESSAGE", json.Property("NUMBER_MESSAGE").Value);
                                    jsonMessage.Add("ID_1", json.Property("ID_1").Value);
                                    jsonMessage.Add("ID_2", json.Property("ID_2").Value);
                                    jsonMessage.Add("STATE", 0);
                                    jsonMessage.Add("STANDBY_DELAY_1", standby%256);
                                    jsonMessage.Add("STANDBY_DELAY_2", (standby-standby%256)/256);
                                    jsonMessage.Add("TIMEOUT_1", timeout%256);
                                    jsonMessage.Add("TIMEOUT_2", (timeout-timeout%256)/256);
                                    jsonMessage.Add("SENDING", 0);

                                    // Sending to Rpi
                                    Console.WriteLine("###   Sending to Rpi   ###");

                                    // Message Building
                                    var message = new MqttApplicationMessageBuilder()
                                        .WithTopic("Server/EnvoiInfos/Rpi")
                                        .WithPayload(Protocol.MongoToMQTT(jsonMessage.ToString(), configJson))
                                        .WithExactlyOnceQoS()
                                        .Build();

                                    // Sending Message
                                    mqttClient.PublishAsync(message);
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
                            
                            var collectionCapteurs = database.GetCollection<BsonDocument>("Capteurs");
                            
                            JObject json = JObject.Parse(jsonString);
                    
                            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            if((int) json.Property("TYPE_MESSAGE").Value != 3){
                                Console.WriteLine("ERROR: BAD TYPE_MESSAGE ON QUEUE");
                                // Mauvais message, on renvoit une erreur

                                return; 
                            }else{

                                // Get "Capteurs" Value from DataBase
                                var id = (int) json.Property("ID_1").Value + (int) json.Property("ID_2").Value * 255;
                                var filterID = Builders<BsonDocument>.Filter.Eq("idCapteur", id);
                                
                                if(collectionCapteurs.Find(filterID).ToList().Count == 0){
                                    Console.WriteLine("ERROR: Sensor ID not present in the DataBase");
                                    // Error : sensor doesn't exist un the database

                                        /*
                                        _____________________________
                                        ERROR NOT GENERATED UNTIL NOW

                                        */
                                    return;
                                }else{
                                    var documentCapteur = collectionCapteurs.Find(filterID).First();

                                    // Default Values
                                    int standby = documentCapteur["delaiVeille"].ToInt32();
                                    int timeout = documentCapteur["timeout"].ToInt32(); // In seconds

                                    // Sending to MongoDB
                                    Console.WriteLine("###   Getting Action   ###");

                                    // Getting Actions
                                    var actionArray = documentCapteur["Action"].AsBsonArray.ToArray();

                                    // Creating the Callback Message for the Rpi
                                    JObject jsonMessage = new JObject();
                                    jsonMessage.Add("VERSION_PROTOCOL_1", json.Property("VERSION_PROTOCOL_1").Value);
                                    jsonMessage.Add("VERSION_PROTOCOL_2", json.Property("VERSION_PROTOCOL_2").Value);
                                    jsonMessage.Add("TYPE_MESSAGE", "03");
                                    jsonMessage.Add("NUMBER_MESSAGE", json.Property("NUMBER_MESSAGE").Value);
                                    jsonMessage.Add("ID_1", json.Property("ID_1").Value);
                                    jsonMessage.Add("ID_2", json.Property("ID_2").Value);
                                    jsonMessage.Add("STATE", 0);
                                    jsonMessage.Add("STANDBY_DELAY_1", standby%256);
                                    jsonMessage.Add("STANDBY_DELAY_2", (standby-standby%256)/256);
                                    jsonMessage.Add("TIMEOUT_1", timeout%256);
                                    jsonMessage.Add("TIMEOUT_2", (timeout-timeout%256)/256);
                                    jsonMessage.Add("SENDING", 0);

                                    if(actionArray.Length == 0){ // No actions found
                                        Console.WriteLine("WARNING: No Actions in the Database for Sensor " + id);
                                        Console.WriteLine("");
                                    }else{
                                        Console.WriteLine("INFO: " + actionArray.Length + " Actions values found");
                                        Console.WriteLine("");
                                        for(int i = 0; i < actionArray.Length; i++){
                                            jsonMessage.Add("PARAMETER_" + i, ( (BsonDocument)actionArray[i] )["data"].ToInt32());
                                        }
                                    }

                                    // Sending to Rpi
                                    Console.WriteLine("###   Sending to Rpi   ###");

                                    // Message Building
                                    var message = new MqttApplicationMessageBuilder()
                                        .WithTopic("Server/DemandeAction/Rpi")
                                        .WithPayload(Protocol.MongoToMQTT(jsonMessage.ToString(), configJson))
                                        .WithExactlyOnceQoS()
                                        .Build();

                                    // Sending Message
                                    mqttClient.PublishAsync(message);
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
