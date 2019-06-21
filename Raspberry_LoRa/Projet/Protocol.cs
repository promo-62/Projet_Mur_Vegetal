using System;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Projet{
    public class Protocol{

        //Nom du tableau contenant tout les formats de header d'envoie
        private static string Array_Formats_PropertyName = "Header";
        //Nom du tableau contenant tout les formats de header de reponse
        private static string Array_Header_Response = "Header_Response";
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
        //Nom de la propriete contenant tout les propriete a retirer du header
        private static string Header_To_Remove = "TO_REMOVE";
        //Nom de la propriete contenant tout les propriete a ajouter au header
        private static string Header_To_Add = "TO_ADD";
        //Nom de la propriete contenant la position dans le tableau de byte a envoyer a la base de donnees
        private static string Header_To_Add_Position = "POSITION";
        //Nom de la propriete contenant le format de la payload
        private static string Payload_Format = "Format";
        //Nom de la propriete contenant l'ID dans la payload
        private static string ID_PropertyName = "ID_1";
        private static string ID2_PropertyName = "ID_2";
        //Stockage de l'ID au cas ou la base de donnees ne repond plus
        private static string Id = "";
        //Nom du tableau contenant tout les formats de payload
        private static string Array_Payload_Format = "Payload";
        //Nom de la propriete contenant le type de message dans le header
        private static string Header_Type_PropertyName = "TYPE_MESSAGE";
        //Octet stockant le type d'erreur
        private static byte Errors;
        
        //Verifie le format des trames arrivant du LoRA et les convertit en fichier JSON
        public static async Task<string> DataToJson(byte[] chain,string file1){
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
                    if( !property.Name.Equals(Header_To_Remove) && !property.Name.Equals(Header_To_Add)){
                        string value = (string)property.Value;
                        if( !(value.Equals(header[k])) && !(value.Equals("")) ){
                            isValidated = false;
                        }
                        k++;
                    }
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

                    int header_size = obj.Count-2;
                    for(int i = obj.Count+size-2; i < chain.Length; i++){
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
                        if( !property.Name.Equals(Header_To_Remove) && !property.Name.Equals(Header_To_Add)){
                            string value = (string)property.Value;
                            if((value.Equals("")) ){
                                obj.Property(property.Name).Value = header[z];
                            }
                            z++;
                        }
                    }

                    byte[] payload = new byte[size];
                    for(int i = k; i < payload.Length+k; i++){
                        payload[i-k] = chain[i]; 
                    }
                    string raw_data = BitConverter.ToString(payload);
                    obj.Add(Header_Payload_PropertyName, raw_data);

                    JObject ToRemove = (JObject)obj.GetValue(Header_To_Remove);
                    foreach(JProperty property in ToRemove.Properties()){
                        obj.Remove(property.Name);
                    }
                    JArray ToAdd = (JArray)obj[Header_To_Add];
                    foreach(JObject obj_ToAdd in ToAdd.Children<JObject>()){
                        foreach(JProperty property in obj_ToAdd.Properties()){
                            obj.Add(property.Name, property.Value);
                        }
                    }

                    obj.Remove(Header_To_Remove);
                    obj.Remove(Header_To_Add);
                    Console.WriteLine("JSON CREATED: "+obj.ToString());
                    Console.WriteLine("");

                    //Renvoie du JSON en string
                    //string response = obj.ToString();
                    string response = await MQTT_Raspberry.RaspberryToServer(obj.ToString());

                    //Verifie si le serveur repond et si il ne repond pas parse la payload et stocke l'ID 
                    if(response.Equals("")){
                        JArray tableau = (JArray)obj_config[Array_Payload_Format];
                        foreach(JObject obj_id in tableau.Children<JObject>()){
                            if ( (string)(obj.Property(Header_VerProtocol_1_PropertyName).Value) == (string)(obj_id.Property(Header_VerProtocol_1_PropertyName).Value) && (string)(obj.Property(Header_VerProtocol_2_PropertyName).Value) == (string)(obj_id.Property(Header_VerProtocol_2_PropertyName).Value) && (string)(obj.Property(Header_Type_PropertyName).Value) == (string)(obj_id.Property(Header_Type_PropertyName).Value) ){
                                if( !((string)(obj.Property(Header_Type_PropertyName))).Equals("01") ){
                                    JObject format = (JObject)(obj_id.GetValue(Payload_Format));
                                    int w = 0;
                                    byte[] rev = new byte[2];
                                    foreach(JProperty property in format.Properties()){
                                        if(property.Name == ID_PropertyName){
                                            rev[0] = chain[w+header_size];
                                        }
                                        if(property.Name == ID2_PropertyName){
                                            rev[1] = chain[w+header_size];
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
        public static byte[] JsonToData(string json, string file1){

            //Convertit la reponse string en JSON
            JObject obj = JObject.Parse(json);
            JObject config = JObject.Parse(file1);
            JArray arr = (JArray)config[Array_Header_Response];
            foreach(JObject obj_conf in arr.Children<JObject>()){
                if( ((string)obj_conf.Property(Header_VerProtocol_1_PropertyName).Value).Equals((string)obj.Property(Header_VerProtocol_1_PropertyName).Value) && ((string)obj.Property(Header_VerProtocol_2_PropertyName).Value).Equals((string)obj_conf.Property(Header_VerProtocol_2_PropertyName).Value) ){
                    JObject obj_format = obj_conf;

                    JObject ToRemove = (JObject)obj_format.GetValue(Header_To_Remove);
                    foreach(JProperty property in ToRemove.Properties()){
                        obj.Remove(property.Name);
                    }
                    JArray ToAdd = (JArray)obj_format[Header_To_Add];
                    string[] json_values = new string[obj.Count+ToAdd.Count];
                    int[] positions = new int[ToAdd.Count];
                    int y = 0;
                    foreach(JObject obj_ToAdd in ToAdd.Children<JObject>()){
                        foreach(JProperty property in obj_ToAdd.Properties()){
                            //obj.Add(property.Name, property.Value);
                            if(property.Name.Equals(Header_To_Add_Position)){
                                positions[y] = (int)property.Value;
                            }else{
                                json_values[positions[y]] = (string)property.Value;
                            }
                        }
                        y++;
                    }
                    //Insertion des valeur du JSON dans une STRING[] json_values
                    string payload = (string)obj.Property(Header_Payload_PropertyName).Value;
                    int k = 0;
                    foreach(JProperty property in obj.Properties()){
                        if(positions.Contains(k)){
                            while(positions.Contains(k)){
                                k++;
                            }
                        }
                        json_values[k] = (string)property.Value;
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
            }
            byte[] erreurs = {0x00};
            return erreurs;
        }

        //Code byte[] a renvoyer en cas d'erreur
        public static byte[] ErrorsFound(){
            byte[] sendError = new Byte[3];
            if(Id != ""){
                string[] ids = Id.Split('-');
                sendError[1] = Convert.ToByte(ids[0], 16);
                sendError[2] = Convert.ToByte(ids[1], 16);
                Id = "";
            }
            //Pas reponse serveur
            if(Errors.Equals(0x00)){
                sendError[0] = 0x01;
            //Format Header invalide
            }else if(Errors.Equals(0x01)){
                sendError[0] = 0x02;
            //Taille de payload invalide
            }else if(Errors.Equals(0x02)){
                sendError[0] = 0x03;
            }
            return sendError;
        }
    }
}