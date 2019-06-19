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
            /*connection to the database MurVegetalDb*/
            Console.WriteLine("Connexion à la base de donnee / Connection to the database");
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); //"mongodb://10.127.0.81/MurVegetalDb"
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
            Console.WriteLine("\nConnexion effectuee / Made connection ");

            Console.WriteLine("\nDebut de la configuration du programme / Start of the program configuration");
            /*turnAroundTime:  the time between two system checks */
            /*turnAroundTime: temps entre deux verification du syteme*/
            int turnAroundTime = 0;
            Console.WriteLine("\nEntrez un nombre pour le temps d'execution du programme, c'est a dire le temps entre deux verification du systeme. Ce temps est en secondes. \nEnter a number for the execution time of the program, that is, the time between two system checks. This time is in seconds:");
            turnAroundTime = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*tolerance: this is the tolerance threshold, number of missing statements acceptable*/
            /* Tolerance: nombre de releve non recues avant de considerer le capteur defecteux*/
            int toleranceThreshold = 0; 
            Console.WriteLine("\nEntrez un nombre pour le seuil de tolerance, c'est a dire a partir de combien de releves manquant un capteur est declare defectueux \nEnter a number for the tolerance threshold, that is to say from how many missing readings a sensor is declared defective: ");
            toleranceThreshold = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*death: this is the death threshold, number of missing statements acceptable before delete from database*/
            /* death: nombre de releve non recues avant de considerer le capteur mort et de la retirer de la BDD*/
            int deathThreshold = 0; 
            Console.WriteLine("\nEntrez un nombre pour le seuil de mort, c'est a dire a partir de combien de releves manquant un capteur est declare mort \nEnter a number for the death threshold, that is to say from how many missing readings a sensor is declared dead and deleted from the database: ");
            deathThreshold = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");


            /*alertUpdateTime: intervalle temps maximum au bout duquel un message d'alerte reste dans la base de donnee */ 
            /*alertUpdateTime: maximum time interval at the end of which an alert message remains in the database*/
            int alertUpdateTime = 0;
            Console.WriteLine("\nEntrez un nombre pour l'intervalle de temps maximum au bout duquel un message d'alerte reste dans la base de donnee. Ce temps est en secondes. \nEnter a number for the maximum time interval at the end of which an alert message remains in the database. This time is in seconds:");
            alertUpdateTime = int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*goodAlertTime: les bonnes alertes sont les alertes qui indiquent que la batterie a ete recharge ou qu'un capteur est de nouveau operationnel*/
            /*goodAlertTime: the right alerts are alerts that indicate that the battery has been recharged or that a sensor is operational again */
            int goodAlertTime = 0;
            Console.WriteLine("\nEntrez un nombre pour le delai d'affichage des bonnes alertes, les bonnes alertes sont les alertes qui indiquent que la batterie a ete recharge ou qu'un capteur est de nouveau operationnel. Ce temps est en secondes. \nEnter a number for the maximum time interval of display of a good alert,  the right alerts are alerts that indicate that the battery has been recharged or that a sensor is operational again. This time is in seconds: ");
            goodAlertTime= int.Parse(Console.ReadLine());
            Console.WriteLine("\n");

            /*quelques variables utiles*/
            /*Some useful variables*/
            int n = 0;/*pour la boucle infini / for infinity loop*/
            
            /*variable temporelles utilisees pour l'execution periodique de la verification/ time variable used for the periodic execution of the verification*/
            long timexec = 0;
            long timenow = 0;

            /*variables temporelles utilisees comme critere lors du test des capteurs / time variables used as a screen during sensor testing*/
            long timelimit = 0;
            long timedeath = 0;

            /*utilise pour eviter une repetition d'alertes / used to avoid repetition of alerts*/
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
                Console.WriteLine("\nDebut de la mise a jour des alertes/ update of alerts begins");
                var alertlist = m_CRUD.LoadRecords<Alerte>("Alertes"); /*je recupere les alertes*/ /*retrievering of the alerts from the database*/
                foreach(var alert in alertlist)
                {
                    
                    /*les erreurs vieillent de plus de alertUpdateTime sont supprimes*/
                    /*oldest alerts are deleted depending on the configuration alertUpdateTime*/
                    if (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - alertUpdateTime)
                    {
                        m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id);
                    }
                    else
                    {

                        /*les messages d'alerte back online sont supprime au bout de goodAlertTime*/
                        /*back-online alert messages are deleted after goodAlertTime*/
                        if ((alert.RaisonAlerte == "De nouveau operationnel / Back online") && (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - goodAlertTime))
                        {
                            m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id);
                        }
                        /*les messages attention batterie sont supprimes si le niveau de batterie est de nouveau plein, une alerte batterie remise est ajoute pendant goodAlertTime*/
                        /*the battery attention messages are deleted if the battery level is again full, a reset battery alert is added during goodAlertTime*/
                        if (alert.RaisonAlerte == "Attention Batterie faible/ Careful low battery")
                        {
                            List<Capteur> alertsensor = m_CRUD.LoadRecordByParameter<Capteur, int>("Capteurs", "IdCapteur", alert.IdCapteur);
                            foreach (var sensor in alertsensor) /*La fonction LoadRecord renvoie une liste mais il n'y a normalement qu'un seul capteur dedans*/
                                                                /*The LoadRecord function returns a list but there is normally only one sensor in it*/
                            {
                                if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] > 20) /*nouveau test / New test*/
                                {
                                    m_CRUD.DeleteRecord<Alerte>("Alertes", alert.Id); /*je supprime l'alerte Attention batterie*//*I delete the battery alert*/

                                    Alerte AlerteBatterieRechargee = new Alerte   /*Une alerte qui indique que la batterie a ete rechargee est ajoutee a la liste des alertes*/
                                    {                                            /*An alert indicating that the battery has been recharged is added to the alert list*/
                                        IdCapteur = sensor.IdCapteur,
                                        Nom = sensor.Nom,
                                        DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        Fonctionne = true,
                                        RaisonAlerte = "Batterie Rechargee / Recharged Battery"
                                    };
                                    m_CRUD.InsertRecord<Alerte>("Alertes", AlerteBatterieRechargee);
                                    Console.WriteLine($"\nLe capteur {sensor.Nom} d'id {sensor.IdCapteur} a sa batterie rechargee\n The sensor {sensor.Nom} id {sensor.IdCapteur} has his battery recharged");
                                }
                            }

                        }
                        if ((alert.RaisonAlerte == "Batterie Rechargee / Recharged Battery") && (alert.DateAlerte < DateTimeOffset.Now.ToUnixTimeSeconds() - goodAlertTime)) /*la bonne alerte est supprimee apres goodAlertTime minutes*/
                                                                                                                                                                             /*the good alert is deleted after goodAlertTime minutes*/
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
                    Console.WriteLine($"\n_______________________________________________________________________\ntest de {sensor.Nom}");

                    alertBattery = false; /*indicateur de dysfonctionnement du a la batterie, utilise pour eviter d'afficher une alerte changement de batterie s'il y a deja une alerte plus de batterie*/
                    /*Battery malfunction indicator, used to avoid displaying a battery change alert if there is already a more battery alert*/

                    if (sensor.DateDernierReleve == 0)     /*si le capteur n'a jamais au grand jamais envoye de donnees*/ /*if the sensor had not yet send statement*/
                    {

                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/


                        if (sensor.DateCapteur + sensor.DelaiVeille * deathThreshold < timenow) /*si un capteur n'est pas neuf et n'a pas envoye de releve depuis tres longtemps*/
                        {                                                                       /*if a sensor is not new and has not sent a lift for a long time*/
                            Alerte AlerteDeath = new Alerte
                            {                                 /*une alerte capteur mort est emise*//*an dead sensor alert is create*/
                                IdCapteur = sensor.IdCapteur,
                                Nom = sensor.Nom,
                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Fonctionne = false,
                                RaisonAlerte = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                            };
                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteDeath);
                            Console.WriteLine($"\nAlerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                            alertBattery = true;
                            m_CRUD.DeleteRecord<Capteur>("Capteurs", sensor.Id); /*le capteur mort est supprime de la base de donnees*//*the dead sensor is removed from the database*/
                        }

                        else
                        {
                            if (sensor.DateCapteur + sensor.DelaiVeille * toleranceThreshold < timenow) /*s'il n'est pas neuf il est considere comme non fonctionnel */ /*if the sensor isn't new, it is defected*/
                            {
                                sensor.Fonctionne = false;
                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                Console.WriteLine($"\nle capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux\n the sensor {sensor.Nom} id {sensor.IdCapteur} id defected");




                                /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                                /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/
                                if (sensor.Batterie == true)
                                {
                                    if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)
                                    {

                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
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
                                        Console.WriteLine($"\nAlerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                        alertBattery = true;
                                    }
                                    else
                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
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
                                        Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}/\nOperating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");

                                    }
                                }
                                else
                                {
                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
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
                                    Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                }


                            }
                            else { Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien\n the sensor {sensor.Nom} id {sensor.IdCapteur} is functional"); }

                        }
                    }
                    else
                    {       /*s'il a deja donne des releves*/ /*if the sensor had already send a statement*/
                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                        timelimit = timenow - sensor.DelaiVeille * toleranceThreshold;
                        timedeath = timenow - sensor.DelaiVeille * deathThreshold;

                        /*if the sensor has not sent a statement for a long time, it become dead and it is deleted from the database*/
                        if (sensor.DateDernierReleve < timedeath)
                        { /*si le capteur n'a pas envoye de releve depuis tres longtemps, il est mort et est retire de la BDD*/

                            Console.WriteLine($"\nle capteur {sensor.Nom} d'id {sensor.IdCapteur} est mort il a ete supprime de la BDD\n the sensor {sensor.Nom} id {sensor.IdCapteur} is dead and is deleted from the database");
                            Alerte AlerteDeath = new Alerte
                            {
                                IdCapteur = sensor.IdCapteur,
                                Nom = sensor.Nom,
                                DateAlerte = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                Fonctionne = false,
                                RaisonAlerte = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                            };
                            m_CRUD.InsertRecord<Alerte>("Alertes", AlerteDeath);
                            Console.WriteLine($"\nAlerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                            alertBattery = true;
                            m_CRUD.DeleteRecord<Capteur>("Capteurs", sensor.Id);


                        }
                        else
                        {
                            if (sensor.DateDernierReleve < timelimit) /*if the last statement is older than the timelimit, it is considered as defected*/
                            { /*si le dernier releve est plus age que notre limite timelimite il est considere comme defectueux*/
                                sensor.Fonctionne = false;
                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                Console.WriteLine($"\nle capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux\n the sensor {sensor.Nom} id {sensor.IdCapteur} is defected");

                                /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                                /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/


                                if (sensor.Batterie == true)
                                {
                                    if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)

                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
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
                                        Console.WriteLine($"\nAlerte Batterie capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Battery Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                        alertBattery = true;
                                    }
                                    else
                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
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
                                        Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");

                                    }
                                }
                                else
                                {

                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
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
                                    Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Nom} d'id {sensor.IdCapteur}\n Operating Alert sensor {sensor.Nom} id {sensor.IdCapteur}");
                                }
                            }
                            else /*si un capteur a bien donne des releves*/ /*if the sensor had send recently a statement*/
                            {

                                if (sensor.Fonctionne == false)/*si un capteur defectueux redonne des releves on peut le reclasser parmis les vivants*/ /*if a defected gives back statement, it is again functional*/
                                {
                                    sensor.Fonctionne = true;
                                    m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                    Console.WriteLine($"\nle capteur {sensor.Nom} d'id {sensor.IdCapteur} n'est plus defectueux\n the sensor {sensor.Nom} id {sensor.IdCapteur}is not defective any more");

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
                                    Console.WriteLine($"\nLe capteur {sensor.Nom} d'id {sensor.IdCapteur} est de nouveau operationnel\n The sensor {sensor.Nom} id {sensor.IdCapteur} is back online");

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
                                    Console.WriteLine($"\nle capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien\n the sensor {sensor.Nom} id {sensor.IdCapteur} is functional");
                                }
                            }
                        }

                    }


                    /*verification du niveau de batterie, alerte l'utilisateur s'il faut changer la batterie*/
                    /*check battery level, alert user to change battery*/
                    if ((alertBattery==false)&(sensor.Batterie == true)) /*alertBattery est ici utilisée pour eviter d'envoyer un message avertissement batterie en plus d'une alerte plus de batterie*/
                    {                                                    /*alertBattery is used here to avoid sending a battery warning message in addition to a more battery alert*/
                        if (sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1] <= 20)
                        {

                            /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                            /*before adding a new alert, check to see if there is one already*/
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
                            Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Nom} d'id {sensor.IdCapteur} est presque dechargee ({sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1]})\n Careful the Battery of the sensor {sensor.Nom} id {sensor.IdCapteur} is runnig out of power ({sensor.NiveauBatterie[sensor.NiveauBatterie.Count - 1]})");
                        }
                    }


                } Console.WriteLine("tous les capteurs ont ete verifies/ all sensors have been checked\n##################################################\n");
            }
        }

      
    }
}
