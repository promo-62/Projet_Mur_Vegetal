/*################################################################################################################################*/
/*   Name: Sensorchecking programm                                                                                                */
/*   Goal: check sensor operation, identify defective sensor, create alerts, manage alerts, check  battery level                   */
/*   How to use this programm: execute this code on the server, this program runs indefinitely                                    */
/*   Name and project date: Murs Vegetal may/june 2019                                                                            */
/*   Project group: Mongo/ Big Data                                                                                               */
/*   Creator: Desmullier Gabriel, Anthony Coupey, Gregoire De Clercq                                                              */
/*   Other major contributors: Verept Alexandre                                                                                 */
/*################################################################################################################################*/

using WebAPI.Models; /*classes de la Bdd*/
using MongoDB.Bson;
using MongoDB.Driver;
using Setup; /*outilsmongoC#*/
using System;
using System.Collections.Generic;
using BigDataHub;

namespace SpaceSensorchecking
{
    class Sensorchecking

    {
        static MongoCRUD m_CRUD;
        static Random m_Rand;
        static MongoClient m_Client;
        static IMongoDatabase m_Database;

        static public void DeleteSensor(String f_sensorID)
        {
        /* delete sensor ID in SensorTypes */
                m_CRUD.LoadRecords<SensorTypes>("SensorTypes");
                List<SensorTypes> listSensorTypes = m_CRUD.LoadRecords<SensorTypes>("SensorTypes");
                foreach(SensorTypes Type in listSensorTypes)
                {
                    foreach(String SensorID in Type.SensorIds)
                    {
                        if (SensorID == f_sensorID)
                        {
                            Type.SensorIds.Remove(SensorID);
                            m_CRUD.UpsetRecord<SensorTypes>("SensorTypes", ObjectId.Parse(Type.Id), Type);
                            break;
                        }
                    }
                }
                /* delete sensor in BDD */
                DeleteSensor(f_sensorID); /* delete sensor */
        }

        static public void SensorProg(CheckingConfiguration config)
        {
            /*connection to the database MurVegetalDb*/
            Console.WriteLine("Connexion à la base de donnee / Connection to the database");
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); //"mongodb://10.127.0.81/MurVegetalDb"
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
            Console.WriteLine("\nConnexion effectuee / Made connection ");        
        
            /*variables temporelles utilisees comme critere lors du test des capteurs / time variables used as a screen during sensor testing*/
            long timelimit = 0;
            long timedeath = 0;

            int numberAberrantValues = 0;

            /*utilise pour eviter une repetition d'alertes / used to avoid repetition of alerts*/
            bool alertBattery = false;
            
            Console.WriteLine("\n Fin de la configuration, mise en marche / End of configuration, start ");

            /*Tri des alertes*/
            Console.WriteLine("\nDebut de la mise a jour des alertes/ update of alerts begins");
            var alertlist = m_CRUD.LoadRecords<Alerts>("Alerts"); /*je recupere les alertes*/ /*retrievering of the alerts from the database*/
            foreach(var alert in alertlist)
            {                    
               /*les erreurs vieillent de plus de alertUpdateTime sont supprimes*/
                /*oldest alerts are deleted depending on the configuration alertUpdateTime*/
                if (alert.DateAlert < DateTimeOffset.Now.ToUnixTimeSeconds() - config.alertUpdateTime)                    {
                    m_CRUD.DeleteRecord("Alerts", alert.Id);
                }
                else
                {
                   /*les messages d'alerte back online sont supprime au bout de goodAlertTime*/
                    /*back-online alert messages are deleted after goodAlertTime*/
                    if ((alert.AlertReason == "De nouveau operationnel / Back online") && (alert.DateAlert < DateTimeOffset.Now.ToUnixTimeSeconds() - config.goodAlertTime))
                    {
                        m_CRUD.DeleteRecord("Alerts", alert.Id);
                    }
                    /*les messages attention batterie sont supprimes si le niveau de batterie est de nouveau plein, une alerte batterie remise est ajoute pendant goodAlertTime*/
                    /*the battery attention messages are deleted if the battery level is again full, a reset battery alert is added during goodAlertTime*/
                    if (alert.AlertReason == "Attention Batterie faible/ Careful low battery")
                    {
                        List<Sensors> alertsensor = m_CRUD.LoadRecordByParameter<Sensors, int>("Sensors", "IdSensor", alert.IdSensor);
                            foreach (var sensor in alertsensor) /*La fonction LoadRecord renvoie une liste mais il n'y a normalement qu'un seul capteur dedans*/
                                                            /*The LoadRecord function returns a list but there is normally only one sensor in it*/
                        {
                            if (sensor.BatteryLevel[sensor.BatteryLevel.Count - 1] > 20) /*nouveau test / New test*/
                            {
                                m_CRUD.DeleteRecord("Alerts", alert.Id); /*je supprime l'alerte Attention batterie*//*I delete the battery alert*/

                                Alerts RechargedBatteryAlert = new Alerts   /*Une alerte qui indique que la batterie a ete rechargee est ajoutee a la liste des alertes*/
                                {                                            /*An alert indicating that the battery has been recharged is added to the alert list*/
                                    IdSensor= sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = true,
                                    AlertReason = "Batterie Rechargee / Recharged Battery"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", RechargedBatteryAlert);                                    Console.WriteLine($"\nLe capteur {sensor.Name} d'id {sensor.IdSensor} a sa batterie rechargee\n The sensor {sensor.Name} id {sensor.IdSensor} has his battery recharged");
                            }
                        }
                    }
                    if ((alert.AlertReason == "Batterie Rechargee / Recharged Battery") && (alert.DateAlert < DateTimeOffset.Now.ToUnixTimeSeconds() - config.goodAlertTime)) /*la bonne alerte est supprimee apres goodAlertTime minutes*/
                                                                                                                                                                             /*the good alert is deleted after goodAlertTime minutes*/
                    {
                        m_CRUD.DeleteRecord("Alerts", alert.Id);
                    }
                }

            }
            Console.WriteLine("\nAlertes mises a jour/ alerts updated");

            /*declenchement de la verification des capteurs*/ /*starting*/
            Console.WriteLine("\nDebut de la verification des capteurs/ Sensor verification begins");


            var sensorlist = m_CRUD.LoadRecords<Sensors>("Sensors"); /*je recupere les capteurs*/ /*retrievering of the sensors from the database*/
            foreach (var sensor in sensorlist) /*we will check all sensors one by one*/
            {
                Console.WriteLine($"\n_______________________________________________________________________\ntest de {sensor.Name}");

                alertBattery = false; /*indicateur de dysfonctionnement du a la batterie, utilise pour eviter d'afficher une alerte changement de batterie s'il y a deja une alerte plus de batterie*/
                /*Battery malfunction indicator, used to avoid displaying a battery change alert if there is already a more battery alert*/
                    
                if (sensor.LastSampleDate == 0)     /*si le capteur n'a jamais au grand jamais envoye de donnees*/ /*if the sensor had not yet send statement*/
                {



                    if (sensor.SensorDate + sensor.SleepTime * 60 * config.deathThreshold < DateTimeOffset.Now.ToUnixTimeSeconds()) /*si un capteur n'est pas neuf et n'a pas envoye de releve depuis tres longtemps*/
                    {                                                                       /*if a sensor is not new and has not sent a lift for a long time*/
                        Alerts AlerteDeath = new Alerts
                        {                                 /*une alerte capteur mort est emise*//*an dead sensor alert is create*/
                            IdSensor = sensor.IdSensor,
                            Name = sensor.Name,
                            DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                            IsWorking = false,
                            AlertReason = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                        };
                        m_CRUD.InsertRecord<Alerts>("Alerts", AlerteDeath);
                        Console.WriteLine($"\nAlerte Batterie capteur {sensor.Name} d'id {sensor.IdSensor}\n Battery Alert sensor {sensor.Name} id {sensor.IdSensor}");
                        alertBattery = true;
                        DeleteSensor(sensor.Id); /* delete sensor */
                    }

                    else
                    {
                        if (sensor.SensorDate + sensor.SleepTime * 60 * config.toleranceThreshold < DateTimeOffset.Now.ToUnixTimeSeconds()) /*s'il n'est pas neuf il est considere comme non fonctionnel */ /*if the sensor isn't new, it is defective*/
                        {
                                
                            sensor.IsWorking = false;
                            m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/
                            Console.WriteLine($"\nle capteur {sensor.Name} d'id {sensor.IdSensor} est defectueux\n the sensor {sensor.Name} id {sensor.IdSensor} id defective");

                            /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                            /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/
                            if (sensor.Battery == true)
                            {
                                if (sensor.BatteryLevel[sensor.BatteryLevel.Count - 1] <= 20)
                                {

                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
                                    List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Plus de Batterie/ No more Battery");
                                    if (previousAlert == null)
                                    {
                                        Alerts AlerteBatterie = new Alerts
                                        {
                                            IdSensor = sensor.IdSensor,
                                            Name = sensor.Name,
                                            DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            IsWorking = false,
                                            AlertReason = "Plus de Batterie/ No more Battery"
                                        };
                                        m_CRUD.InsertRecord<Alerts>("Alerts", AlerteBatterie);
                                    }
                                    Console.WriteLine($"\nAlerte Batterie capteur {sensor.Name} d'id {sensor.IdSensor}\n Battery Alert sensor {sensor.Name} id {sensor.IdSensor}");
                                    alertBattery = true;
                                }
                                else
                                {
                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
                                    List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Erreur de fonctionnement/ Operating error");
                                    if (previousAlert == null)
                                    {
                                        Alerts AlerteFonctionnement = new Alerts
                                        {
                                            IdSensor = sensor.IdSensor,
                                            Name = sensor.Name,
                                            DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            IsWorking = false,
                                            AlertReason = "Erreur de fonctionnement/ Operating error"
                                        };
                                        m_CRUD.InsertRecord<Alerts>("Alerts", AlerteFonctionnement);
                                    }
                                    Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Name} d'id {sensor.IdSensor}/\nOperating Alert sensor {sensor.Name} id {sensor.IdSensor}");

                                }
                            }
                            else
                            {
                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
                                    List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Erreur de fonctionnement/ Operating error");
                                    if (previousAlert == null)
                                    {
                                        Alerts AlerteFonctionnement = new Alerts
                                        {
                                            IdSensor = sensor.IdSensor,
                                            Name = sensor.Name,
                                            DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            IsWorking = false,
                                            AlertReason = "Erreur de fonctionnement/ Operating error"
                                        };
                                        m_CRUD.InsertRecord<Alerts>("Alerts", AlerteFonctionnement);
                                    }
                                    Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Name} d'id {sensor.IdSensor}\n Operating Alert sensor {sensor.Name} id {sensor.IdSensor}");
                                }


                            }

                            /*Si le capteur n'a jamais fait de relevé et est nouveau
                          If the sensor never send summaries and is new*/
                            else
                            {
                                /*Si le capteur n'a pas de nom, lui en attribuer un et mettre une alerte
                                If the sensor has any name, give it one to it and send an alert */
                                if (sensor.Name == "")
                                {
                                    sensor.Name = "Capteur_Sans_Nom=" + sensor.IdSensorType + "_";
                                    sensor.Name += sensor.Id;

                                    Alerts AlerteVide = new Alerts
                                    {
                                        IdSensor = sensor.IdSensor,
                                        Name = sensor.Name,
                                        DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        IsWorking = true,
                                        AlertReason = "Nouveau capteur a remplir/ New sensor to be filled"
                                    };
                                    m_CRUD.InsertRecord<Alerts>("Alerts", AlerteVide);
                                    Console.WriteLine($"Alerte capteur {sensor.Name} d'id {sensor.IdSensor} est a remplir!/ Alert sensor {sensor.Name} id {sensor.IdSensor} needs to be filled!");
                                }
                                /*Si le capteur a deja un nom, une alerte a deja ete effectuee, donc pas de problemes
                                If the sensor has already a name, an alert was already sent and there is no problem, okeyyyy ????*/
                                /*a été changé de place */
                                else { Console.WriteLine($"le capteur {sensor.Name} d'id {sensor.IdSensor} fonctionne bien/ the sensor {sensor.Name} id {sensor.IdSensor} is functional"); }
                            }
                        }
                    }
                    else
                    {       /*s'il a deja donne des releves*/ /*if the sensor had already send a statement*/
                        
                        timelimit = DateTimeOffset.Now.ToUnixTimeSeconds() - sensor.SleepTime * 60 * config.toleranceThreshold;
                        timedeath = DateTimeOffset.Now.ToUnixTimeSeconds() - sensor.SleepTime * 60 * config.deathThreshold;

                        /*if the sensor has not sent a statement for a long time, it become dead and it is deleted from the database*/
                        if (sensor.LastSampleDate < timedeath)
                        { /*si le capteur n'a pas envoye de releve depuis tres longtemps, il est mort et est retire de la BDD*/

                            Console.WriteLine($"\nle capteur {sensor.Name} d'id {sensor.IdSensor} est mort il a ete supprime de la BDD\n the sensor {sensor.Name} id {sensor.IdSensor} is dead and is deleted from the database");
                            Alerts AlertDeath = new Alerts
                            {
                                IdSensor = sensor.IdSensor,
                                Name = sensor.Name,
                                DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                IsWorking = false,
                                AlertReason = "Capteur est considere comme mort, il a ete supprime de la base de donnee/ Sensor is considered dead, it has been deleted from the database"
                            };
                            m_CRUD.InsertRecord<Alerts>("Alerts", AlertDeath);
                            Console.WriteLine($"\nAlerte Batterie capteur {sensor.Name} d'id {sensor.IdSensor}\n Battery Alert sensor {sensor.Name} id {sensor.IdSensor}");
                            alertBattery = true;
                            DeleteSensor(sensor.Id); /* delete sensor */


                        }
                        else
                        {
                            if (sensor.LastSampleDate < timelimit) /*if the last statement is older than the timelimit, it is considered as defective*/
                            { /*si le dernier releve est plus age que notre limite timelimite il est considere comme defectueux*/
                                sensor.IsWorking = false;
                               
                                m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/
                                Console.WriteLine($"\nle capteur {sensor.Name} d'id {sensor.IdSensor} est defectueux\n the sensor {sensor.Name} id {sensor.IdSensor} is defective");

                                /*puisqu'il a une erreur, le programme va regarder si la batterie est responsable si c'est le cas une alerte Batterie est ajoutee a la liste des alertes si ce n'est pas la batterie c'est une alerte de fonctionnement qui est ajoutee a la liste des alertes*/
                                /*it has an error, the program will look if the battery is responsible if it is the case a battery alert is added to the list of Alerts if it is not the battery is an operating alert that is added to the list of Alerts*/


                                if (sensor.Battery == true)
                                {
                                    if (sensor.BatteryLevel[sensor.BatteryLevel.Count - 1] <= 20)

                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
                                        List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Plus de Batterie/ No more Battery");
                                        if (previousAlert.Count == 0)
                                        {
                                            Alerts BatteryAlert = new Alerts
                                            {
                                                IdSensor = sensor.IdSensor,
                                                Name = sensor.Name,
                                                DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                IsWorking = false,
                                                AlertReason = "Plus de Batterie/ No more Battery"
                                            };
                                            m_CRUD.InsertRecord<Alerts>("Alerts", BatteryAlert);
                                        }
                                        Console.WriteLine($"\nAlerte Batterie capteur {sensor.Name} d'id {sensor.IdSensor}\n Battery Alert sensor {sensor.Name} id {sensor.IdSensor}");
                                        alertBattery = true;
                                    }
                                    else
                                    {
                                        /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                        /*before adding a new alert, check to see if there is one already*/
                                        List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Erreur de fonctionnement/ Operating error");
                                        if (previousAlert.Count == 0)
                                        {

                                            Alerts OperatingAlert = new Alerts
                                            {
                                                IdSensor = sensor.IdSensor,
                                                Name = sensor.Name,
                                                DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                                IsWorking = false,
                                                AlertReason = "Erreur de fonctionnement/ Operating error"
                                            };
                                            m_CRUD.InsertRecord<Alerts>("Alerts", OperatingAlert);
                                        }
                                        Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Name} d'id {sensor.IdSensor}\n Operating Alert sensor {sensor.Name} id {sensor.IdSensor}");

                                    }
                                }
                                else
                                {

                                    /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                                    /*before adding a new alert, check to see if there is one already*/
                                    List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Erreur de fonctionnement/ Operating error");
                                    if (previousAlert.Count == 0)
                                    {
                                        Alerts OperatingAlert = new Alerts
                                        {
                                            IdSensor = sensor.IdSensor,
                                            Name = sensor.Name,
                                            DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                            IsWorking = false,
                                            AlertReason = "Erreur de fonctionnement/ Operating error"
                                        };
                                        m_CRUD.InsertRecord<Alerts>("Alerts", OperatingAlert);
                                    }
                                    Console.WriteLine($"\nAlerte de Fonctionnement capteur {sensor.Name} d'id {sensor.IdSensor}\n Operating Alert sensor {sensor.Name} id {sensor.IdSensor}");
                                }
                            }
                            else /*si un capteur a bien donne des releves*/ /*if the sensor had send recently a statement*/
                            {

                                if (sensor.IsWorking == false)/*si un capteur defectueux redonne des releves on peut le reclasser parmis les vivants*/ /*if a defective gives back statement, it is again functional*/
                                {
                                    sensor.IsWorking = true;
                                    m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/
                                    Console.WriteLine($"\nle capteur {sensor.Name} d'id {sensor.IdSensor} n'est plus defectueux\n the sensor {sensor.Name} id {sensor.IdSensor}is not defective any more");

                                    /*Une alerte qui indique le retour du capteur est ajoutee aux alertes*/
                                    /*An alert indicating the return of the sensor is added to the alerts*/
                                    Alerts AlerteRetour = new Alerts
                                    {
                                        IdSensor = sensor.IdSensor,
                                        Name = sensor.Name,
                                        DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                        IsWorking = true,
                                        AlertReason = "De nouveau operationnel/ Back online"
                                    };
                                    m_CRUD.InsertRecord<Alerts>("Alerts", AlerteRetour);
                                    Console.WriteLine($"\nLe capteur {sensor.Name} d'id {sensor.IdSensor} est de nouveau operationnel\n The sensor {sensor.Name} id {sensor.IdSensor} is back online");

                                    /*Les alertes fonctionnement ou batterie precedentes sont supprimes*/
                                    /*Previous operation or battery alerts are deleted*/
                                    List<Alerts> OldAlerts = m_CRUD.LoadRecordByParameter<Alerts, int>("Alerts", "IdSensor", sensor.IdSensor);
                                    foreach (var OldAlert in OldAlerts)
                                    {
                                        if ((OldAlert.AlertReason == "Erreur de fonctionnement/ Operating error") || (OldAlert.AlertReason == "Plus de Batterie/ No more Battery"))
                                        {
                                            m_CRUD.DeleteRecord("Alerts", OldAlert.Id);

                                        }
                                    }

                                }
                                else /*si tout va bien*/ /*if all goes well*/
                                {
                                    Console.WriteLine($"\nle capteur {sensor.Name} d'id {sensor.IdSensor} fonctionne bien\n the sensor {sensor.Name} id {sensor.IdSensor} is functional");
                                }
                            }
                        }

                    }

                    /*gestion alertes de releves defaillants*//*management of aberrant statements alerts*/


                    if (sensor.LastSampleDate != 0)
                    {
                        List<Samples> statementlist = m_CRUD.LoadRecordByTwoParameter<Samples, int, int>("Releve", "IdSensor", sensor.IdSensor, "TypeCapteur", sensor.IdSensorType);
                        /*valeur qui compte le nombre de valeurs aberrantes*//*value that counts the number of outliers*/
                        numberAberrantValues = 0;

                        /*outils pour compter le nombre de valeurs *//*tools to count the number of values */
                        int actualValue = -1;
                        int repetitionNumber = 0;
                        int maxRepetitionNumber = 0;

                        foreach (var statement in statementlist)
                        {

                            /*if the value is not the same as the previous one, this value is recorded and the repetition counter is reset to zero */
                            if (statement.Value != actualValue)/*si jamais la valeur n'est pas la meme que la precedente, on enregistre cette valeur et on remet le compteur des repetitions a zero*/
                            {
                                actualValue = statement.Value;
                                repetitionNumber = 0;
                            }
                            else                            /*si la valeur est la meme que la precedente on augmente le compteur des repetitions*/
                            {                               /*if the value is not the same as the previous one, this value is recorded and the repetition counter is reset to zero*/ /*if the value is the same as the previous one the repetition counter*/
                                repetitionNumber = repetitionNumber + 1;
                                if (repetitionNumber > maxRepetitionNumber)/*on garde a chaque fois le plus grand nombre de repetitions*//*the greatest number of repetitions is kept each time*/
                                {
                                    maxRepetitionNumber = repetitionNumber;
                                }
                            }
                            /*compte le nombre de valeurs aberrantes*//*counts the number of aberrant values*/
                            if (statement.Note != null)
                            {
                                numberAberrantValues = numberAberrantValues + 1;
                            }
                        }
                        /*if the outlier number is between the two thresholds, a simple aberrant data alert is sent*/
                        if ((numberAberrantValues > config.warningThreshold) && (config.errorThreshold > numberAberrantValues)) /*si le nombre de valeur aberrante est compris entre les deux seuil, une simple alerte de donne aberrante est envoye*/
                        {
                            Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Name} d'id {sensor.IdSensor} a des valeurs aberrantes\n Careful the Battery of the sensor {sensor.Name} id {sensor.IdSensor} has aberrant values");
                            List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Valeurs aberrantes constatees/ Noticed absurd values");
                            if (previousAlert.Count == 0)
                            {
                                Alerts AlerteAberrant = new Alerts
                                {
                                    IdSensor = sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = true,
                                    AlertReason = "Valeurs aberrantes constatees/ Noticed absurd values"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", AlerteAberrant);
                            }
                            /*if the administrator has deleted values a defective sensor due to outliers can return to running*/
                            /*si l'administrateur a supprime des valeur un capteur defectueux a cause des valeurs aberrante peut revenir en marche*/
                            List<Alerts> previousError = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Valeurs aberrantes constatees, le capteur est  defectueux/ Noticed absurd values, the sensor id defective");
                            if (previousError.Count != 0)
                            {
                                sensor.IsWorking = true;
                                m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/

                                foreach (Alerts alert in previousError) /*les erreur capteur defectueux sont supprime*//*Defective sensor errors are removed*/
                                {
                                    m_CRUD.DeleteRecord("Alerts", alert.Id);
                                }
                                Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Name} d'id {sensor.IdSensor} n'est plus defectueux\n Careful the Battery of the sensor {sensor.Name} id {sensor.IdSensor} is not defective any more");
                            }
                        }
                        if (numberAberrantValues > config.errorThreshold) /*si le nombre de valeurs aberrante est superieur au seuil max, le capteur devient defaillant*//*if the number of outliers is greater than the max threshold, the sensor becomes defacing*/
                        {
                            List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Valeurs aberrantes constatees, le capteur est  defectueux/ Noticed absurd values, the sensor id defective");
                            if (previousAlert.Count == 0)
                            {
                                Alerts AlerteAberrant = new Alerts
                                {
                                    IdSensor = sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = false,
                                    AlertReason = "Valeurs aberrantes constatees, le capteur est  defectueux/ Noticed absurd values, the sensor id defective"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", AlerteAberrant);
                                sensor.IsWorking = false;
                                m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/
                            }
                        }
                        /*when everything is going well, we check to see if there were previous alerts indicating outliers, if there were any they are deleted*/
                        if (numberAberrantValues < config.warningThreshold)  /*lorsque tout va bien, on regarde si avant il y avait des alertes signalant des valeurs aberrantes, s'il y en avait elles sont supprimees*/
                        {                                
                            List<Alerts> previousError = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Valeurs aberrantes constatees, le capteur est  defectueux/ Noticed absurd values, the sensor id defective");
                            if (previousError.Count != 0)
                            {
                                Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Name} d'id {sensor.IdSensor} n'est plus defectueux\n Careful the Battery of the sensor {sensor.Name} id {sensor.IdSensor} is not defective any more");

                                sensor.IsWorking = true;
                                m_CRUD.UpsetRecord<Sensors>("Sensors", ObjectId.Parse(sensor.Id), sensor); /*mise a jour du statut*/

                                foreach (Alerts alert in previousError) /*les erreur capteur defectueux sont supprime*//*Defective sensor errors are removed*/
                                {
                                    m_CRUD.DeleteRecord("Alerts", alert.Id);
                                }
                                Alerts AlerteRetour = new Alerts
                                {
                                    IdSensor = sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = true,
                                    AlertReason = "De nouveau operationnel/ Back online"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", AlerteRetour);
                                Console.WriteLine($"\nLe capteur {sensor.Name} d'id {sensor.IdSensor} est de nouveau operationnel\n The sensor {sensor.Name} id {sensor.IdSensor} is back online");
                            }
                            previousError = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Valeurs aberrantes constatees/ Noticed absurd values");
                            if (previousError.Count != 0)
                            {
                                foreach (Alerts alert in previousError) /*les erreur capteur defectueux sont supprime*//*Defective sensor errors are removed*/
                                {
                                    m_CRUD.DeleteRecord("Alerts", alert.Id);
                                }
                            }
                        }
                        /*pour le nombre de repetitions*//*for the number of repetitions*/
                        /*if several times in a row the same value is found on the sensor readings, a warning message is sent*/
                        if (maxRepetitionNumber > config.repetitionThreshold)  /*si plusieurs fois d'affilee on a repere la meme valeur sur les releves d'un capteur on envoie un message d'avertissement*/
                        {
                            List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Avertissement, le capteur a envoye la meme mesure plusieurs fois d'affilee/ Warning, the sensor sent the same statement several times in a row");
                            if (previousAlert.Count == 0)
                            {
                                Alerts AlerteRepetition = new Alerts
                                {
                                    IdSensor = sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = true,
                                    AlertReason = "Avertissement, le capteur a envoye la meme mesure plusieurs fois d'affilee/ Warning, the sensor sent the same statement several times in a row"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", AlerteRepetition);
                            }
                            Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Name} d'id {sensor.IdSensor} a enregistre un grand nombre de fois la meme valeur\n Careful the Battery of the sensor {sensor.Name} id {sensor.IdSensor} has recorded a large number of times the same value ");
                        }
                        /*if this is not the case but it was before, the alerts are deleted*/
                        if (maxRepetitionNumber < config.repetitionThreshold)/*si ce n'est pas le cas mais que cela l'était avant, les alertes sont supprimees*/
                        {
                            List<Alerts> previousError = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Avertissement, le capteur a envoye la meme mesure plusieurs fois d'affilee/ Warning, the sensor sent the same statement several times in a row");
                            if (previousError.Count != 0)
                            {
                                foreach (Alerts alert in previousError) /*les erreur capteur defectueux sont supprime*//*Defective sensor errors are removed*/
                                {
                                    m_CRUD.DeleteRecord("Alerts", alert.Id);
                                }
                            }

                        }
                    }
                        /*verification du niveau de batterie, alerte l'utilisateur s'il faut changer la batterie*/
                        /*check battery level, alert user to change battery*/
                        if ((alertBattery==false)&(sensor.Battery == true)) /*alertBattery est ici utilisée pour eviter d'envoyer un message avertissement batterie en plus d'une alerte plus de batterie*/
                    {                                                    /*alertBattery is used here to avoid sending a battery warning message in addition to a more battery alert*/
                        if (sensor.BatteryLevel[sensor.BatteryLevel.Count - 1] <= 20)
                        {
                            /*avant d'ajouter une nouvelle alerte on regarde s'il n'y en a pas deja une*/
                            /*before adding a new alert, check to see if there is one already*/
                            List<Alerts> previousAlert = m_CRUD.LoadRecordByTwoParameter<Alerts, int, string>("Alerts", "IdSensor", sensor.IdSensor, "AlertReason", "Attention Batterie faible/ Careful low battery");
                            if (previousAlert.Count == 0)
                            {
                                Alerts AlerteBatterie = new Alerts
                                {
                                    IdSensor = sensor.IdSensor,
                                    Name = sensor.Name,
                                    DateAlert = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                    IsWorking = true,
                                    AlertReason = "Attention Batterie faible/ Careful low battery"
                                };
                                m_CRUD.InsertRecord<Alerts>("Alerts", AlerteBatterie);
                            }
                            Console.WriteLine($"\nAttention la Batterie du capteur{sensor.Name} d'id {sensor.IdSensor} est presque dechargee ({sensor.BatteryLevel[sensor.BatteryLevel.Count - 1]})\n Careful the Battery of the sensor {sensor.Name} id {sensor.IdSensor} is runnig out of power ({sensor.BatteryLevel[sensor.BatteryLevel.Count - 1]})");
                        }
                    }
                } Console.WriteLine("tous les capteurs ont ete verifies/ all sensors have been checked\n##################################################\n");
            }
        }
    }

