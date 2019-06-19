/*################################################################################################################################*/
/*   Name: sensorchecking programm                                                                                                */
/*   Goal: check sensor operation, identify defected sensor, create alerts, manage alerts, check  battery level                   */
/*   How to use this programm: execute this code on the server, this program runs indefinitely                                    */
/*   Name and project date: Murs Vegetal may/june 2019                                                                            */
/*   Project group: Mongo/ Big Data                                                                                               */
/*   Creator: Desmullier Gabriel                                                                                                  */
/*   With the participation of: Verept Alexandre,                                                                                 */
/*################################################################################################################################*/

using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using Tests; /*outilsmongoC#*/
using CapteursApi.Models; /*classes de la Bdd*/




namespace Capteurdefectueux
{
    class Program

    {
        static MongoCRUD m_CRUD;
        static Random m_Rand;
        static MongoClient m_Client;
        static IMongoDatabase m_Database;

        static void Main(string[] args)
        {
            /*connection to database*/
            Console.WriteLine("Connexion à la base de donnee / Connection to the database");
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); //"mongodb://10.127.0.81/MurVegetalDb"
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
            Console.WriteLine("Connexion effectuee / Made connection ");

            Console.WriteLine("Debut de la configuration du programme / Start of the program configuration");
            /*turnAroundTime:  the time between two system checks */
            int turnAroundTime = 0;
            Console.WriteLine("Entrez un nombre pour le temps d'execution du programme, c'est a dire le temps entre deux verification du systeme. Ce temps est en secondes. / Enter a number for the execution time of the program, that is, the time between two system checks. This time is in seconds:");
            turnAroundTime = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*tolerance: this is the tolerance threshold, number of missing statements acceptable*/
            int toleranceThreshold = 0; /* Tolerance: nombre de releve non recues avant de considerer le capteur defecteux*/
            Console.WriteLine("Entrez un nombre pour le seuil de tolerance, c'est a dire a partir de combien de releves manquant un capteur est declare defectueux / Enter a number for the tolerance threshold, that is to say from how many missing readings a sensor is declared defective: ");
            toleranceThreshold = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*death: this is the death threshold, number of missing statements acceptable before delete from database*/
            int deathThreshold = 0; /* death: nombre de releve non recues avant de considerer le capteur mort et de la retirer de la BDD*/
            Console.WriteLine("Entrez un nombre pour le seuil de mort, c'est a dire a partir de combien de releves manquant un capteur est declare mort / Enter a number for the death threshold, that is to say from how many missing readings a sensor is declared dead and deleted from the database: ");
            deathThreshold = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");


            /*alertUpdateTime: intervalle temps maximum au bout duquel un message d'alerte reste dans la base de donnee / maximum time interval at the end of which an alert message remains in the database*/
            int alertUpdateTime = 0;
            Console.WriteLine("Entrez un nombre pour l'intervalle de temps maximum au bout duquel un message d'alerte reste dans la base de donnee / Enter a number for the maximum time interval at the end of which an alert message remains in the database:");
            alertUpdateTime = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*goodAlertTime: les bonnes alertes sont les alertes qui indiquent que la batterie a ete recharge ou qu'un capteur est de nouveau operationnel / the right alerts are alerts that indicate that the battery has been recharged or that a sensor is operational again */
            int goodAlertTime = 0;
            Console.WriteLine("Entrez un nombre pour le delai d'affichage des bonnes alertes, les bonnes alertes sont les alertes qui indiquent que la batterie a ete recharge ou qu'un capteur est de nouveau operationnel / Enter a number for the maximum time interval of display of a good alert,  the right alerts are alerts that indicate that the battery has been recharged or that a sensor is operational again ");
            goodAlertTime= int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*quelques variables utiles*/
            int n = 0;
            long timenow = 0;
            long timexec = 0;
            long timelimit = 0;
            long timedeath = 0;
            bool alertBattery = false;

            Console.WriteLine("\n Fin de la configuration, mise en marche / End of configuration, start ");



            while (n == 0) /*boucle infinie*/ /*infinite loop: this program is voluntarily endless*/
            {
                timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                timexec = timenow + turnAroundTime; /*temps de la prochaine execution du programme toutes les heures*/
                                        /*time of the next execution of this algorithm*/
                while (timenow < timexec) /*on attend*/ /*waiting of the next execution of the algorithm*/
                {
                    timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                }

                /*Tri des alertes*/
                Console.WriteLine("Debut de la mise a jour des alertes/ update of alerts begins");
                var alertlist = m_CRUD.LoadRecords<Alerte>("Alertes"); /*je recupere les alertes*/ /*retrievering of the alerts from the database*/
                foreach(var alert in alertlist)
                {
                    
                    /*les erreurs vieillent de plus de alertUpdateTime sont supprimes*/
                    if (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - alertUpdateTime)
                    {
                        m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id);
                    }
                    else
                    {

                        /*les messages d'alerte back online sont supprime au bout de trois jours*/
                        if ((alert.RaisonAlerte == "De nouveau operationnel / Back online") && (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - goodAlertTime))
                        {
                            m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id);
                        }
                        /*les messages attention batterie sont supprimes si le niveau de batterie est de nouveau plein, une alerte batterie remise est ajoute pour 3 jours*/
                        if (alert.RaisonAlerte == "Attention Batterie faible/ Careful low battery")
                        {
                            List<Capteur> alertsensor = m_CRUD.LoadRecordByParameter<Capteur, int>("Capteurs", "IdCapteur", alert.IdCapteur);
                            foreach (var sensor in alertsensor) /*La fonction LoadRecord renvoie une liste mais il n'y a normalement qu'un seul capteur dedans*/
                            {
                                if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] > 20)
                                {
                                    m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id); /*je supprime l'alerte Attention batterie*/

                                    Alerte AlerteBatterieRechargee = new Alerte   /*Une alerte qui indique que la batterie a ete rechargee est ajoutee a la liste des alertes*/
                                    {
                                        IdCapteur = sensor.IdCapteur,
                                        Nom = sensor.Nom,
                                        DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        Fonctionne = true,
                                        RaisonAlerte = "Batterie Rechargee / Recharged Battery"
                                    };
                                    m_CRUD.InsertRecord<Alerte>("Alertes", AlerteBatterieRechargee);
                                    Console.WriteLine($"Le capteur {sensor.Nom} d'id {sensor.IdCapteur} a sa batterie rechargee/ The sensor {sensor.Nom} id {sensor.IdCapteur} has his battery recharged");
                                }
                            }

                        }
                        if ((alert.RaisonAlerte == "Batterie Rechargee / Recharged Battery") && (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - goodAlertTime))
                        {
                            m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id);
                        }
                    }


                }
                Console.WriteLine("\nAlertes mises a jour/ alerts updated");





                /*declenchement de la verification des capteurs*/ /*starting*/
                Console.WriteLine("\nDebut de la verification des capteurs/ Sensor verification begins");


                var sensorlist = m_CRUD.LoadRecords<Capteur>("Capteurs"); /*je recupere les capteurs*/ /*retrievering of the sensors from the database*/
                foreach (var sensor in sensorlist) /*we will check all sensors one by one*/
                {
                    Console.WriteLine($"\ntest de {sensor.Nom}");

                    alertBattery = false; /*indicateur de disfonctionnement du a la batterie, utilise pour eviter d'afficher une alerte changement de batterie s'il y a deja une alerte plus de batterie*/


                    if (sensor.DateDernierReleve == 0)     /*si le capteur n'a jamais au grand jamais envoye de donnees*/ /*if the sensor had not yet send statement*/
                    {

                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/


                        if (sensor.DateCapteur + sensor.DelaiVeille * deathThreshold < timenow) /*si un capteur n'est pas neuf et n'a pas envoye de releve depuis tres longtemps*/
                        {
                            Alerte AlerteDeath = new Alerte
                            {
                                IdCapteur = sensor.IdCapteur,
                                Nom = sensor.Nom,
                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Fonctionne = false,
                                RaisonAlerte = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                            };
                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteDeath);
                            Console.WriteLine($"Alerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                            alertBattery = true;
                            m_CRUD.DeleteRecord<Capteur>("Capteurs", sensor.Id);
                        }

                        else
                        {
                            if (sensor.DateCapteur + sensor.DelaiVeille * toleranceThreshold < timenow) /*s'il n'est pas neuf il est considere comme non fonctionnel */ /*if the sensor isn't new, it is defected*/
                            {
                                sensor.Fonctionne = false;
                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur} id defected");




                                /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                                /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/
                                if (sensor.Batterie == true)
                                {
                                    if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)
                                    {

                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte,int,string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Plus de Batterie/ No more Battery");
                                        if (previousAlert == null)
                                        {
                                            Alerte AlerteBatterie = new Alerte
                                            {
                                                IdCapteur = sensor.IdCapteur,
                                                Nom = sensor.Nom,
                                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                Fonctionne = false,
                                                RaisonAlerte = "Plus de Batterie/ No more Battery"
                                            };
                                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteBatterie);
                                        }
                                        Console.WriteLine($"Alerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                        alertBattery = true;
                                    }
                                    else
                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Erreur de fonctionnement/ Operating error");
                                        if (previousAlert == null)
                                        {
                                            Alerte AlerteFonctionnement = new Alerte
                                            {
                                                IdCapteur = sensor.IdCapteur,
                                                Nom = sensor.Nom,
                                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                Fonctionne = false,
                                                RaisonAlerte = "Erreur de fonctionnement/ Operating error"
                                            };
                                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteFonctionnement);
                                        }
                                        Console.WriteLine($"Alerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");

                                    }
                                }
                                else
                                {
                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Erreur de fonctionnement/ Operating error");
                                    if (previousAlert == null)
                                    {
                                        Alerte AlerteFonctionnement = new Alerte
                                        {
                                            IdCapteur = sensor.IdCapteur,
                                            Nom = sensor.Nom,
                                            DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            Fonctionne = false,
                                            RaisonAlerte = "Erreur de fonctionnement/ Operating error"
                                        };
                                        m_CRUD.InsertRecord<Alerte>("Alertes", AlerteFonctionnement);
                                    }
                                    Console.WriteLine($"Alerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                }


                            }
                            else { Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien/ the sensor {sensor.Nom} id {sensor.IdCapteur} is functional"); }

                        }
                    }
                    else
                    {       /*s'il a deja donne des releves*/ /*if the sensor had already send a statement*/
                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                        timelimit = timenow - sensor.DelaiVeille * toleranceThreshold;
                        timedeath = timenow - sensor.DelaiVeille * deathThreshold;

                        /*if the sensor has not sent a statement for a long time, it become dead and it is deleted from the database*/
                        if (sensor.DateDernierReleve < timedeath)
                        { /*si le capteur n'a pas envoye de releve depuis treeeeeeeeees longtemps, il est mort et est retire de la BDD*/

                            Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} est mort il a ete supprime de la BDD/ the sensor {sensor.Nom} id {sensor.IdCapteur} is dead and is deleted from the database");
                            Alerte AlerteDeath = new Alerte
                            {
                                IdCapteur = sensor.IdCapteur,
                                Nom = sensor.Nom,
                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Fonctionne = false,
                                RaisonAlerte = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                            };
                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteDeath);
                            Console.WriteLine($"Alerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                            alertBattery = true;
                            m_CRUD.DeleteRecord<Capteur>("Capteurs", sensor.Id);


                        }
                        else
                        {
                            if (sensor.DateDernierReleve < timelimit) /*if the last statement is older than the timelimit, it is considered as defected*/
                            { /*si le dernier releve est plus age que notre limite timelimite il est considere comme defectueux*/
                                sensor.Fonctionne = false;
                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur} is defected");

                                /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                                /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/


                                if (sensor.Batterie == true)
                                {
                                    if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)

                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Plus de Batterie/ No more Battery");
                                        if (previousAlert.Count == 0)
                                        {
                                            Alerte AlerteBatterie = new Alerte
                                            {
                                                IdCapteur = sensor.IdCapteur,
                                                Nom = sensor.Nom,
                                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                Fonctionne = false,
                                                RaisonAlerte = "Plus de Batterie/ No more Battery"
                                            };
                                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteBatterie);
                                        }
                                        Console.WriteLine($"Alerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                        alertBattery = true;
                                    }
                                    else
                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Erreur de fonctionnement/ Operating error");
                                        if (previousAlert.Count == 0)
                                        {

                                            Alerte AlerteFonctionnement = new Alerte
                                            {
                                                IdCapteur = sensor.IdCapteur,
                                                Nom = sensor.Nom,
                                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                Fonctionne = false,
                                                RaisonAlerte = "Erreur de fonctionnement/ Operating error"
                                            };
                                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteFonctionnement);
                                        }
                                        Console.WriteLine($"Alerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");

                                    }
                                }
                                else
                                {

                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Erreur de fonctionnement/ Operating error");
                                    if (previousAlert.Count== 0)
                                    {
                                        Alerte AlerteFonctionnement = new Alerte
                                        {
                                            IdCapteur = sensor.IdCapteur,
                                            Nom = sensor.Nom,
                                            DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            Fonctionne = false,
                                            RaisonAlerte = "Erreur de fonctionnement/ Operating error"
                                        };
                                        m_CRUD.InsertRecord<Alerte>("Alertes", AlerteFonctionnement);
                                    }
                                    Console.WriteLine($"Alerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}/ Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                }
                            }
                            else /*si un capteur a bien donne des releves*/ /*if the sensor had send recently a statement*/
                            {
                                sensor.Fonctionne = true;

                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                if (sensor.Fonctionne == false)/*si un capteur mort redonne des releves on peut le reclasser parmis les vivants*/ /*if a defected gives back statement, it is again functional*/
                                {
                                    sensor.Fonctionne = true;
                                    m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                    Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} n'est plus defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur}is not defective any more");

                                    /*Une alerte qui indique le retour du capteur est ajoutee aux alertes*/
                                    /*An alert indicating the return of the sensor is added to the alerts*/
                                    Alerte AlerteRetour = new Alerte
                                    {
                                        IdCapteur = sensor.IdCapteur,
                                        Nom = sensor.Nom,
                                        DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        Fonctionne = true,
                                        RaisonAlerte = "De nouveau operationnel/ Back online"
                                    };
                                    m_CRUD.InsertRecord<Alerte>("Alertes", AlerteRetour);
                                    Console.WriteLine($"Le capteur {sensor.Nom} d'id {sensor.IdCapteur} est de nouveau operationnel/ The sensor {sensor.Nom} id {sensor.IdCapteur} is back online");

                                    /*Les alertes fonctionnement ou batterie precedentes sont supprimes*/
                                    /*Previous operation or battery alerts are deleted*/
                                    List<Alerte> VieilleAlertes = m_CRUD.LoadRecordByParameter<Alerte, int>("Alerte", "IdCapteur", sensor.IdCapteur);
                                    foreach (var VieilleAlerte in VieilleAlertes)
                                    {
                                        if ((VieilleAlerte.RaisonAlerte == "Erreur de fonctionnement/ Operating error") || (VieilleAlerte.RaisonAlerte == "Plus de Batterie/ No more Battery"))
                                        {
                                            m_CRUD.DeleteRecord<Alerte>("Alertes", VieilleAlerte.Id);

                                        }
                                    }

                                }
                                else /*si tout va bien*/ /*if all goes well*/
                                {
                                    Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien/ the sensor {sensor.Nom} id {sensor.IdCapteur} is functional");
                                }
                            }
                        }

                    }


                    /*verification du niveau de batterie, alerte l'utilisateur s'il faut changer la batterie*/
                    if ((alertBattery==false)&(sensor.Batterie == true))
                    {
                        if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)
                        {

                            /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                            List<Alerte> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerte, int, string>("Alertes", "IdCapteur", sensor.IdCapteur, "RaisonAlerte", "Attention Batterie faible/ Careful low battery");
                            if (previousAlert.Count == 0)
                            {
                                Alerte AlerteBatterie = new Alerte
                                {
                                    IdCapteur = sensor.IdCapteur,
                                    Nom = sensor.Nom,
                                    DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    Fonctionne = true,
                                    RaisonAlerte = "Attention Batterie faible/ Careful low battery"
                                };
                                m_CRUD.InsertRecord<Alerte>("Alertes", AlerteBatterie);
                            }
                            Console.WriteLine($"Attention la Batterie du capteur{sensor.Nom} d'id {sensor.IdCapteur} est presque dechargee ({sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1]})/ Careful the Battery of the sensor {sensor.Nom} id {sensor.IdCapteur} is runnig out of power ({sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1]})");
                        }
                    }


                } Console.WriteLine("tous les capteurs ont ete verifies/ all sensors have been checked\n##################################################\n");
            }
        }

      
    }
}
