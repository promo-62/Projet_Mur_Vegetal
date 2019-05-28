﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.IO;
using System.Threading;

namespace Projet
{
    class Program
    {
        //Chemin vers le fichier config.json (Formats des trames)
        private static string filepath_1 ="../aethonn/Documents/Projet_Mur_Vegetal/Raspberry_LoRa/Projet/config.json";
        //Chemin vers le  fichier payload_sizes.json (Association des versions de protocoles à des tailles de payload)
        private static string filepath_2 ="../aethonn/Documents/Projet_Mur_Vegetal/Raspberry_LoRa/Projet/payload_sizes.json";

        private static string Config;
        private static string PayloadSizes;
        public static void Main(string[] args){
            //Mise en place de la comunication entre C++ et C#
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener server = new TcpListener(ip, 15200);
            TcpClient client = default(TcpClient);

            //Lancement de la comunication entre les 2 programmes (LoRA et Protocol)
            try{
                server.Start();
                Console.WriteLine("SERVER: STARTED");
                Console.WriteLine("");
                using(StreamReader reader = new StreamReader(filepath_1)){
                    Config = reader.ReadToEnd();
                }
                using(StreamReader reader = new StreamReader(filepath_2)){
                    PayloadSizes = reader.ReadToEnd();
                }
            }catch(Exception e){
                Console.WriteLine(e.ToString());
                Console.Read();
            }

            Thread Update = new Thread(UpdateConfig);
            Update.Start();

            string test;
            client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 3600000;
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
                    Console.WriteLine(e.StackTrace);
                    System.Environment.Exit(0);
                }
                //Convertion du message recu en BYTE[] (Raw Data)
                test = BitConverter.ToString(buffer);
                Console.WriteLine("MESSAGE RECEIVED: "+test);
                Console.WriteLine("");

                //Envoi du message dans la section traitement du Protocol.cs et attente d'une reponse du serveur
                //Cette section va verifier puis convertir le message en JSON
                string response = Protocol.DataToJson(buffer, Config, PayloadSizes);

                //Verification si le message etait bien formate
                if( !(response.Equals("")) ){

                    //Convertion du JSON en BYTE[]
                    byte[] Raw_Data = Protocol.JsonToData(response);
                    
                    //Envoi de la reponse au LoRA
                    try{
                        stream.Write(Raw_Data, 0, Raw_Data.Length);
                    }catch(Exception e){
                        Console.WriteLine(e.StackTrace);
                    }
                    test = BitConverter.ToString(Raw_Data);
                    Console.WriteLine("MESSAGE SEND: "+test);
                    Console.WriteLine("");
                }else{
                    //Envoi des erreurs
                    byte[] Errors = Protocol.ErrorsFound();
                    try{
                        stream.Write(Errors, 0, Errors.Length);
                    }catch(Exception e){
                        Console.WriteLine(e.StackTrace);
                        System.Environment.Exit(0);
                    }
                    test = BitConverter.ToString(Errors);
                    if(test.Equals("00")){
                        Console.WriteLine("MESSAGE SEND: NO RESPONSE FROM DATABASE");
                        Console.WriteLine("");
                    }else if(test.Equals("01")){
                        Console.WriteLine("MESSAGE SEND: INVALID FORMAT");
                        Console.WriteLine("");
                    }else{
                        Console.WriteLine("MESSAGE SEND: INVALID PAYLOAD SIZE");
                        Console.WriteLine("");
                    }
                }
            }
            Console.ReadKey();

        }

        //Mise a jour des fichier json de config
        private static void UpdateConfig(){
            while(true){
                //A remplacer par un wget
                try{
                    //var client = new WebClient();
                    //Config = client.DownloadString(filepath_1);
                    //PayloadSizes = client.DownloadString(filepath_2);
                    using(StreamReader reader = new StreamReader(filepath_1)){
                        Config = reader.ReadToEnd();
                    }
                    using(StreamReader reader = new StreamReader(filepath_2)){
                        PayloadSizes = reader.ReadToEnd();
                    }
                }catch(Exception e){
                    Console.WriteLine(e.StackTrace);
                    System.Environment.Exit(1);
                }
                Thread.Sleep(600);
            }
        }

    }
}