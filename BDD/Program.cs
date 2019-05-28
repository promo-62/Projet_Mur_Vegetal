using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson; ///Utilise pour le ObjectId

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
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
            addNewDocuments<TypeCapteur>(CreateNewTypeCapteurs(), "Type Capteurs").Wait();
            addNewDocuments<Parametre>(CreateNewParameter(), "Parametres").Wait();
            addNewDocuments<Plant>(CreateNewPlants(), "Plants").Wait();
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
            var client = new MongoClient();
            IMongoDatabase db = client.GetDatabase(nameOfDataBase);
            var collection = db.GetCollection<T>(nameOfCollection);
            await collection.InsertManyAsync(Documents);
        }

        private static IEnumerable<TypeCapteur> CreateNewTypeCapteurs()
        {
            var Capteur1 = new TypeCapteur
            {
                idNatureComp = "01",
                name = "Capteur de Temperature",
                description = "Capteur1",
                Versions = new List<Version>()
            };
            //Gerard.AddSystem("Systeme Solaire");
            //System.AddPlanet("Terre");
            //Planet.AddLife();

            Capteur1.Versions.Add( new Version()
            {
                idVersionComp = "0.1",  
                numero = 1,
                description = "coucou",
                Parametres = new List<String>(),
                Actions = new List<Action>()
            });
            Capteur1.Versions[0].Parametres.Add("Param_Id    ");
            Capteur1.Versions[0].Parametres.Add("Param_Veille");
            
            var newCapteurs = new List<TypeCapteur> {Capteur1};

            return newCapteurs;
        }
        private static IEnumerable<Plant> CreateNewPlants()
        {
            var Plant_1 = new Plant
            {
                name = "Tournesol",
                exposition = "Semi-shade",
                port = "Really beautiful",
                hauteur =  100000,
                substrat = "Fucking watered soil!!!",
                picture = "yolo.png"
            };
            
            var newPlants = new List<Plant> {Plant_1};

            return newPlants;
        }
        
        private static IEnumerable<Capteur> CreateNewCapteurs()
        {
            
            var newCapteurs = new List<Capteur> {};

            return newCapteurs;
        }
        private static IEnumerable<Parametre> CreateNewParameter()
        {
            var Param_id = new Parametre
            {
                position = 1,
                length = 2,
                name = "Identifiant",
                description = "L'identifiant unique blabla.",
                defaultValue = 0
            };
            
            var Param_Veille = new Parametre
            {
                position = 1,
                length = 2,
                name = "Veille",
                description = "L'identifiant unique blabla.",
                defaultValue = 0
            };

            var newCapteurs = new List<Parametre> {Param_id, Param_Veille};

            return newCapteurs;
        }
    }
}