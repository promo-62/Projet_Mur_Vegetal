using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace Projet
{
    class MQTT_Raspberry
    {
        public static string messageServer = "";

        static void Appel(string[] args)
        {
            Console.WriteLine("Here is the Raspberry!");
            Console.WriteLine("Enter a message for the server: ");
            string mes = Console.ReadLine();
            string test = RaspberryToServer(mes);
            //Console.WriteLine("\n\n\n Tu m'as oublier crétin" + test);
        }

        public static string RaspberryToServer(string mes)
        {
            /*
             * string mes : message du Raspberry pour le serveur (JSON serialized)
             */
            string ans = "";

            dynamic deserialized = JsonConvert.DeserializeObject(mes);
            var typeMessage = deserialized.type;

            var factory = new ConnectionFactory() { HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                if (typeMessage == "01")
                {
                    //Console.WriteLine("Par ici!");
                    channel.QueueDeclare(queue: "decouverte_client_serveur",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(mes);

                    channel.BasicPublish(exchange: "",
                        routingKey: "decouverte_client_serveur",
                        basicProperties: null,
                        body: body);

                    Console.WriteLine("We send to the server: " + mes + " ; By AskID");

                    channel.QueueDeclare(queue: "decouverte_serveur_client",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    while (messageServer == "") {
                        consumer.Received += (model, ea) =>
                        {
                            var rep = ea.Body;
                            Program.messageServer = Encoding.UTF8.GetString(rep);
                        };
                        channel.BasicConsume(queue: "decouverte_serveur_client",
                                             autoAck: true,
                                             consumer: consumer);
                    }
                    Console.WriteLine("We receive from the server: " + messageServer + " ; By AnsID");
                    ans = messageServer;
                    messageServer = "";
                }

                if (typeMessage == "02") {
                    //Console.WriteLine("Par là");
                    channel.QueueDeclare(queue: "envoiInfoClientServeur",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body1 = Encoding.UTF8.GetBytes(mes);

                    channel.BasicPublish(exchange: "",
                        routingKey: "envoiInfoClientServeur",
                        basicProperties: null,
                        body: body1);

                    Console.WriteLine("We send to the server: " + mes + " ; By DeliverValues");

                    channel.QueueDeclare(queue: "envoiInfoServeurClient",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer1 = new EventingBasicConsumer(channel);
                    while (messageServer == "") {
                        consumer1.Received += (model, ea) =>
                        {
                            var rep = ea.Body;
                            Program.messageServer = Encoding.UTF8.GetString(rep);
                        };
                        channel.BasicConsume(queue: "envoiInfoServeurClient",
                                             autoAck: true,
                                             consumer: consumer1);
                    }
                    Console.WriteLine("We receive from the server: " + messageServer + " ; By AnsValue");
                    ans = messageServer;
                    messageServer = "";
                }

                if (typeMessage == "03")
                {
                    channel.QueueDeclare(queue: "demandeActionClientServeur",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body2 = Encoding.UTF8.GetBytes(mes);

                    channel.BasicPublish(exchange: "",
                        routingKey: "demandeActionClientServeur",
                        basicProperties: null,
                        body: body2);

                    Console.WriteLine("We send to the server: " + mes + " ; By AskAction");

                    channel.QueueDeclare(queue: "demandeActionServeurClient",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer2 = new EventingBasicConsumer(channel);
                    while (messageServer == "")
                    {
                        consumer2.Received += (model, ea) =>
                        {
                            var rep = ea.Body;
                            Program.messageServer = Encoding.UTF8.GetString(rep);
                        };
                        channel.BasicConsume(queue: "demandeActionServeurClient",
                                             autoAck: true,
                                             consumer: consumer2);
                    }
                    Console.WriteLine("We receive from the server: " + messageServer + " ; By AnsAction");
                    ans = messageServer;
                    messageServer = "";
                }

                return ans;
            }
        }
    }
}
