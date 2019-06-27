/*################################################################################################################################*/
/*   Name: Sensorchecking programm                                                                                                */
/*   Goal: check sensor operation, identify defected sensor, create alerts, manage alerts, check  battery level                   */
/*   How to use this programm: execute this code on the server, this program runs indefinitely                                    */
/*   Name and project date: Murs Vegetal may/june 2019                                                                            */
/*   Project group: Mongo/ Big Data                                                                                               */
/*   Creator: Desmullier Gabriel, Verept Alexandre                                                                               */
/*################################################################################################################################*/


using SpaceSensorchecking;
using SpaceStatementchecking;
using SapcePump;
using Newtonsoft.Json;
using System.IO;
using System.Threading;


namespace BigDataHub
{
    public class CheckingConfiguration
    {
        public int turnAroundTime { get; set; }/*turnAroundTime:  the time between two system checks */
        public int toleranceThreshold { get; set; }/*tolerance: this is the tolerance threshold, number of missing statements acceptable*/
        public int deathThreshold { get; set; }/*death: this is the death threshold, number of missing statements acceptable before delete from database*/
        public int warningThreshold { get; set; }/*number of aberrant values acceptable before to send an alert*/
        public int errorThreshold { get; set; }/*number of aberrant values acceptable before to consider a sensor as a defected sensor*/
        public int repetitionThreshold { get; set; }/*number of value repetition acceptable before to send an alert*/
        public long alertUpdateTime { get; set; }  /*alertUpdateTime: maximum time interval at the end of which an alert message remains in the database*/
        public long goodAlertTime { get; set; }  /*goodAlertTime: the right alerts are alerts that indicate that the battery has been recharged or that a sensor is operational again */
    }
    public class Hub
    {
        static void Main()
        {
            PumpAnalyser.PrepareTheAnalyser(); /* Setup the analyzer CRUD for the pump */

        exeloop: /* infifite loop */
            CheckingConfiguration config = JsonConvert.DeserializeObject<CheckingConfiguration>(File.ReadAllText(@"Configuration.json")); /*if config change*/
            Statementchecking.StatementProg(config); /*check all statements of all sensors*/
            Sensorchecking.SensorProg(config); /*check all sensors*/
            PumpAnalyser.MainHandlingPumps(); /* take care of the water on the wall */
            Thread.Sleep(1000 * config.turnAroundTime);/*sleep turnaroundtime*/
            goto exeloop;
        }
    }
}

