using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson; ///Utilise pour le ObjectId
using WebAPI.Models;
using Setup;
using nsPump;

namespace SapcePump
{   
    static class PumpAnalyser
    {
        static MongoCRUD m_CRUD;

        static int typeHumidity = 4;
        static int typeWattering = 3;
        static MongoClient m_Client;
        static IMongoDatabase m_Database;

        /*Prends les Sensors d'un meme type/Get the sensors of the same kind*/
        static List<Sensors> getSensorsOfSameKind(List<Sensors> listSensors, int type)
        {
            List<Sensors> sensorsSameType = new List<Sensors>();
            foreach(Sensors Sensor in listSensors)
                if(Sensor.IdSensorType == type)
                    sensorsSameType.Add(Sensor);
            return sensorsSameType;
        }

         /*Get the operational sensors in the list*/
        static List<Sensors> getSensorsFunctional(List<Sensors> listSensors)
        {
            List<Sensors> listSensorsFonctional = new List<Sensors>();
            foreach(Sensors Capteur in listSensors)
                if(Capteur.IsWorking == true)
                 listSensorsFonctional.Add(Capteur);
            return listSensorsFonctional;
        }

        /*Get all the sensors from the project list given*/
        static List<Sensors> getSensorsOfProjects(List<Sensors> listSensors, List<String> f_Project)
        {
            List<Sensors> listSensorsProject = new List<Sensors>();
            foreach(Sensors Sensor in listSensors)
                foreach(String SensorProject in Sensor.Project)
                    foreach(String Project in f_Project)
                        if(SensorProject == Project)
                            if(listSensorsProject.Any(sensor => Sensor.IdSensor == sensor.IdSensor))
                             listSensorsProject.Add(Sensor);
            return listSensorsProject;
        }

        /*Get the samples made by the sensor itself! */
        static List<Samples> getSummariesLinkToSensors(List<Samples> listSamples, List<Sensors> listSensors)
        {
            List<Samples> listSamplesOfSensors = new List<Samples>();
            /*Prendre les Samples des Sensors d'humidite fonctionnel/Take the summaries of functional humidity sensors*/
            foreach(Samples Sample in listSamples)
                foreach(Sensors Sensor in listSensors)
                    if(Sample.IdSensor == Sensor.IdSensor)
                        listSamplesOfSensors.Add(Sample);
            return listSamplesOfSensors;
        }

        /*Take the summaries of the sensors*/
        static List<Samples> getSummariesLinkToSensor(List<Samples> listSamples, Sensors Capteur)
        {
            List<Samples> listSamplesOfSensors = new List<Samples>();
            foreach(Samples Sample in listSamples)
                if(Sample.IdSensor == Capteur.IdSensor)
                    listSamplesOfSensors.Add(Sample);
            return listSamplesOfSensors;
        }
        /*Get all the samples that was published the last "lastTimeSummaries" in seconds,
          default for the last hour!*/
        static List<Samples> getLastSummaries(List<Samples> listSamples, int lastTimeSummaries = 1*3600)
        {
            List<Samples> lastSummaries = new List<Samples>();
            foreach(Samples Sample in listSamples)
                if(DateTimeOffset.Now.ToUnixTimeSeconds() - Sample.SampleDate < lastTimeSummaries)
                    lastSummaries.Add(Sample);
            return listSamples;
        }
        /*Get the last published sample from the list*/
        static Samples getLastSample(List<Samples> listSamples)
        {
            Samples lastSample = new Samples();
            foreach(Samples Sample in listSamples)
                if(DateTimeOffset.Now.ToUnixTimeSeconds() - Sample.SampleDate < DateTimeOffset.Now.ToUnixTimeSeconds() - lastSample.SampleDate)
                    lastSample = Sample;
            return lastSample;
        }

        static long lastGlobalAnalysis = 0;
        /*Handle the analysis of all the pumps!*/
        static public void MainHandlingPumps()
        {
            lastGlobalAnalysis = DateTimeOffset.Now.ToUnixTimeSeconds();
            List<Sensors> listSensors = m_CRUD.LoadRecords<Sensors>("Sensors");
            List<Samples> listSamples = m_CRUD.LoadRecords<Samples>("Samples");
            List<Sensors> listPumps = getSensorsFunctional(getSensorsOfSameKind(listSensors, typeWattering));

            IAnalysisForPumpStrategy AnalysisObj = new StandardAnalysisForPumpStrategy();

            foreach(Sensors Pump in listPumps)
            {
                Pump.Action[0].ToDo = 0;
                HandlingPumps(Pump, Pump.Project, AnalysisObj);
            }
        }
        /*Choose if the pump is on or off for the next connection! Also add a Sample if it will be active.*/
        static private void HandlingPumps(Sensors Pump, List<String> ProjectLink, IAnalysisForPumpStrategy AnalysisObj)
        {
            List<Sensors> listSensors = m_CRUD.LoadRecords<Sensors>("Sensors");
            List<Sensors> listHumiditySensors = getSensorsOfSameKind(listSensors, typeHumidity);
            List<Sensors> listHumidityFunctionalSensors = getSensorsFunctional(listHumiditySensors);
            List<Sensors> listHumidityFunctionalSensors_Project = getSensorsOfProjects(listHumidityFunctionalSensors, ProjectLink);


            
            List<Samples> listSamples = m_CRUD.LoadRecords<Samples>("Samples");
            List<Samples> listHumiditySamplesProject = getLastSummaries(getSummariesLinkToSensors(listSamples, listHumidityFunctionalSensors_Project));
            List<Samples> samplesPump = getSummariesLinkToSensor(listSamples, Pump);
            Samples lastSample = getLastSample(samplesPump);



            /*Analyse de releve/Sample analysis */
            int notation = AnalysisObj.Analysis(listHumiditySamplesProject, lastSample);

            /*Dit aux pompes d'arroser ou pas en fonction des Samples/Order the wattering of the plants by the pumps*/
            
            if(notation > 0)
            {
                Pump.Action[0].ToDo = notation;
                Samples RelevePompe = new Samples{
                    IdSensor = Pump.IdSensor,
                    SampleDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Value = notation
                };
                Console.WriteLine("A Pump is going to water for " + notation + " minuts!");
            }
            else
            {
                Pump.Action[0].ToDo = 0;
                Console.WriteLine("A Pump is not going to water for now!");
            }
        }

        static public void PrepareTheAnalyser()
        {
            m_Client = new MongoClient("mongodb://127.0.0.1:27017/");
            m_Database = m_Client.GetDatabase("MurVegetalDb");
            m_CRUD = new MongoCRUD(m_Database);
        }
    }
}