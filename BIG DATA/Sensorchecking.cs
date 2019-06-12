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
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); //"mongodb://10.127.0.81/MurVegetalDb"
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
         
                                  /*tolerance: this is the tolerance threshold, number of missing statements acceptable*/
            int toleranceThreshold = 3; /* Tolerance: nombre de releve non recues avant de considerer le capteur mort*/ 
            int n = 0;
            long timenow = 0;
            long timexec = 0;
            long timelimit = 0;
            while (n == 0) /*boucle infinie*/ /*infinite loop: this program is voluntarily endless*/
            {
                timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                timexec = timenow + 3600; /*temps de la prochaine execution du programme toutes les heures a regler en fonction de ce que l'on souhaite*/
                                        /*time between two executions of this algorithm, can be settle*/
                while (timenow < timexec) /*on attend*/ /*waiting of the next execution of the algorithm*/
                {
                    timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                }

                /*declenchement de la verification*/ /*starting*/
                Console.WriteLine("Début de la vérification des capteurs/ Sensor verification begins");


                var sensorlist = m_CRUD.LoadRecords<Capteur>("Capteurs"); /*je recupere les capteurs*/ /*retrievering of the sensors from the database*/
                foreach (var sensor in sensorlist) /*we will check all sensors one by one*/
                {
                    Console.WriteLine($"test de {sensor.Nom}");

                    if (sensor.DateDernierReleve == 0)     /*si le capteur n'a jamais au grand jamais envoye de donnees*/ /*if the sensor had not yet send statement*/
                    {
                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                        if (sensor.DateCapteur + sensor.DelaiVeille * toleranceThreshold < timenow) /*s'il n'est pas neuf il est considere comme non fonctionnel */ /*if the sensor isn't new, it is defected*/
                        {
                            sensor.Fonctionne = false;
                            m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                            Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur} id defected");
                        }
                        else { Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien/ the sensor {sensor.Nom} id {sensor.IdCapteur} is functional"); }

                    }
                    else
                    {       /*s'il a deja donne des releves*/ /*if the sensor had already send a statement*/
                        timenow = DateTimeOffset.Now.ToUnixTimeSeconds(); /*prise de temps reel*/
                        timelimit = timenow - sensor.DelaiVeille * toleranceThreshold;
                        if (sensor.DateDernierReleve < timelimit) /*if the last statement is older than the timelimit, it is considered as defected*/
                        { /*si le dernier releve est plus age que notre limite timelimite il est considere comme defectueux*/
                            sensor.Fonctionne = false;
                            m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                            Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} est defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur} id defected");
                        }
                        else /*si un capteur a bien donne des releves*/ /*if the sensor had send recently a statement*/
                        {
                            sensor.Fonctionne = true;

                            m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                            if (sensor.Fonctionne == false)/*si un capteur mort redonne des releves on peut le reclasser parmis les vivants*/ /*if a defected gives back statement, it is again functional*/
                            {
                                sensor.Fonctionne = true;
                                m_CRUD.UpsetRecord<Capteur>("Capteurs", sensor.Id, sensor); /*mise a jour du statut*/
                                Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} n'est plus defectueux/ the sensor {sensor.Nom} id {sensor.IdCapteur} id defected");
                            }
                            else /*si tout va bien*/ /*if all goes well*/
                            {
                                Console.WriteLine($"le capteur {sensor.Nom} d'id {sensor.IdCapteur} fonctionne bien/ the sensor {sensor.Nom} id {sensor.IdCapteur} is functional");
                            }
                        }

                    }

                } Console.WriteLine("tous les capteurs ont été vérifiés/ all sensors have been checked");
            }
        }

      
    }
}
