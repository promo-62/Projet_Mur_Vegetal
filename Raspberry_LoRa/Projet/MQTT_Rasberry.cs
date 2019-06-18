using System;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Projet
{
    class MQTT_Raspberry
    {
        public static string message = "";
        public static string IP = "localhost"; //"192.168.43.155"
        public static string new_ID_client_server = "d_c_s";
        public static string new_ID_server_client = "d_s_c";
        public static string value_client_server = "e_d_c_s";
        public static string value_server_client = "e_d_s_c";
        public static string action_client_server = "d_I_c_s";
        public static string action_server_client = "d_I_s_c";
        public static int waitingTime = 10; // time in seconds
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("We receive : " + Encoding.UTF8.GetString(e.Message) + "; By " + e.Topic + "\n");
            message = Encoding.UTF8.GetString(e.Message);
            
        }
        public static string RaspberryToServer(string jsonLora)
        {
            /*
            *jsonLora : message from the RaspBerry to the server (serialized JSON)
            */
            message = "";

            dynamic deserialized = JsonConvert.DeserializeObject(jsonLora);
            var typeMessage = deserialized.TYPE_MESSAGE;

            //Connection to the server

            MqttClient client = new MqttClient(IP);

            //choose the right topic from the message "mes"

            string clientId = Guid.NewGuid().ToString();

            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            client.Connect(clientId);
            Console.WriteLine("CONNECT\n");

            if (typeMessage == "01")
            {
                //Inscrition au topic de réponse
                string[] topic = { new_ID_server_client };
                byte[] QoS = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
                client.Subscribe(topic, QoS);

                client.Publish(new_ID_client_server, Encoding.UTF8.GetBytes(jsonLora));
                Console.WriteLine("We send to the server: " + jsonLora + "; " + new_ID_client_server + "\n");
                DateTime deadLine = DateTime.Now.AddSeconds(waitingTime);

                while (message == "" && deadLine.CompareTo(DateTime.Now)>0)
                {
                    //Boucle d'attente de réponse de la part du serveur
                }
            }

            if (typeMessage == "02")
            {
                string[] topic = { value_server_client };
                byte[] QoS = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
                client.Subscribe(topic, QoS);

                client.Publish(value_client_server, Encoding.UTF8.GetBytes(jsonLora));
                Console.WriteLine("We send to the server: " + jsonLora + "; " + value_client_server + "\n");
                DateTime deadLine = DateTime.Now.AddSeconds(waitingTime);

                while (message == "" && deadLine.CompareTo(DateTime.Now) > 0)
                {
                    //Boucle d'attente de réponse de la part du serveur
                }
            }

            if (typeMessage == "03")
            {
                string[] topic = { action_server_client };
                byte[] QoS = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
                client.Subscribe(topic, QoS);

                client.Publish("d_I_c_s", Encoding.UTF8.GetBytes(jsonLora));
                Console.WriteLine("We send to the server: " + jsonLora + "; " + action_client_server + "\n");
                DateTime deadLine = DateTime.Now.AddSeconds(waitingTime);

                while (message == "" && deadLine.CompareTo(DateTime.Now) > 0)
                {
                    //Boucle d'attente de réponse de la part du serveur
                }
            }
            
            client.Disconnect();
            Console.WriteLine("DISCONNECT\n");

            return message;
        }
    }
}
