using System;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace test{
    public class Protocol{

        //Taille du Header complete
        private static int Header_Size = 4;

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
        
        
        //Verifie le format des trames arrivant du LoRA et les convertit en fichier JSON
        public static string DataToJson(string reception, string json_type){
            Console.WriteLine("JSON RECEIVED FROM SENSOR: "+reception);
            JObject obj_type = JObject.Parse(json_type);
            JObject obj_reception = JObject.Parse(reception);
            string [] payload = ((string)obj_reception.Property("PAYLOAD").Value).Split('-');
            JArray arr = (JArray)obj_type[Array_Payload_Format];
            
            //Parcourt tout les formats
            foreach (JObject obj in arr.Children<JObject>()){
                if ( (string)(obj_reception.Property(VerProtocol_1_PropertyName).Value) == (string)(obj.Property(VerProtocol_1_PropertyName).Value) && (string)(obj_reception.Property(VerProtocol_2_PropertyName).Value) == (string)(obj.Property(VerProtocol_2_PropertyName).Value) && (string)(obj_reception.Property(Payload_TYPE_MESSAGE).Value) == (string)(obj.Property(Payload_TYPE_MESSAGE).Value) ){
                    JObject format = (JObject)(obj.GetValue(Payload_Format));
                    int h = 0;

                    //Parcourt toutes les proprietes
                    foreach (JProperty property in format.Properties()){
                        if ( !(((string)property.Value).Equals("")) && !(((string)property.Value).Equals(payload[h])) ) {
                            Console.WriteLine("MESSAGE: BAD PAYLOAD VALUES");
                            return "";
                        }
                        h++;
                    }
                    foreach(JProperty property in obj_reception.Properties()){
                        if(property.Name != "PAYLOAD"){
                            property.Value = Convert.ToByte((string)(property.Value),16);
                        }
                    }
                    int k = 0;
                    byte[] oct = new Byte[1];
                    int val;
                    foreach(JProperty property in format.Properties()){
                        //On parcourt la payload, et pour chaque propriétés on rajoute les valeurs présente dans la payload.
                        oct[0] = Convert.ToByte(payload[k], 16);
                        obj_reception.Add(property.Name, oct[0]);
                        k++;
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
        public static string MongoToMQTT(string json){
            Console.WriteLine("JSON RECEIVED FROM DATABASE: "+json);
            Console.WriteLine("");

            //Creer les chaines d'octets pour convertir les valeur de la payload
            JObject obj = JObject.Parse(json);
            int k = 0;
            int value;
            byte[] hex;
            byte[] hex_2 = new byte[1];
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
