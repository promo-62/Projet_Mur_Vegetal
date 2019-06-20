using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson; ///Utilise pour le ObjectId

//////////////////////////////////////////////////////////////////////////////////
/// Setup d'exemple dans MurVegetalDb : certains sont les hard codes et d'autres sont
/// pseudo generes. DELETE L'ANCIENNE BASE DE DONNEE AVANT ! ATTENTION !!!
/// Example setup on MurVegetalDb : some of them are hard coded while others are
/// pseudo generated. DELETE THE OLD DATA BASE BEFORE !!! WARNING !!!!
//////////////////////////////////////////////////////////////////////////////////

namespace Setup
{   
    public class CapteurComparer : Comparer<Capteurs> 
    {
        // Compares by Length, Height, and Width.
        public override int Compare(Capteurs x, Capteurs y)
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
            addNewDocuments<TypesCapteurs>(CreateNewTypeCapteur(), "TypesCapteurs").Wait();
            addNewDocuments<Capteurs>(CreateNewCapteur(), "Capteurs").Wait();
            addNewDocuments<Releves>(CreateNewReleve(), "Releves").Wait();
            addNewDocuments<VersionsProtocoles>(CreateNewVersionProtocoles(), "VersionsProtocoles").Wait();
            addNewDocuments<Plantes>(CreateNewPlantes(), "Plantes").Wait();
            addNewDocuments<Events>(CreateNewEvents(), "Events").Wait();
            addNewDocuments<UsersHololens>(CreateNewUsersHololens(), "UsersHololens").Wait();
            addNewDocuments<UsersAdmin>(CreateNewUsersAdmin(), "UsersAdmin").Wait();
            addNewDocuments<UsersAPI>(CreateNewUsersAPI(), "UsersAPI").Wait();
            addNewDocuments<Socials>(CreateNewSocial(), "Socials").Wait();
            addNewDocuments<Tableaux>(CreateNewTableaux(), "Tableaux").Wait();
            addNewDocuments<Medias>(CreateNewMedias(), "Medias").Wait();
            addNewDocuments<ComptesARebours>(CreateNewComptesARebours(), "ComptesARebours").Wait();  
            addNewDocuments<Alertes>(CreateNewAlertes(), "Alertes").Wait();  
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
        private static IEnumerable<TypesCapteurs> CreateNewTypeCapteur()
        {
            List<string> typesCapteur = new List<string>{"Humidite", "Temperature", "Luminosite", "QualiteDAir", "Pression"};
            
            var NewTypeCapteur = new List<TypesCapteurs>();
            for(int i = 0; i < typesCapteur.Count; i++)
            {
                NewTypeCapteur.Add( new TypesCapteurs
                {
                    TypeCapteur = i,
                    NomCapteur = typesCapteur[i]
                });

            }
            return NewTypeCapteur;
        }
        
        private static IEnumerable<Capteurs> CreateNewCapteur()
        {

            var NewCapteur = new List<Capteurs>();
            for(int i = 0; i < 10; i++)
            {
                bool f_fonctionne = true;
                if(m_Rand.Next(8) == 0)
                    f_fonctionne = false;
                NewCapteur.Add( new Capteurs
                {
                    IdCapteur = i,
                    TypeCapteur = m_Rand.Next(5),
                    Projet = new List<string>{"MurVegetal"},
                    Nom = "",
                    Description = "",
                    DateCapteur = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000),
                    DateDernierReleve = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Batterie = true,
                    NiveauBatterie = new List<int>(),
                    DelaiVeille = 10,
                    Action = new List<ActionModel>(),
                    Version = 3,
                    Fonctionne = f_fonctionne,
                    Timeout = 0
                });
                NewCapteur[i].NiveauBatterie.Add(100);
                int max = m_Rand.Next(40);
                for(int j = 0; j < max; j++)
                {
                    int newBatteryLvl = NewCapteur[i].NiveauBatterie[j] - m_Rand.Next(4);
                    if(newBatteryLvl < 0)
                        newBatteryLvl = 0;
                    NewCapteur[i].NiveauBatterie.Add(newBatteryLvl);
                }
                if(m_Rand.Next(2) == 0)
                    NewCapteur[i].Action.Add
                    (new ActionModel{
                        Nom = "LedDelai",
                        Description = "Delai du clignotement de la led d'etat en secondes.",
                        Data = 10
                    });
            }
            return NewCapteur;
        }
        
        private static IEnumerable<Releves> CreateNewReleve()
        {
            List<Capteurs> capteurs = m_CRUD.LoadRecords<Capteurs>("Capteurs");
            var newReleves = new List<Releves> {};
            int nbReleve = m_Rand.Next(50) + 50;
            for(int i = 0; i < nbReleve; i++)
            {
                int idCapteur = m_Rand.Next(5);
                long f_dateReleve = DateTimeOffset.Now.ToUnixTimeSeconds() - m_Rand.Next(100000);
                if(capteurs[idCapteur].DateDernierReleve < f_dateReleve)
                    capteurs[idCapteur].DateDernierReleve = f_dateReleve;
                    
                m_CRUD.UpsetRecord<Capteurs>("Capteurs", ObjectId.Parse(capteurs[idCapteur].Id), capteurs[idCapteur]);
                newReleves.Add(new Releves
                {
                    IdCapteur = capteurs[idCapteur].IdCapteur,
                    DateReleve = f_dateReleve,
                    Valeurs = new List<int>()
                });
                int r = m_Rand.Next(2) + 1;
                for(int j = 0; j < r; j++)
                {
                    newReleves[newReleves.Count - 1].Valeurs.Add(m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26) + m_Rand.Next(26));
                }
            }
            
            return newReleves;
        }
        private static IEnumerable<VersionsProtocoles> CreateNewVersionProtocoles(){
            VersionsProtocoles VersionProtocole1 = new VersionsProtocoles
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
            var newVersionProtocoles = new List<VersionsProtocoles> {VersionProtocole1};
            return newVersionProtocoles;
        }
        private static IEnumerable<Plantes> CreateNewPlantes(){
            Plantes Plante1 = new Plantes
            {
                Nom = "polystichum setiferum proliferum",
                Description = "Bonjour je suis une polystichum setiferum proliferum",
                PosX = 1,
                PosY = 5,
                LinkImg = "https://www.andre-briant.fr/media/polystichum_polyblepharum__023010700_1629_24042016.jpg",
            };
            var newPlantes = new List<Plantes> {Plante1};
            return newPlantes;
        }
        private static IEnumerable<Events> CreateNewEvents(){
            Events Event1 = new Events
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
                Texte = "TOP 10 DES TRUCS LES PLUS DROLES D INTERNET LE TROISIEME VA VOUS RENDRE FOU !!!"
            });
            var newEvents = new List<Events> {Event1};
            return newEvents;
        }
        private static IEnumerable<UsersHololens> CreateNewUsersHololens(){
            UsersHololens User1 = new UsersHololens
            {
                Nom = "User",
                Mdp = "User",
                UtilisateurHololens = "Billy"
            };
            var newUsers = new List<UsersHololens> {User1};
            return newUsers;
        }
        private static IEnumerable<UsersAdmin> CreateNewUsersAdmin(){
            UsersAdmin User1 = new UsersAdmin
            {
                Username = "UserLambda",
                PwdHash = "TaMaman",
                CleHash = "BobbyBob"
            };
            var newUsers = new List<UsersAdmin> {User1};
            return newUsers;
        }
        private static IEnumerable<UsersAPI> CreateNewUsersAPI(){
            UsersAPI User1 = new UsersAPI
            {
                Username = "JeanClaudeVendamne",
                Password = "CeciEstUnMotDePasseHacheDoncVoila",
                Salt = "azerty",
                NiveauAccreditation = 3
            };
            UsersAPI User2 = new UsersAPI
            {
                Username = "Un mec au pif",
                Password = "CeciEstUnMotDePasseHacheDoncVoilou",
                Salt = "azerty",
                NiveauAccreditation = 1
            };
            var newUsers = new List<UsersAPI> {User1};
            return newUsers;
        }
        private static IEnumerable<Socials> CreateNewSocial(){
            Socials Social1 = new Socials
            {
                Username = "MurVegetal",
                PageWidget = "accueil",
                Widget = "http://poulespondeuses.com/wp-content/uploads/2019/02/Poussin-Muesli.jpg",
            };
            var newSocials = new List<Socials> {Social1};
            return newSocials;
        }
        private static IEnumerable<Tableaux> CreateNewTableaux(){
            Tableaux Tableau1 = new Tableaux
            {
                DureeAffichage = 10,
                EstAffiche = true,
                Nom = "LA Joconde",
                DureeCarroussel = 20,
            };
            var newTableaux = new List<Tableaux> {Tableau1};
            return newTableaux;
        }
        private static IEnumerable<Medias> CreateNewMedias(){
            Medias Media1 = new Medias
            {
                Nom = "BFMTV",
                DateDeb = 1559689243,
                DateFin = 1559689946,
                Data = new List<DataModel>(),
            };
            Media1.Data.Add
            (new DataModel{
                LinkImg = "https://i2.cdscdn.com/pdt2/4/9/2/1/700x700/auc2009459774492/rw/stickers-citron-rigolo-sens-inverse-30-x-30-cm.jpg",
                LinkVideo = "https://www.youtube.com/watch?v=3q7oJuyy5Ac",
                Texte = "TOP 10 DES TRUCS LES PLUS DROLES D INTERNET LE TROISIEME VA VOUS RENDRE FOU !!!"
            });
            var newMedias = new List<Medias> {Media1};
            return newMedias;
        }
        private static IEnumerable<ComptesARebours> CreateNewComptesARebours(){
            ComptesARebours CompteARebours1 = new ComptesARebours
            {
                Nom = "je suis le car numero 1",
                Texte = "ATTENTION: Il ne reste plus beaucoup de temps pour vous acheter votre abonnement TéléZ",
                DateButoir = 1559689946,
                DateDebut = 1559689243,
                DateFin = 1559689946,
                Pos = m_Rand.Next(203) + 100
            };
            /* 
            CompteARebours1.Data.Add
            (new Data{
                linkImg = "http://fr.web.img4.acsta.net/c_216_288/pictures/19/04/25/17/17/5767838.jpg",
                linkVideo = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                texte = "IL MONTE SUR UNE GRUE (l oiseau) ET CA TOURNE MAL OMG !!!!"
            });*/
            var newCompteARebours = new List<ComptesARebours> {CompteARebours1};
            return newCompteARebours;
        }
        private static IEnumerable<Alertes> CreateNewAlertes(){
            Alertes Alerte1 = new Alertes
            {
                IdCapteur = m_Rand.Next(5),
                Nom = "je suis une alerte",
                DateAlerte = 1559689996,
                Fonctionne = true,
                RaisonAlerte = "La raison de l'alerte est encore une demande random de changement de la bdd."
            };
            /* 
            CompteARebours1.Data.Add
            (new Data{
                linkImg = "http://fr.web.img4.acsta.net/c_216_288/pictures/19/04/25/17/17/5767838.jpg",
                linkVideo = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                texte = "IL MONTE SUR UNE GRUE (l oiseau) ET CA TOURNE MAL OMG !!!!"
            });*/
            var newCompteARebours = new List<Alertes> {Alerte1};
            return newCompteARebours;
        }
    }
}