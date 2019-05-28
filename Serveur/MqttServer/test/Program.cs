using System;
using System.Text;
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
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            // Use TCP connection.
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer("192.168.43.11", 5672) // Port is optional
            .Build();

            // gerer les deconnexions
            client.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await client.ConnectAsync(options);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });


            // on se connecte vraiment au server
            await client.ConnectAsync(options);

            // quoi faire des msg qui arrivent
            client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            // se connecter au topic test01
            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("test01").Build());

                Console.WriteLine("### SUBSCRIBED ###");
            });

            // on créer un msg 
            var message = new MqttApplicationMessageBuilder()
            .WithTopic("test01")
            .WithPayload("Hello World")
            .WithExactlyOnceQoS()
            .Build();

            // on envoie un msg
            await client.PublishAsync(message);

        }
    }
}
