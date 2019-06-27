using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;

namespace Mur_Vegetal.Pages
{
    public class WallModel : PageModel
    {

        public class Sensors
        {
            public int idSensor { get; set; }
            public int idSensorType { get; set; }
            public List<string> project { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int sensorDate { get; set; }
            public int lastSampleDate { get; set; }
            public List<object> batteryLevel { get; set; }
            public bool battery { get; set; }
            public int sleepTime { get; set; }
            public List<object> action { get; set; }
            public int version { get; set; }
            public int timeOut { get; set; }
            public bool isWorking { get; set; }
            public string id { get; set; }
        }
        public List <Sensors> Result {get; private set;}
        public bool IsError { get; private set; }

        public void OnGet()
        {
         
            var requestWall = Query.Get("http://iotdata.yhdf.fr/api/web/sensors");
            if(requestWall == "Error" || String.IsNullOrEmpty(requestWall)){
                IsError = true;
            }
            else{
                IsError = false;
                Result = JsonConvert.DeserializeObject<List<Sensors>>(requestWall); 
            }
        }
    }
}
