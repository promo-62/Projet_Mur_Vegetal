using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson; ///Utilise pour le ObjectId
using WebAPI.Models;

//////////////////////////////////////////////////////////////////////////////////
/// Setup d'exemple dans MurVegetalDb : certains sont les hard codes et d'autres sont
/// pseudo generes.DELETE L'ANCIENNE BASE DE DONNEE AVANT ! ATTENTION !!!
/// Example setup on MurVegetalDb : some of them are hard coded while others are
/// pseudo generated. DELETE THE OLD DATA BASE BEFORE !!! WARNING !!!!
//////////////////////////////////////////////////////////////////////////////////

namespace Setup
{   
    public class CapteurComparer : Comparer<Sensors> 
    {
        // Compares by Length, Height, and Width.
        public override int Compare(Sensors x, Sensors y)
        {
            if (x.IdSensor.CompareTo(y.IdSensor) != 0)
            {
                return x.IdSensor.CompareTo(y.IdSensor);
            }
            else
            {
                Console.Error.WriteLine("Two sensors have the same SensorId!!!!");
                return 0;
            }
        }

    }
    class ProgramTest
    {
        static MongoCRUD m_CRUD;
        static Random m_Rand;
        static MongoClient m_Client;
        static IMongoDatabase m_Database;

        static void Main(string[] args)
        {
            StartNewExamples(args);
        }

        static void StartNewExamples(string[] args)
        {
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://127.0.0.1:27017/");
            if(m_Client.GetDatabase("MurVegetalDb") != null)
                m_Client.DropDatabase("MurVegetalDb");
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);

            ExampleFunction();
            
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        //////////////////////////////////////////////////////////////////////////////////////
        /// ExampleFunction, shows somes uses to add a new document or a group of documents.
        /// "CreateNewTypeCapteurs", "CreateNewParameter" and "CreateNewPlants" need to be
        /// done YOURSELF.!-- (don't need to made that in a new function, see their code for
        /// more information.)
        //////////////////////////////////////////////////////////////////////////////////////
        static void ExampleFunction()
        {
            addNewDocuments<SensorTypes>(CreateNewSensorTypes(), "SensorTypes").Wait();
            addNewDocuments<Sensors>(CreateNewSensors(), "Sensors").Wait();

            List<SensorTypes> listTypes = m_CRUD.LoadRecords<SensorTypes>("SensorTypes");
            List<Sensors> listSensors = m_CRUD.LoadRecords<Sensors>("Sensors");
            foreach(Sensors Sensor in listSensors)
                foreach(SensorTypes type in listTypes)
                {
                    if(type.IdSensorType == Sensor.IdSensor)
                    {
                        type.SensorIds.Add(Sensor.Id);
                        m_CRUD.UpsetRecord<SensorTypes>("SensorTypes", ObjectId.Parse(type.Id), type);
                    }
                }

            addNewDocuments<Samples>(CreateNewSamples(), "Samples").Wait();
            addNewDocuments<ProtocolVersions>(CreateNewProtocolVersions(), "ProtocolVersions").Wait();
            addNewDocuments<Plants>(CreateNewPlants(), "Plants").Wait();
            addNewDocuments<Events>(CreateNewEvents(), "Events").Wait();
            addNewDocuments<UsersHololens>(CreateNewUsersHololens(), "UsersHololens").Wait();
            addNewDocuments<UsersAdmin>(CreateNewUsersAdmin(), "UsersAdmin").Wait();
            addNewDocuments<UsersAPI>(CreateNewUsersAPI(), "UsersAPI").Wait();
            addNewDocuments<Socials>(CreateNewSocials(), "Socials").Wait();
            addNewDocuments<Tables>(CreateNewTables(), "Tables").Wait();
            addNewDocuments<Medias>(CreateNewMedias(), "Medias").Wait();
            addNewDocuments<Countdowns>(CreateNewCountdowns(), "Countdowns").Wait();  
            addNewDocuments<Alerts>(CreateNewAlerts(), "Alerts").Wait();
            addNewDocuments<Screens>(CreateNewScreens(), "Screens").Wait();  
        }
        //////////////////////////////////////////////////////////////////////////////////////
        /// addNewDocuments adds the objects given from the "Documents" list inside
        /// the "nameOfCollection" collection.
        /// Works asycronously, therefore, you need to use .Wait()
        ///     Documents : List of objects that will be added. NEEDS TO BE COMPLETED 
        ///                 BEFORE USING THIS FUNCTION.
        ///     nameOfCollection : Name of the collections. Will be added if the collection
        ///                        doesn't exist.
        ///     nameOfDataBase : Name of the data base. Will be added if the data base
        ///                        doesn't exist. "ISENption" is at default.
        /// return : Task, don't touch if you don't know what is it.
        //////////////////////////////////////////////////////////////////////////////////////
        public static async Task addNewDocuments<T>(IEnumerable<T> Documents, string nameOfCollection, string nameOfDataBase = "ISENption")
        {
            var collection = m_Database.GetCollection<T>(nameOfCollection);
            await collection.InsertManyAsync(Documents);
        }
        private static IEnumerable<SensorTypes> CreateNewSensorTypes()
        {
            List<string> sensorTypes = new List<string>{"Humidite", "Temperature", "Luminosite", "QualiteDAir", "Pression"};
            
            var NewSensorType = new List<SensorTypes>();
            for(int i = 0; i < sensorTypes.Count; i++)
            {
                NewSensorType.Add( new SensorTypes
                {
                    IdSensorType = i,
                    SamplesTypes = new List<string>(),
                    SensorIds = new List<string>()
                });
                NewSensorType[i].SamplesTypes.Add(sensorTypes[i]);

            }
            return NewSensorType;
        }
        
        private static IEnumerable<Sensors> CreateNewSensors()
        {
            var NewSensors = new List<Sensors>();
            for(int i = 0; i < 10; i++)
            {
                bool f_IsWorking = true;
                if(m_Rand.Next(8) == 0)
                    f_IsWorking = false;
                int sensorType = m_Rand.Next(5);
                NewSensors.Add( new Sensors
                {
                    IdSensor = i,
                    IdSensorType = sensorType,
                    Project = new List<string>{"MurVegetal"},
                    Name = "",
                    Description = "",
                    SensorDate = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000),
                    LastSampleDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Battery = true,
                    BatteryLevel = new List<int>(),
                    SleepTime = 10,
                    Action = new List<ActionModel>(),
                    Version = 3,
                    IsWorking = f_IsWorking,
                    TimeOut = 0
                });
                
                
                NewSensors[i].BatteryLevel.Add(100);
                int max = m_Rand.Next(40);
                for(int j = 0; j < max; j++)
                {
                    int newBatteryLvl = NewSensors[i].BatteryLevel[j] - m_Rand.Next(4);
                    if(newBatteryLvl < 0)
                        newBatteryLvl = 0;
                    NewSensors[i].BatteryLevel.Add(newBatteryLvl);
                }
                if(m_Rand.Next(2) == 0)
                    NewSensors[i].Action.Add
                    (new ActionModel{
                        Name = "LedDelai",
                        Description = "Delai du clignotement de la led d'etat en secondes.",
                        ToDo = 10
                    });
            }
            return NewSensors;
        }
        
        private static IEnumerable<Samples> CreateNewSamples()
        {
            List<Sensors> sensors = m_CRUD.LoadRecords<Sensors>("Sensors");
            var newSamples = new List<Samples> {};
            int nbSample = m_Rand.Next(50) + 50;
            for(int i = 0; i < nbSample; i++)
            {
                int idSensor = m_Rand.Next(5);
                long f_SampleDate = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000);
                if(sensors[idSensor].LastSampleDate < f_SampleDate)
                    sensors[idSensor].LastSampleDate = f_SampleDate;
                    
                m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensors[idSensor].Id), sensors[idSensor]);
                newSamples.Add(new Samples
                {
                    IdSensor = sensors[idSensor].IdSensor,
                    SampleDate = f_SampleDate,
                    Value = m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26)
                });
            }
            
            return newSamples;
        }
        private static IEnumerable<ProtocolVersions> CreateNewProtocolVersions(){
            ProtocolVersions ProtocolVersion1 = new ProtocolVersions
            {
                Version = 2,
                Message = new List<MessageModel>(),
            };
            ProtocolVersion1.Message.Add
            (new MessageModel{
                TypeMessage = 1,
                PayloadParam = new List<PayloadParamModel>(),
            });
            ProtocolVersion1.Message.Add
            (new MessageModel{
                TypeMessage = 2,
                PayloadParam = new List<PayloadParamModel>(),
            });
            ProtocolVersion1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Size = 36
                }
            );
            ProtocolVersion1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Size = 16
                }
            );
            ProtocolVersion1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Size = 200
                }
            );
            ProtocolVersion1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "3",
                    Size = 20
                }
            );
            var newProtocolVersions = new List<ProtocolVersions> {ProtocolVersion1};
            return newProtocolVersions;
        }
        private static IEnumerable<Plants> CreateNewPlants(){
            Plants Plant1 = new Plants
            {
                Name = "polystichum setiferum proliferum",
                Temperature = "Bonjour je suis une polystichum setiferum proliferum",
                Humidity="J'ai swouaf",
                Luminosity="gris",
                PositionX = 1,
                PositionY = 5,
                Image = "https://www.andre-briant.fr/media/polystichum_polyblepharum__023010700_1629_24042016.jpg",
            };
            var newPlants = new List<Plants> {Plant1};
            return newPlants;
        }
        private static IEnumerable<Events> CreateNewEvents(){
            Events Event1 = new Events
            {
                Name = "VENEZ AU LASER GAME !!!!",
                EventDate = 1559689200,
                BeginningDate = 1559676600,
                EndingDate=1559689200,
                EventImage = "vacance.jpeg",
                Text="pew pew",
                Position = 4,
            };
            //ne sert plus a rien ??
            /*Event1.Data.Add
            (new DataModel{
                LinkImg = "https://i2.cdscdn.com/pdt2/4/9/2/1/700x700/auc2009459774492/rw/stickers-citron-rigolo-sens-inverse-30-x-30-cm.jpg",
                LinkVideo = "https://www.youtube.com/watch?v=3q7oJuyy5Ac",
                Text = "TOP 10 DES TRUCS LES PLUS DROLES D INTERNET LE TROISIEME VA VOUS RENDRE FOU !!!"
            });*/
            var newEvents = new List<Events> {Event1};
            return newEvents;
        }
        private static IEnumerable<UsersHololens> CreateNewUsersHololens(){
            UsersHololens User1 = new UsersHololens
            {
                Name = "User",
                Password = "User",
                HololensUsername = "Billy"
            };
            var newUsers = new List<UsersHololens> {User1};
            return newUsers;
        }
        private static IEnumerable<UsersAdmin> CreateNewUsersAdmin(){
            UsersAdmin User1 = new UsersAdmin
            {
                Username = "UserLambda",
                PasswordHash = "TaMaman",
                HashKey = "BobbyBob"
            };
            var newUsers = new List<UsersAdmin> {User1};
            return newUsers;
        }
        private static IEnumerable<UsersAPI> CreateNewUsersAPI(){
            UsersAPI admin = new UsersAPI
            {
                Username = "admin",
                PasswordHash = "dda4c543d55d652c43be7f8be2c15982394a1faa00f3ea51297240452178c37f",
                Salt = "blabla",
                AccreditationLevel = 3
            };
            UsersAPI interfaceWeb = new UsersAPI
            {
                Username = "interfaceWeb",
                PasswordHash = "68b1382b48b44ec56651d0ac56e7d775979e1d85b71ca1c52e2465138d22d121",
                Salt = "cococococo",
                AccreditationLevel = 1
            };
            UsersAPI interfaceMobile = new UsersAPI
            {
                Username = "interfaceMobile",
                PasswordHash = "6dc248aff25aa300c74817846b913f63b78a6f350822e74c14e0592c424177be",
                Salt = "coucoujesuis1seltrèss4le",
                AccreditationLevel = 1
            };
            var newUsers = new List<UsersAPI> {admin, interfaceWeb, interfaceMobile};
            return newUsers;
        }
        private static IEnumerable<Socials> CreateNewSocials(){
            Socials Social1 = new Socials
            {
                Username = "MurVegetal",
                PageWidget = "accueil",
                Widget = "http://poulespondeuses.com/wp-content/uploads/2019/02/Poussin-Muesli.jpg",
            };
            var newSocials = new List<Socials> {Social1};
            return newSocials;
        }
        private static IEnumerable<Tables> CreateNewTables(){
            Tables Table1 = new Tables
            {
                OnScreenTime = 10,
                IsOnScreen = true,
                Name = "LA Joconde",
                CarrousselTime = 20,
            };
            var newTable = new List<Tables> {Table1};
            return newTable;
        }
        private static IEnumerable<Medias> CreateNewMedias(){
            Medias Media1 = new Medias
            {
                Name = "BFMTV",
                BeginningDate = 1559689243,
                EndingDate = 1559689946,
                Video="ytp.wav",
                Image="vacances.jpeg"
            };
            //bah osef aussi du coup
            /* Media1.Data.Add
            (new DataModel{
                LinkImg = "https://i2.cdscdn.com/pdt2/4/9/2/1/700x700/auc2009459774492/rw/stickers-citron-rigolo-sens-inverse-30-x-30-cm.jpg",
                LinkVideo = "https://www.youtube.com/watch?v=3q7oJuyy5Ac",
                Text = "TOP 10 DES TRUCS LES PLUS DROLES D INTERNET LE TROISIEME VA VOUS RENDRE FOU !!!"
            });*/
            var newMedias = new List<Medias> {Media1};
            return newMedias;
        }
        private static IEnumerable<Countdowns> CreateNewCountdowns(){
            Countdowns Countdown1 = new Countdowns
            {
                Name = "je suis le car numero 1",
                Text = "ATTENTION: Il ne reste plus beaucoup de temps pour vous acheter votre abonnement TéléZ",
                EndingDateEvent = 1559689946,
                BeginningDateEvent = 1559689243,
                EndingDateCountdown = 1559689946,
                Position = m_Rand.Next(203) + 100
            };
            /* 
            CompteARebours1.Data.Add
            (new Data{
                linkImg = "http://fr.web.img4.acsta.net/c_216_288/pictures/19/04/25/17/17/5767838.jpg",
                linkVideo = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                texte = "IL MONTE SUR UNE GRUE (l oiseau) ET CA TOURNE MAL OMG !!!!"
            });*/
            var newCountdowns = new List<Countdowns> {Countdown1};
            return newCountdowns;
        }
        private static IEnumerable<Alerts> CreateNewAlerts(){
            Alerts Alert1 = new Alerts
            {
                IdSensor = m_Rand.Next(5),
                Name = "je suis une alerte",
                DateAlert = 1559689996,
                IsWorking = true,
                AlertReason = "La raison de l'alerte est encore une demande random de changement de la bdd."
            };
            /* 
            CompteARebours1.Data.Add
            (new Data{
                linkImg = "http://fr.web.img4.acsta.net/c_216_288/pictures/19/04/25/17/17/5767838.jpg",
                linkVideo = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                texte = "IL MONTE SUR UNE GRUE (l oiseau) ET CA TOURNE MAL OMG !!!!"
            });*/
            var newCountdowns = new List<Alerts> {Alert1};
            return newCountdowns;
        }

        private static IEnumerable<Screens> CreateNewScreens(){

            Screens Screen1 = new Screens{

                OnDate = 1561365090,
                OffDate = 1561375090,
                Delay = 600

            };

            var newScreens = new List<Screens> {Screen1};
            return newScreens;
        }
    }
}