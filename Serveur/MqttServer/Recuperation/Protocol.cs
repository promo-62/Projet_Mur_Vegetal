using System;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace test{
    public class Protocol{

        //Nom du tableau qui contient tout les formats de header en reponse
        private static string Header_Response = "Header_Response";
        //Nom du tableau contenant tout les formats de header
        private static string Array_Payload_Format = "Payload";
        //Nom du tableau contenant toutes les propriétés d'envoie
        private static string Payload_Format = "Format";
        //Nom du tableau contenant tous les types de messages
        private static string Payload_TYPE_MESSAGE = "TYPE_MESSAGE";
        //Nom de la premiere propriete de la version du protocole
        private static string VerProtocol_1_PropertyName = "VERSION_PROTOCOL_1";
        //Nom de la seconde propriete de la version du protocole
        private static string VerProtocol_2_PropertyName = "VERSION_PROTOCOL_2";
        private static string Data_PropertyName = "DATA";
        
        
        //Verifie le format des trames arrivant du LoRA et les convertit en fichier JSON
        public static string MQTTToMongo(string reception, string json_type){
            Console.WriteLine("JSON RECEIVED FROM SENSOR: "+reception);
            Console.WriteLine("");
            JObject obj_type = JObject.Parse(json_type);
            JObject obj_reception = JObject.Parse(reception);
            string [] payload = ((string)obj_reception.Property("PAYLOAD").Value).Split('-');
            JArray arr = (JArray)obj_type[Array_Payload_Format];
            
            //Parcourt tout les formats
            foreach (JObject obj in arr.Children<JObject>()){
                if ( (string)(obj_reception.Property(VerProtocol_1_PropertyName).Value) == (string)(obj.Property(VerProtocol_1_PropertyName).Value) && (string)(obj_reception.Property(VerProtocol_2_PropertyName).Value) == (string)(obj.Property(VerProtocol_2_PropertyName).Value) && (string)(obj_reception.Property(Payload_TYPE_MESSAGE).Value) == (string)(obj.Property(Payload_TYPE_MESSAGE).Value) ){
                    JObject format = (JObject)(obj.GetValue(Payload_Format));

                    /*int z = payload.Length-1;
                    while(payload[z] == "00" && z >= 0 ){
                        z--;
                    }
                    if(format.Count < (z+1)){
                        Console.WriteLine("MESSAGE: BAD PAYLOAD SIZE");
                        return "";
                    }*/
                    
                    int h = 0;
                    //Parcourt toutes les proprietes
                    /*foreach (JProperty property in format.Properties()){
                        if ( !(((string)property.Value).Equals("")) && !(((string)property.Value).Equals(payload[h])) ) {
                            Console.WriteLine("MESSAGE: BAD PAYLOAD VALUES");
                            return "";
                        }
                        h++;
                    }*/
                    foreach(JProperty property in obj_reception.Properties()){
                        if(property.Name != "PAYLOAD"){
                            try{
                                property.Value = Convert.ToByte((string)(property.Value),16);
                            }catch(Exception e){
                                Console.WriteLine("MESSAGE: NON-HEX VALUES");
                                return "";
                            }
                        }
                    }
                    int k = 0;
                    byte[] oct = new Byte[1];

                    foreach(JProperty property in format.Properties()){
                        //On parcourt la payload, et pour chaque propriétés on rajoute les valeurs présente dans la payload.
                        string[] tab = new string[payload.Length-format.Count+1];
                        
                        if(property.Name == Data_PropertyName){
                            int g = k;
                            for(int v = k; v < tab.Length+k; v++){
                                try{
                                    tab[v-k] = ((int)Convert.ToByte(payload[g], 16)).ToString();
                                }catch(Exception e){
                                    Console.WriteLine("MESSAGE: NON-HEX VALUES");
                                    return "";
                                }
                                g++;
                            }
                            obj_reception.Add(property.Name, String.Join(",", tab));
                            k = g;
                        }else{
                            try{
                                oct[0] = Convert.ToByte(payload[k], 16);
                            }catch(Exception e){
                                Console.WriteLine("MESSAGE: NON-HEX VALUES");
                                return "";
                            }
                            obj_reception.Add(property.Name, oct[0]);
                            k++;
                        }
                    }
                    //On vide la payload
                    obj_reception.Remove("PAYLOAD");
                    Console.WriteLine("JSON CREATED FOR DATABASE: "+obj_reception.ToString());
                    Console.WriteLine("");
                    //Renvoie du JSON en string
                    return obj_reception.ToString();
                }
            }
            Console.WriteLine("MESSAGE: BAD PAYLOAD HEADER");
            return "";
        }
    
        
        //Verifie le format des trames arrivant du LoRA et les convertit en fichier JSON
        public static string MongoToMQTT(string json, string json_config){
            Console.WriteLine("JSON RECEIVED FROM DATABASE: "+json);
            Console.WriteLine("");

            JObject obj = JObject.Parse(json);
            JObject config = JObject.Parse(json_config);
            JArray arr = (JArray)config[Header_Response];
            int Header_Size = 4;

            string[]  header_values = new string[3];
            byte[] tmp = new Byte[1];
            tmp[0] = (BitConverter.GetBytes((int)obj.Property(VerProtocol_1_PropertyName).Value))[0];
            header_values[0] = BitConverter.ToString(tmp);
            tmp[0] = (BitConverter.GetBytes((int)obj.Property(VerProtocol_2_PropertyName).Value))[0];
            header_values[1] = BitConverter.ToString(tmp);
            tmp[0] = (BitConverter.GetBytes((int)obj.Property(Payload_TYPE_MESSAGE).Value))[0];
            header_values[2] = BitConverter.ToString(tmp);

            foreach(JObject header in arr.Children<JObject>()){
                if ( (string)(header.Property(VerProtocol_1_PropertyName).Value) == header_values[0] && (string)(header.Property(VerProtocol_2_PropertyName).Value) == header_values[1] && (string)(header.Property(Payload_TYPE_MESSAGE).Value) == header_values[2] ){
                    Header_Size = header.Count-2;
                    break;
                }
            }

            //Creer les chaines d'octets pour convertir les valeur de la payload
            int k = 0;
            int value;
            byte[] hex;
            byte[] hex_2 = new byte[1];
            //+1 pour rajouter le 2em octet de l'ID
            byte[] payload = new byte[obj.Count-(Header_Size)];
            //Sauvegarde les proproietes (de la payload) a supprimer de l'objet
            JProperty[] toRemove = new JProperty[obj.Count-(Header_Size)];
            //Rajoute chaque propriete dans le byte[] payload
            foreach(JProperty property in obj.Properties()){
                //Verifie si les propriete appartiennent a la payload et convertit toutes les valeurs en hex
                if(k >= Header_Size){
                    value = (int)property.Value;
                    payload[k-(Header_Size)] = BitConverter.GetBytes(value)[0];
                    toRemove[k-(Header_Size)] = property;
                }else{
                    value = (int)property.Value;
                    hex = BitConverter.GetBytes(value);
                    hex_2[0] = hex[0];
                    property.Value = BitConverter.ToString(hex_2);
                }
                k++;
            }
            //Supprime les prorpiete de la payload
            for(int i = 0; i < toRemove.Length; i++){
                obj.Remove(toRemove[i].Name);
            }
            //Rajoute la propriete payload qui contient toute les valeurs en hex
            obj.Add("PAYLOAD", BitConverter.ToString(payload));
            Console.WriteLine("JSON CREATED FOR SENSOR: "+obj.ToString());
            Console.WriteLine("");

            return obj.ToString();
        }
    }
}
