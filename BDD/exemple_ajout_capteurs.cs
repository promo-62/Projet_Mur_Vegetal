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
            addNewDocuments<Capteur>(CreateNewCapteurs(), "Capteurs").Wait();
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
        
        private static IEnumerable<Capteur> CreateNewCapteurs()
        {
            Capteur Capteur1 = new Capteur
            {
                idCapteur = 1,
                typeCapteur = new List<string>{"Humidite", "Temperature"},
                projet = new List<string>{"MurVegetal"},
                nom = "Capteur H/T du haut",
                description = "Ce capteur est un capteur d'humidite et de temperature. Il est place en haut du mur vegetal.",
                dateAjout = 1559646116,
                dateDernierReleve = 1560351716,
                batterie = true,
                niveauBatterie = new List<int>{100, 99, 99, 97, 92, 87, 84, 80, 75, 71, 64, 54},
                delaisVeille = 10,
                Actions = new List<Action>(),
                version = 3,
                fonctionne = true
            };
            Capteur1.Actions.Add
            (new Action{
                nom = "LedDelai",
                description = "Delai du clignotement de la led d'etat en secondes.",
                data = 10
            });

            Capteur Capteur2 = new Capteur
            {
                idCapteur = 2,
                typeCapteur = new List<string>{"Temperature"},
                projet = new List<string>{"MurVegetal", "Abeille"},
                nom = "Capteur T interieur",
                description = "Ce capteur est un capteur de temperature servant a comparer la temperature interieur et exterieur.",
                dateAjout = 1559648776,
                dateDernierReleve = 1561362403,
                batterie = true,
                niveauBatterie = new List<int>{100, 99, 99, 97, 92, 87, 84, 80, 75, 71, 64, 54, 45, 35, 22, 12, 2, 2, 2},
                delaisVeille = 10,
                Actions = new List<Action>(),
                version = 3,
                fonctionne = true
            };
            Capteur1.Actions.Add
            (new Action{
                nom = "LedDelai",
                description = "Delai du clignotement de la led d'etat en secondes.",
                data = 10
            });
            
            var newCapteurs = new List<Capteur> {Capteur1, Capteur2};
            return newCapteurs;
        }
    }
}