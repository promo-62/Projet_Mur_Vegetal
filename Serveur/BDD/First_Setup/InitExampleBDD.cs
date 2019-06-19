using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson; ///Utilise pour le ObjectId
using CapteursApi.Models;

namespace Setup
{   
    public class CapteurComparer : Comparer<Capteur> 
    {
        // Compares by Length, Height, and Width.
        public override int Compare(Capteur x, Capteur y)
        {
            if (x.IdCapteur.CompareTo(y.IdCapteur) != 0)
            {
                return x.IdCapteur.CompareTo(y.IdCapteur);
            }
            else
            {
                Console.Error.WriteLine("Deux capteurs ont le meme idCapteur!!!!");
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
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);

            ExampleFunction();
            
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
        static int createNewIdCapteur()
        {
            List<Capteur> capteurs = m_CRUD.LoadRecords<Capteur>("Capteurs");
            capteurs.Sort(new CapteurComparer());
            int newID = 0;
            foreach(Capteur c in capteurs)
            {
                if(c.IdCapteur != newID)
                    return newID;
                newID++;
            }
            return newID;
        }

        //////////////////////////////////////////////////////////////////////////////////////
        /// ExampleFunction, shows somes uses to add a new document or a group of documents.
        /// "CreateNewTypeCapteurs", "CreateNewParameter" and "CreateNewPlants" need to be
        /// done YOURSELF.!-- (don't need to made that in a new function, see their code for
        /// more information.)
        //////////////////////////////////////////////////////////////////////////////////////
        static void ExampleFunction()
        {
            addNewDocuments<Capteur>(CreateNewCapteur(), "Capteurs").Wait();
            addNewDocuments<Capteur>(CreateNewCapteur(), "Capteurs").Wait();
            addNewDocuments<Capteur>(CreateNewCapteur(), "Capteurs").Wait();
            addNewDocuments<Capteur>(CreateNewCapteur(), "Capteurs").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<Releve>(CreateNewReleve(), "Releve").Wait();
            addNewDocuments<VersionProtocole>(CreateNewVersionProtocoles(), "Version_protocole").Wait();
            addNewDocuments<Plantes>(CreateNewPlantes(), "Plantes").Wait();
            addNewDocuments<Event>(CreateNewEvents(), "Event").Wait();
            addNewDocuments<User>(CreateNewUsers(), "User").Wait();
            addNewDocuments<Social>(CreateNewSocial(), "Social").Wait();
            addNewDocuments<Tableau>(CreateNewTableaux(), "Tableau").Wait();
            addNewDocuments<Media>(CreateNewMedias(), "Media").Wait();
            addNewDocuments<CompteARebours>(CreateNewCompteARebours(), "CompteARebours").Wait();   
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
        
        private static IEnumerable<Capteur> CreateNewCapteur()
        {
            List<string> typesCapteur = new List<string>{"Humidite", "Temperature", "Luminosite", "QualiteDAir", "Pression"};
            Capteur Capteur1 = new Capteur
            {
                IdCapteur = createNewIdCapteur(),
                TypeCapteur = new List<string>(),
                Projet = new List<string>{"MurVegetal"},
                Nom = "",
                Description = "Ceci est un capteur exemple",
                DateCapteur = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000),
                DateDernierReleve = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Batterie = true,
                NiveauBatterie = new List<int>(),
                DelaiVeille = 10,
                Action = new List<ActionModel>(),
                Version = 3,
                Fonctionne = true
            };
            List<int> entiers = new List<int>();
            for(int i = 0; i < 3; i++)
            {
                int temp = m_Rand.Next(typesCapteur.Count);
                if(entiers.IndexOf(temp) == -1)
                {
                    entiers.Add(temp);
                    Capteur1.TypeCapteur.Add(typesCapteur[temp]);
                }
            }
            Capteur1.NiveauBatterie.Add(100);
            int max = m_Rand.Next(40);
            for(int i = 0; i < max; i++)
            {
                int newBatteryLvl = Capteur1.NiveauBatterie[i] - m_Rand.Next(4);
                if(newBatteryLvl < 0)
                    newBatteryLvl = 0;
                Capteur1.NiveauBatterie.Add(newBatteryLvl);
            }
            if(m_Rand.Next(2) == 0)
                Capteur1.Action.Add
                (new ActionModel{
                    Nom = "LedDelai",
                    Description = "Delai du clignotement de la led d'etat en secondes.",
                    Data = 10
                });
            
            var newCapteurs = new List<Capteur> {Capteur1};
            return newCapteurs;
        }
        
        private static IEnumerable<Releve> CreateNewReleve()
        {
            List<Capteur> capteurs = m_CRUD.LoadRecords<Capteur>("Capteurs");
            Capteur capteur = capteurs[m_Rand.Next(capteurs.Count)];
            long f_dateReleve = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000);
            if(capteur.DateDernierReleve < f_dateReleve)
                capteur.DateDernierReleve = f_dateReleve;
            m_CRUD.UpsetRecord<Capteur>("Capteurs", capteur.Id, capteur);
            Releve releve1 = new Releve
            {
                IdCapteur = capteur.IdCapteur,
                TypeCapteur = capteur.TypeCapteur[m_Rand.Next(capteur.TypeCapteur.Count)],
                Note = "",
                DateReleve = f_dateReleve,
                Valeur = m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26)
            };
            var newReleves = new List<Releve> {releve1};
            return newReleves;
        }
        private static IEnumerable<VersionProtocole> CreateNewVersionProtocoles(){
            VersionProtocole VersionProtocole1 = new VersionProtocole
            {
                Version = 2,
                Message = new List<MessageModel>(),
            };
            VersionProtocole1.Message.Add
            (new MessageModel{
                TypeMessage = 1,
                PayloadParam = new List<PayloadParamModel>(),
            });
            VersionProtocole1.Message.Add
            (new MessageModel{
                TypeMessage = 2,
                PayloadParam = new List<PayloadParamModel>(),
            });
            VersionProtocole1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Taille = 36
                }
            );
            VersionProtocole1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Taille = 16
                }
            );
            VersionProtocole1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "1",
                    Taille = 200
                }
            );
            VersionProtocole1.Message[0].PayloadParam.Add(new PayloadParamModel
                {
                    Type = "3",
                    Taille = 20
                }
            );
            var newVersionProtocoles = new List<VersionProtocole> {VersionProtocole1};
            return newVersionProtocoles;
        }
        private static IEnumerable<Plantes> CreateNewPlantes(){
            Plantes Plante1 = new Plantes
            {
                Nom = "Exemple",
                Description = "Bonjour je suis un exemple",
                PosX = 1,
                PosY = 5,
                LinkImg = "https://www.andre-briant.fr/media/polystichum_polyblepharum__023010700_1629_24042016.jpg",
            };
            var newPlantes = new List<Plantes> {Plante1};
            return newPlantes;
        }
        private static IEnumerable<Event> CreateNewEvents(){
            Event Event1 = new Event
            {
                Nom = "VENEZ AU LASER GAME !!!!",
                DateEvent = 1559689200,
                DateDebut = 1559676600,
                DateFin = 1559689200,
                Data = new List<DataModel>(),
                Position = 4,
            };
            Event1.Data.Add
            (new DataModel{
                LinkImg = "https://i2.cdscdn.com/pdt2/4/9/2/1/700x700/auc2009459774492/rw/stickers-citron-rigolo-sens-inverse-30-x-30-cm.jpg",
                LinkVideo = "https://www.youtube.com/watch?v=3q7oJuyy5Ac",
                Texte = "Ceci est un exemple d'event"
            });
            var newEvents = new List<Event> {Event1};
            return newEvents;
        }
        private static IEnumerable<User> CreateNewUsers(){
            User User1 = new User
            {
                Nom = "User",
                Mdp = "User",
                UtilisateurHololens = "Billy",
            };
            var newUsers = new List<User> {User1};
            return newUsers;
        }
        private static IEnumerable<Social> CreateNewSocial(){
            Social Social1 = new Social
            {
                Username = "Exemple",
                PageWidget = "Exemple",
                Widget = "http://poulespondeuses.com/wp-content/uploads/2019/02/Poussin-Muesli.jpg",
            };
            var newSocials = new List<Social> {Social1};
            return newSocials;
        }
        private static IEnumerable<Tableau> CreateNewTableaux(){
            Tableau Tableau1 = new Tableau
            {
                DureeAffichage = 10,
                EstAffiche = true,
                Nom = "Exemple",
                DureeCarroussel = 20,
            };
            var newTableaus = new List<Tableau> {Tableau1};
            return newTableaus;
        }
        private static IEnumerable<Media> CreateNewMedias(){
            Media Media1 = new Media
            {
                Nom = "Exemple",
                DateDeb = 1559689243,
                DateFin = 1559689946,
                Data = new List<DataModel>(),
            };
            Media1.Data.Add
            (new DataModel{
                LinkImg = "https://i2.cdscdn.com/pdt2/4/9/2/1/700x700/auc2009459774492/rw/stickers-citron-rigolo-sens-inverse-30-x-30-cm.jpg",
                LinkVideo = "https://www.youtube.com/watch?v=3q7oJuyy5Ac",
                Texte = "Exemple"
            });
            var newMedias = new List<Media> {Media1};
            return newMedias;
        }
        private static IEnumerable<CompteARebours> CreateNewCompteARebours(){
            CompteARebours CompteARebours1 = new CompteARebours
            {
                Nom = "Exemple",
                Texte = "Exemple",
                DateButoir = 1559689946,
                DateDebut = 1559689243,
                DateFin = 1559689946,
            };
            /* 
            CompteARebours1.Data.Add
            (new Data{
                linkImg = "http://fr.web.img4.acsta.net/c_216_288/pictures/19/04/25/17/17/5767838.jpg",
                linkVideo = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                texte = "Exemple"
            });*/
            var newCompteARebours = new List<CompteARebours> {CompteARebours1};
            return newCompteARebours;
        }
    }
}