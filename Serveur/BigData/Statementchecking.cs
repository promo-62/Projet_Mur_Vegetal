/*################################################################################################################################*/
/*   Name: Statementchecking programm                                                                                              */
/*   Goal: check statement, identify aberrant values                                                                               */
/*   How to use this programm: execute this code on the server, this program runs indefinitely                                    */
/*   Name and project date: Murs Vegetal may/june 2019                                                                            */
/*   Project group: Mongo/ Big Data                                                                                               */
/*   Creator: Desmullier Gabriel,                                                                                                 */
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
            /*connection to the database MurVegetalDb*/
            Console.WriteLine("Connexion à la base de donnee / Connection to the database");
            m_Rand = new Random();
            m_Client = new MongoClient("mongodb://localhost:27017/"); //"mongodb://10.127.0.81/MurVegetalDb"
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
            Console.WriteLine("\nConnexion effectuee / Made connection ");
           
            List<Samples> statementList = m_CRUD.LoadRecordInferiorEqualParameter<Samples, long>("Samples", "DateReleve", DateTimeOffset.Now.ToUnixTimeSeconds() - config.turnAroundTime);
            foreach (Samples statement in statementList)
            {
                if (statement.IdSampleType == 1) /*si l'IdSample 1 est egal a temperature*/
                {
                    if (statement.Value > 45)
                    {
                        statement.Note = "Valeur trop elevee";
                         m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }
                    if (statement.Value < -15)
                    {
                        statement.Note = "Valeur trop faible";
                        m_CRUD.UpsetRecord<Samples>("Samples", ObjectId.Parse(statement.Id), statement);
                    }                        /*si tout va bien aucun commentaire*/
                }
                /*de meme pour tous les autres types de releves*/
            }
        }
    }
}