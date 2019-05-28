using System;
using MongoDB.Bson; ///Utilise pour le ObjectId
using System.Collections.Generic;

namespace Tests
{
    //////////////////////////////////////////////////////////////////////////////////////
    /// An actionParam defines the parameters of the action used to send the command in
    ///   the rasberry.
    //////////////////////////////////////////////////////////////////////////////////////
       internal class ActionParam
        {
        //Position of the values of this parameter in the network queue
        public string position { get; set; }
        //Length of the parameter, in Bytes
        public int length { get; set; }
        //Name of the parameter
        public string name { get; set; }
        //Description of the parameter
        public string description { get; set; }
        //Default value of the parameter.
        public int defaultValue { get; set; }

        }
    //////////////////////////////////////////////////////////////////////////////////////
    /// An action defines a command send to a componant through the rasberry
    ///
    /// An example of this is opening the valve:
    ///  { 0x01 - Open Valve - Open the valve to water the plants.} 
    ///
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Action
        {
        //Reference the type of action
        public string idAction { get; set; }
        //Name of the action
        public string name { get; set; }
        //Description of the action
        public string description { get; set; }
        //Object ActionParam hold the parameters used to send the message to the rasberry.
        public ActionParam ActionParams { get; set; }
        
        }
    //////////////////////////////////////////////////////////////////////////////////////
    /// A parameter defines a type of value that will be used to read
    ///     the summary send by the rasberry
    ///
    /// Example : {ObjectID - 1 - 2 - standby - standby's duration in minuts - 10}
    ///
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Parametre
        {
        //ID of the summary used in MongoDB
        public ObjectId _id { get; set; }
        //Position of the values of this parameter in the network queue
        public int position { get; set; }
        //Length of the parameter, in Bytes
        public int length { get; set; }
        //Name of the parameter
        public string name { get; set; }
        //Description of the parameter
        public string description { get; set; }
        //Default value of the parameter, used in first connection.
        public int defaultValue { get; set; }
        }
    
    //////////////////////////////////////////////////////////////////////////////////////
    /// A summary ("Releve") defines a value calculated and send by the rasberry to the database.
    ///
    /// Example : {ObjectID - 1 - Battery - % of battery - 1 - 1 - 12672372371 - 1}
    ///
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Releve
        {
        //ID of the summary used in MongoDB
        public ObjectId _id { get; set; }
        //Name of the value calculated
        public string name { get; set; }
        //Description of the summary
        public string description { get; set; }
        //Position of the value in the network queue
        public string position { get; set; }
        //Length of the value, in Bytes
        public int length { get; set; }
        //Time in seconds since 1/1/1970
        public int time { get; set; }
        //1 = int | 2 = float
        public int Type { get; set; } 
        
        }
    //////////////////////////////////////////////////////////////////////////////////////
    /// Each componant can have several versions, which defines the parameters, actions and
    ///     action parameters. In other words, it defines the way the version works.
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Version
        {
        public string idVersionComp { get; set; }
        public int numero { get; set; }
        public string description { get; set; }
        public List<String> Parametres { get; set; }
        public List<String> Releves { get; set; }
        public List<Action> Actions { get; set; }
    }
    //////////////////////////////////////////////////////////////////////////////////////
    /// The "TypeCapteur" defines an entire type of componant, for example:
    ///     a temperature sensor.
    //////////////////////////////////////////////////////////////////////////////////////
    internal class TypeCapteur
        {
        //ID of the summary used in MongoDB
        public ObjectId _id { get; set; }
        //ID of the type of compoanant (example : 1 = temperature sensor, etc... )
        public string idNatureComp { get; set; }
        //Name of the type of componant.
        public string name { get; set; }
        //Description of the general uses of the componant.
        public string description { get; set; }
        //List of componant's versions.
        public List<Version> Versions { get; set; }
    }
    //////////////////////////////////////////////////////////////////////////////////////
    /// The "Capteur" defines an actual phisical componant which is placed somewhere
    /// and was already connected once.
    //////////////////////////////////////////////////////////////////////////////////////
    internal class Capteur
        {
        //ID of the summary used in MongoDB
        public ObjectId _id { get; set; }
        //Keep the lora adress from the rasberry on the first connection.
        public string addressLora { get; set; }
        //ID of the type of compoanant (example: 1 = temperature sensor, etc...)
        public string idNatureComp { get; set; }
        //ID of the project (example: 1 = test, 2 = herbal wall, etc...)
        public int idProject { get; set; }
        //Name of the actual componant.
        public string name { get; set; }
        //Precise description of the actual componant (what, where, uses, etc..)
        public string description { get; set; }
        //Version of this componant. Hold a version ID.
        public string Version { get; set; }
        //List of command that can be send to the componant.
        public List<Action> Actions { get; set; } 
    }
}