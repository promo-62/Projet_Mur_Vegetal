
﻿using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Exceptions;
using MQTTnet.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json;
using System;

namespace Projet
{
    class MQTT_Raspberry
    {
        public static string IP = "10.34.160.10";
        public static int Port = 8883;
        public static string new_ID_client_server = "Rpi/DemandeID/Server";
        public static string new_ID_server_client = "Server/DemandeID/Rpi";
        public static string value_client_server = "Rpi/EnvoiInfos/Server";
        public static string value_server_client = "Server/EnvoiInfos/Rpi";
        public static string action_client_server = "Rpi/DemandeAction/Server";
        public static string action_server_client = "Server/DemandeAction/Rpi";
        public static int waitingTime = 10; // time in seconds
        public static string message2 = "";


        public static void TempoMessage()
        {
            //sous format de boucle
            int waitingTime = 10; // time in seconds
            DateTime deadLine = DateTime.Now.AddSeconds(waitingTime);
            while (message2 == "" && deadLine.CompareTo(DateTime.Now) > 0)
            {
                Thread.Sleep(100);//attente de 0.1 sec
            }

            //à avoir pour utiliser la fonction ci dessus

            //renvoir True si le temps est passé ou si le message à était reçu
        }
        
        public static async Task<string> RaspberryToServer (string jsonLora)
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            X509Certificate ca_crt = new X509Certificate("/home/pi/Login/DigiCertCA.crt");
            var tlsOptions = new MqttClientOptionsBuilderTlsParameters();
            tlsOptions.SslProtocol = System.Security.Authentication.SslProtocols.Tls;
            tlsOptions.Certificates = new List<IEnumerable<byte>>() { ca_crt.Export(X509ContentType.Cert).Cast<byte>() };
            tlsOptions.UseTls = true;
            tlsOptions.AllowUntrustedCertificates = true;
            //tlsOptions.IgnoreCertificateChainErrors = false;
            //tlsOptions.IgnoreCertificateRevocationErrors = false;
            

            MqttNetGlobalLogger.LogMessagePublished += (sender, e) =>
            {
                Console.WriteLine(e.TraceMessage.Message);
            };

            // Use TCP connection.
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer(IP, Port) // Port is optional
            .WithCredentials("admin", "admin")
            .WithClientId("test")
            .WithTls(tlsOptions)
            .Build();

            // quoi faire des msg qui arrivent
            client.UseApplicationMessageReceivedHandler(e =>
            {
                message2 = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            });

            // on se connecte vraiment au server
            await client.ConnectAsync(options);

            // on créer un msg

            dynamic deserialized = JsonConvert.DeserializeObject(jsonLora);
            var typeMessage = deserialized.TYPE_MESSAGE;

            if (typeMessage == "01")
            {
                client.UseConnectedHandler(async e =>
                {

                    // Subscribe to a topic
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(new_ID_client_server).Build());


                });
               
                


                
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(new_ID_server_client).Build());
                // on créer un msg
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(new_ID_client_server)
                .WithPayload(jsonLora)
                .WithExactlyOnceQoS()
                .Build();

                await client.PublishAsync(message);

                TempoMessage();
            }

            if (typeMessage == "02")
            {
                client.UseConnectedHandler(async e =>

                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                    // Subscribe to a topic
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(value_client_server).Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });

                

                
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(value_server_client).Build());
                // on créer un msg
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(value_client_server)
                .WithPayload(jsonLora)
                .WithExactlyOnceQoS()
                .Build();

                await client.PublishAsync(message);
                TempoMessage();
            }

            if (typeMessage == "03")
            {
                client.UseConnectedHandler(async e =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                    // Subscribe to a topic
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(action_server_client).Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });

                // gerer les deconnexions

                
                
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(action_client_server).Build());
                // on créer un msg
                var message = new MqttApplicationMessageBuilder()
                .WithTopic(action_client_server)
                .WithPayload(jsonLora)
                .WithExactlyOnceQoS()
                .Build();

                await client.PublishAsync(message);

                TempoMessage();
            }

            
            await client.DisconnectAsync();
            Console.WriteLine (JsonConvert.DeserializeObject(message2));

            String sentMessage = message2;
            message2 = "";
            return sentMessage;
        }

       
    }
}

