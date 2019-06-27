using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Protocol_MQTTClient
{
    class Program
    {
        //Chemin vers le fichier config.json (Formats des trames)
        private static string filepath_1 = "Config.json";

        private static string Config;
        public static void Main(string[] args){
            Test();
            while(true){

            }
        }

        static async Task Test(){
            //Mise en place de la comunication entre C++ et C#
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[1];
            TcpListener server = new TcpListener(ip, 15200);
            TcpClient client = default(TcpClient);

            //Lancement de la comunication entre les 2 programmes (LoRA et Protocol)
            try{
                server.Start();
                var client2 = new WebClient();
                Config = client2.DownloadString(filepath_1);
                Console.WriteLine("ADDRESS: "+ip);
                Console.WriteLine("SERVER: STARTED");
                Console.WriteLine("");
            }catch(Exception e){
                Console.WriteLine(e.ToString());
                System.Environment.Exit(1);
            }

            Thread Update = new Thread(UpdateConfig);
            Update.Start();

            string test;
            client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            //stream.ReadTimeout = 3600000;
            stream.WriteTimeout = 10000;
            //Boucle infini
            while(true){
                
                //Attend un message du LoRA
                Console.WriteLine("WAITING FOR MESSAGE");
                Console.WriteLine("");
                byte[] buffer = new byte[100];
                try{
                    stream.Read(buffer, 0, buffer.Length);
                }catch(Exception e){
                    Console.WriteLine("CONNECTION LOST WITH LORA");
                    client = server.AcceptTcpClient();
                    stream = client.GetStream();
                    stream.Read(buffer, 0, buffer.Length);
                }
                //Convertion du message recu en BYTE[] (Raw Data)
                test = BitConverter.ToString(buffer);
                Console.WriteLine("MESSAGE RECEIVED: "+test);
                Console.WriteLine("");

                //Envoi du message dans la section traitement du Protocol.cs et attente d'une reponse du serveur
                //Cette section va verifier puis convertir le message en JSON
                string response = await Protocol.DataToJson(buffer, Config);

                //Verification si le message etait bien formate
                if( !(response.Equals("")) ){

                    //Convertion du JSON en BYTE[]
                    byte[] Raw_Data = Protocol.JsonToData(response, Config);

                    //Envoi de la reponse au LoRA
                    try{
                        stream.Write(Raw_Data, 0, Raw_Data.Length);
                    }catch(Exception e){
                        Console.WriteLine("CONNECTION LOST WITH LORA");
                        client = server.AcceptTcpClient();
                        stream = client.GetStream();
                    }
                    test = BitConverter.ToString( Raw_Data);
                    Console.WriteLine("MESSAGE SEND: "+test);
                    Console.WriteLine("");
                }else{
                    //Envoi des erreurs
                    byte[] Errors = Protocol.ErrorsFound();
                    try{
                        stream.Write(Errors, 0, Errors.Length);
                    }catch(Exception e){
                        Console.WriteLine("CONNECTION LOST WITH LORA");
                        client = server.AcceptTcpClient();
                        stream = client.GetStream();
                    }
                    test = BitConverter.ToString(Errors);
                    if(Errors[2].Equals(0x01)){
                        Console.WriteLine("MESSAGE SEND: NO RESPONSE FROM DATABASE");
                        Console.WriteLine("");
                    }else if(Errors[2].Equals(0x02)){
                        Console.WriteLine("MESSAGE SEND: INVALID FORMAT");
                        Console.WriteLine("");
                    }else if(Errors[2].Equals(0x03)){
                        Console.WriteLine("MESSAGE SEND: INVALID PAYLOAD SIZE");
                        Console.WriteLine("");
                    }
                    Console.WriteLine("ERROR SEND: "+test);
                    Console.WriteLine("");
                }
            }
        }

        //Mise a jour des fichier json de config
        private static void UpdateConfig(){
            while(true){
                //A remplacer par un wget
                try{
                    var client = new WebClient();
                    Config = client.DownloadString(filepath_1);
                }catch(Exception e){
                    Console.WriteLine(e.StackTrace);
                    System.Environment.Exit(1);
                }
                Thread.Sleep(600);
            }
        }

    }
}