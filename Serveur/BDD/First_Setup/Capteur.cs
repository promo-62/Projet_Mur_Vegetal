using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Setup
{
    public class ICollectionModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

    public class Capteurs : ICollectionModel
    {
        [BsonElement("IdCapteur")]
        public int IdCapteur { get; set; }

        [BsonElement("TypeCapteur")]
        public int TypeCapteur { get; set; }

        [BsonElement("Projet")]
        public List<string> Projet { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("DateCapteur")]
        public long DateCapteur { get; set; }

        [BsonElement("DateDernierReleve")]
        public long DateDernierReleve { get; set; }

        [BsonElement("Batterie")]
        public bool Batterie { get; set; }

        [BsonElement("NiveauBatterie")]
        public List<int> NiveauBatterie { get; set; }

        [BsonElement("DelaiVeille")]
        public int DelaiVeille { get; set; }

        [BsonElement("Action")]
        public List<ActionModel> Action { get; set; }

        [BsonElement("Version")]
        public int Version { get; set; }

        [BsonElement("Fonctionne")]
        public bool Fonctionne { get; set; }

        [BsonElement("Timeout")]
        public int Timeout { get; set; }
    }

    public class CapteursWeb : ICollectionModel
    {
        [BsonElement("IdCapteur")]
        public int IdCapteur { get; set; }

        [BsonElement("TypeCapteur")]
        public int TypeCapteur { get; set; }

        [BsonElement("Projet")]
        public List<string> Projet { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }
    }

    public class ActionModel
    {
        [BsonElement("Data")]
        public int Data { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }
    }

    public class Releves : ICollectionModel
    {
        [BsonElement("IdCapteur")]
        public int IdCapteur { get; set; }

        [BsonElement("Note")]
        public string Note { get; set; }

        [BsonElement("DateReleve")]
        public long DateReleve { get; set; }

        [BsonElement("Valeurs")]
        public List<int> Valeurs { get; set; }
    }

    public class RelevesWeb : ICollectionModel
    {
        [BsonElement("IdCapteur")]
        public int IdCapteur { get; set; }

        [BsonElement("Valeurs")]
        public List<int> Valeurs { get; set; }
    }

    public class VersionsProtocoles : ICollectionModel
    {
        [BsonElement("Version")]
        public int Version { get; set; }

        [BsonElement("Message")]
        public List<MessageModel> Message { get; set; }
    }

    public class MessageModel
    {
        [BsonElement("TypeMessage")]
        public int TypeMessage { get; set; }

        [BsonElement("Message")]
        public List<PayloadParamModel> PayloadParam { get; set; }
    }

    public class PayloadParamModel
    {
        [BsonElement("Type")]
        public string Type { get; set; }

        [BsonElement("Taille")]
        public int Taille { get; set; }
    }

    public class Plantes : ICollectionModel
    {
        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("PosX")]
        public int PosX { get; set; }

        [BsonElement("PosY")]
        public int PosY { get; set; }

        [BsonElement("LinkImg")]
        public string LinkImg { get; set; }
    }

    public class Events : ICollectionModel
    {
        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("DateEvent")]
        public long DateEvent { get; set; }

        [BsonElement("DateDebut")]
        public long DateDebut { get; set; }

        [BsonElement("DateFin")]
        public long DateFin { get; set; }

        [BsonElement("Data")]
        public List<DataModel> Data { get; set; }

        [BsonElement("Position")]
        public int Position { get; set; }
    }

    public class DataModel
    {
        [BsonElement("LinkImg")]
        public string LinkImg { get; set; }

        [BsonElement("LinkVideo")]
        public string LinkVideo { get; set; }

        [BsonElement("Texte")]
        public string Texte { get; set; }
    }

    public class UsersHololens : ICollectionModel
    {
        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("Mdp")]
        public string Mdp { get; set; }

        [BsonElement("UtilisateurHololens")]
        public string UtilisateurHololens { get; set; }
    }

    public class UsersAdmin : ICollectionModel
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PwdHash")]
        public string PwdHash { get; set; }

        [BsonElement("CleHash")]
        public string CleHash { get; set; }
    }

    public class UsersAPI : ICollectionModel
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Salt")]
        public string Salt { get; set; }

        [BsonElement("NiveauAccreditation")]
        public int NiveauAccreditation { get; set; }
    }

    public class Socials : ICollectionModel
    {
        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PageWidget")]
        public string PageWidget { get; set; }

        [BsonElement("Widget")]
        public string Widget { get; set; }
    }

    public class Tableaux : ICollectionModel
    {
        [BsonElement("DureeAffichage")]
        public int DureeAffichage { get; set; }

        [BsonElement("EstAffiche")]
        public bool EstAffiche { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("DureeCarroussel")]
        public int DureeCarroussel { get; set; }
    }

    public class Medias : ICollectionModel
    {
        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("DateDeb")]
        public long DateDeb { get; set; }

        [BsonElement("DateFin")]
        public long DateFin { get; set; }

        [BsonElement("Data")]
        public List<DataModel> Data { get; set; }
    }

    public class ComptesARebours : ICollectionModel
    {
        [BsonElement("Texte")]
        public string Texte { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("DateButoir")]
        public long DateButoir { get; set; }

        [BsonElement("DateDebut")]
        public long DateDebut { get; set; }

        [BsonElement("Pos")]
        public int Pos { get; set; }

        [BsonElement("DateFin")]
        public long DateFin { get; set; }
    }

    public class TypesCapteurs : ICollectionModel
    {
        [BsonElement("TypeCapteur")]
        public int TypeCapteur { get; set; }

        [BsonElement("NomCapteur")]
        public string NomCapteur { get; set; }
    }

    public class Alertes : ICollectionModel
    {
        [BsonElement("IdCapteur")]
        public int IdCapteur { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("DateAlerte")]
        public long DateAlerte { get; set; }

        [BsonElement("Fonctionne")]
        public bool Fonctionne { get; set; }

        [BsonElement("RaisonAlerte")]
        public string RaisonAlerte { get; set; }
    }

}