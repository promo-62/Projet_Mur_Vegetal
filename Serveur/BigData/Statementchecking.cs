/*################################################################################################################################*/
/*   Name: Statementchecking programm                                                                                              */
/*   Goal: check statement, identify aberrant values                                                                               */
/*   How to use this programm: execute this code on the server, this program runs indefinitely                                    */
/*   Name and project date: Murs Vegetal may/june 2019                                                                            */
/*   Project group: Mongo/ Big Data                                                                                               */
/*   Creator: Desmullier Gabriel, Alexandre verept, Léa Alami
/*   Other major contributors: Léa Orsière                                                                             */
/*################################################################################################################################*/

using WebAPI.Models; /*classes de la Bdd*/
using MongoDB.Bson;
using MongoDB.Driver;
using Setup; /*outilsmongoC#*/
using System;
using System.Collections.Generic;
using BigDataHub;

namespace SpaceStatementchecking
{
    class Statementchecking

    {
        static MongoCRUD m_CRUD;
        static Random m_Rand;
        static MongoClient m_Client;
        static IMongoDatabase m_Database;
        static public void StatementProg(CheckingConfiguration config)
        {
            /* Association between sensor types and ids */
            /* !!! to be changed !!! */
            int IDHumidity = 4;
            int IDTemperature = 6;
            int IDLuminosity = 1;
            int IDAirQuality = 2;
            int IDPression = 4;
            int IDBeeFlux = 5;

            /*connection to the database MurVegetalDb*/
            Console.WriteLine("Connexion à la base de donnee / Connection to the database");
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); 
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
            Console.WriteLine("\nConnexion effectuee / Made connection ");
           
           // LoadRecordSuperiorEqualParameter  ?
            List<Samples> statementList = m_CRUD.LoadRecordSuperiorEqualParameter<Samples, long>("Samples", "SampleDate", DateTimeOffset.Now.ToUnixTimeSeconds() - config.turnAroundTime);
            foreach (Samples statement in statementList)
            {
                if (statement.IdSampleType == IDTemperature) 
                {
                    if (statement.Value > 45)
                    {
                        statement.Note = "Valeur trop elevee // Too high value";
                         m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value < -15)
                    {
                        statement.Note = "Valeur trop faible // Too low value";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }                                 
                }
                if (statement.IdSampleType == IDHumidity) 
                {
                    if (statement.Value < 0)
                    {
                        statement.Note = "Valeur impossible (trop bas) // Impossible value (too low)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value > 100)
                    {
                        statement.Note = "Valeur impossible (trop eleve) // Impossible value (too high)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                }
                if (statement.IdSampleType == IDPression) 
                {
                    if (statement.Value < 0.8)
                    {
                        statement.Note = "Valeur trop faible // Too low value";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value > 1.2)
                    {
                        statement.Note = "Valeur trop elevee // Too high value";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                }
                if (statement.IdSampleType == IDLuminosity) 
                {
                    if (statement.Value < 0)
                    {
                        statement.Note = "Valeur impossible (trop bas) // Impossible value (too low)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value > 100000)
                    {
                        statement.Note = "Valeur impossible (trop haut) // Impossible value (too high)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                }
                if (statement.IdSampleType == IDBeeFlux) 
                {
                    if (statement.Value < 0)
                    {
                        statement.Note = "Valeur impossible (trop bas) // Impossible value (too low)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                }
                if (statement.IdSampleType == IDAirQuality) 
                {
                    if (statement.Value < 0)
                    {
                        statement.Note = "Valeur impossible (trop bas) // Impossible value (too low)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value > 100)
                    {
                        statement.Note = "Valeur impossible (trop haut) // Impossible value (too high)";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                }
            }
        }
    }
}