using System;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace Projet{
    public class Protocol{

        //Nom du tableau contenant tout les formats de header
        private static string Array_Formats_PropertyName = "Header";
        //Nom du tableau contenant tout les versions de protocole associe a la taille maximale de leur payload
        private static string Array_PayloadSizes_PropertyName = "Sizes";
        //Nom de la propriete contenant la taille maximale du payload
        private static string Payload_Size_PropertyName = "SIZE";
        //Nom de la premiere propriete de la version du protocole
        private static string Header_VerProtocol_1_PropertyName = "VERSION_PROTOCOL_1";
        //Nom de la seconde propriete de la version du protocole
        private static string Header_VerProtocol_2_PropertyName = "VERSION_PROTOCOL_2";
        //Nom de la propriete contenant le payload 
        private static string Header_Payload_PropertyName = "PAYLOAD";
        //Nom de la propriete contenant l'ID 1
        private static string Header_ID1_PropertyName = "ID_1";
        //Nom de la propriete contenant l'ID 2
        private static string Header_ID2_PropertyName = "ID_2";

        private static string Payload_Format = "Format";
        private static string ID_PropertyName = "ID";

        private static string Id = "";
        private static int Header_Size = 6;
        private static string Array_Payload_Format = "Payload";
        private static string Header_Type_PropertyName = "TYPE_MESSAGE";

        private static byte Errors;
        
        //Verifie le format des trames arrivant du LoRA et les convertit en fichier JSON
        public static string DataToJson(byte[] chain,string file1){
            //Ouverture des fichiers: config.json et payload_sizes.json
            JObject obj_config = JObject.Parse(file1);
            //Conversion de la chaine d'octet en chaine de string
            string hex = BitConverter.ToString(chain);
            String[] header = hex.Split('-');
            //Selection du tableau des formats Header
            JArray arr = (JArray)obj_config[Array_Formats_PropertyName]; //CHANGING
            bool isValidated = true;
            int k = 0;
            //Initialisation de la variable d'erreurs de format
            Errors = 0x00;

            //Verifie si le header de la trame correspond a un format repertorie dans le tableau 
            foreach(JObject obj in arr.Children<JObject>()){
                k = 0;

                //Compare chaque valeur du header de la trame au valeur de chaque format de header
                foreach(JProperty property in obj.Properties()){
                    string value = (string)property.Value;
                    if( !(value.Equals(header[k])) && !(value.Equals("")) ){
                        isValidated = false;
                    }
                    k++;
                }

                //Convertit la trame en JSON si son header correspond a un format
                if(isValidated == true){

                    //Cherche la valeur de la taille de la payload en fonction de la version du protocole de la trame
                    JArray sizes = (JArray)obj_config[Array_PayloadSizes_PropertyName];
                    int size = 0;
                    foreach(JObject obj_size in sizes.Children<JObject>()){
                        if( ((string)obj.Property(Header_VerProtocol_1_PropertyName).Value).Equals((string)obj_size.Property(Header_VerProtocol_1_PropertyName).Value) && ((string)obj.Property(Header_VerProtocol_2_PropertyName).Value).Equals((string)obj_size.Property(Header_VerProtocol_2_PropertyName).Value) ){
                            size = (int)obj_size.Property(Payload_Size_PropertyName).Value;
                            break;
                        }
                    }

                    for(int i = obj.Count+size; i < chain.Length; i++){
                        if( !(chain[i].Equals(0x00)) ){
                            Errors = 0x02;
                            return "";
                        }
                    }

                    Console.WriteLine("MESSAGE: VALID FORMAT AND PAYLOAD SIZE");
                    Console.WriteLine("");

                    //Insertion des valeurs du payload et des valeur non remplie dans le JSON
                    int z =0;
                    foreach(JProperty property in obj.Properties()){
                        string value = (string)property.Value;
                        if((value.Equals("")) ){
                            obj.Property(property.Name).Value = header[z];
                        }
                    z++;
                    }

                    byte[] payload = new byte[size];
                    for(int i = k; i < payload.Length+k; i++){
                        payload[i-k] = chain[i]; 
                    }
                    string raw_data = BitConverter.ToString(payload);
                    obj.Add(Header_Payload_PropertyName, raw_data);
                    obj.Remove(Header_ID1_PropertyName);
                    obj.Remove(Header_ID2_PropertyName);
                    Console.WriteLine("JSON CREATED: "+obj.ToString());
                    Console.WriteLine("");

                    //Renvoie du JSON en string
                    string response = MQTT_Raspberry.RaspberryToServer(obj.ToString());
                    //string response = "";

                    //Verifie si le serveur repond et si il ne repond pas parse la payload et stocke l'ID 
                    if(response.Equals("")){
                        JArray tableau = (JArray)obj_config[Array_Payload_Format];
                        foreach(JObject obj_id in tableau.Children<JObject>()){
                            if ( (string)(obj.Property(Header_VerProtocol_1_PropertyName).Value) == (string)(obj_id.Property(Header_VerProtocol_1_PropertyName).Value) && (string)(obj.Property(Header_VerProtocol_2_PropertyName).Value) == (string)(obj_id.Property(Header_VerProtocol_2_PropertyName).Value) && (string)(obj.Property(Header_Type_PropertyName).Value) == (string)(obj_id.Property(Header_Type_PropertyName).Value) ){
                                if( !((string)(obj.Property(Header_Type_PropertyName))).Equals("01") ){
                                    JObject format = (JObject)(obj_id.GetValue(Payload_Format));
                                    int w = 0;
                                    byte[] rev = new byte[1];
                                    foreach(JProperty property in format.Properties()){
                                        if(property.Name == ID_PropertyName){
                                            rev[0] = chain[w+Header_Size];
                                            Id = BitConverter.ToString(rev);
                                        }
                                        w++;
                                    }
                                }
                            }
                        }
                    }
                    return response;
                }else{
                    isValidated = true;
                }
            }

            //Format invalide
            Errors = 0x01;
            return "";
        }

        //Convertit la reponse JSON du serveur en BYTE[] pour les capteurs
        public static byte[] JsonToData(string json){

            //Convertit la reponse string en JSON
            JObject obj = JObject.Parse(json);

            //Insertion des valeur du JSON dans une STRING[] json_values
            string payload = (string)obj.Property(Header_Payload_PropertyName).Value;
            string[] json_values = new string[obj.Count+2];
            json_values[0] = "26";
            json_values[1] = "42";
            int k = 0;
            foreach(JProperty property in obj.Properties()){
                json_values[k+2] = (string)property.Value;
                k++;
            }
            
            //Creation du STRING[] contenant toutes les valeurs de la payload
            string[] payload_values = json_values[json_values.Length-1].Split('-');
            string[] total_data = new string[payload_values.Length+json_values.Length-1];

            //Insertion des donnees du header et de la payload(Separes) dans un STRING[]
            for(int i = 0; i < json_values.Length-1; i++){
                total_data[i] = json_values[i];
            }
            for(int i = (json_values.Length-1); i < total_data.Length; i++ ){
                total_data[i] = payload_values[i-(json_values.Length-1)];
            }

            //Convertion de STRING[] en BYTE[]
            byte[] Raw_Data = new byte[total_data.Length];
            for(int i=0; i < Raw_Data.Length; i++){
                Raw_Data[i]=Convert.ToByte(total_data[i],16);
            }

            //Renvoie des Raw Data a transmettre au LoRA
            return Raw_Data;
        }

        //Code byte[] a renvoyer en cas d'erreur
        public static byte[] ErrorsFound(){
            byte[] sendError = new Byte[2];
            if(Id != ""){
                sendError[1] = Convert.ToByte(Id, 16);
                Id = "";
            }
            //Pas reponse serveur
            if(Errors.Equals(0x00)){
                sendError[0] = 0x00;
            //Format invalide
            }else if(Errors.Equals(0x01)){
                sendError[0] = 0x01;
            //Taille de payload invalide
            }else if(Errors.Equals(0x02)){
                sendError[0] = 0x02;
            }
            return sendError;
        }
    }
}