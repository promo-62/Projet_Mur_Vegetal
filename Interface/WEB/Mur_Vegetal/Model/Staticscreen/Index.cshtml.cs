using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Mur_Vegetal.Pages
{
    public class StaticscreenModel : PageModel
    {

        public class Frame{
            public int onScreenTime { get; set; }
            public bool isOnScreen { get; set; }
            public string name { get; set; }
            public int carrousselTime { get; set; }
            public string id { get; set; }
        }

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

        public class News{
            public string name { get; set; }
            public int eventDate { get; set; }
            public int beginningDate { get; set; }
            public string eventImage { get; set; }
            public int endingDate { get; set; }
            public string text { get; set; }
            public int position { get; set; }
            public string id { get; set; }
        }

        public class CountDown{
            public string text { get; set; }
            public string name { get; set; }
            public int endingDateEvent { get; set; }
            public int beginningDateEvent { get; set; }
            public int endingDateCountdown { get; set; }
            public int position { get; set; }
            public object image { get; set; }
            public string id { get; set; }
        }

        public class Medias
        {
            public string name { get; set; }
            public int beginningDate { get; set; }
            public int endingDate { get; set; }
            public string video { get; set; }
            public string image { get; set; }
            public string id { get; set; }
        }

        public class Social
        {
            public string username { get; set; }
            public string pageWidget { get; set; }
            public string widget { get; set; }
            public string id { get; set; }
        }
        public List <Frame> ResultFrame {get; private set;}
        public List <Sensors> ResultWall {get; private set;}
        public List <News> ResultNews { get; private set; }
        public List <CountDown> ResultCountdown {get; private set;}
        public List <Medias> ResultMedias {get; private set;}
        public List <Social> ResultSocialnetworks {get; private set;}

        public CountDown ResultLastCountdown {get; private set;}

        public bool IsErrorMedias { get; private set; }
        public bool IsErrorNews { get; private set; }
        public bool IsErrorSocial { get; private set; }
        public bool IsErrorWall { get; private set; }
        public bool IsErrorFrame { get; private set; }
        public bool IsErrorCountdown { get; private set; }

        public int timeWall { get; set; }
        public int timeNews { get; set; }
        public int timeCountdown { get; set; }
        public int timeMedias { get; set; }
        public int timeSocial { get; set; }

        public void OnGet(){
            var requestMedias = Query.Get("http://iotdata.yhdf.fr/api/web/medias");
            if(requestMedias == "Error" || String.IsNullOrEmpty(requestMedias)){
                IsErrorMedias = true;
            }
            else{
                IsErrorMedias = false;
                ResultMedias = JsonConvert.DeserializeObject<List<Medias>>(requestMedias); 
            }

            var requestFrame = Query.Get("http://iotdata.yhdf.fr/api/web/tables");
            if(requestFrame == "Error" || String.IsNullOrEmpty(requestFrame)){
                IsErrorFrame = true;
            }
            else{
                IsErrorFrame = false;
                ResultFrame = JsonConvert.DeserializeObject<List<Frame>>(requestFrame); 
                    foreach (var e in ResultFrame){
                    if (1==1){//condition de verification données
                        if(e.name=="wall"){
                                if(e.isOnScreen==true){
                                    timeWall = e.onScreenTime;
                                }
                                else{
                                    timeWall = 0;
                                }
                            }
                        else if(e.name=="news"){
                                if(e.isOnScreen==true){
                                    timeNews = e.carrousselTime;
                                }
                                else{
                                    timeNews = 0;
                                }
                            }
                            else if(e.name=="countdown"){
                                if(e.isOnScreen==true){
                                    timeCountdown = e.onScreenTime;
                                }
                                else{
                                    timeCountdown = 0;
                                }
                            }
                            else if(e.name=="medias"){
                                if(e.isOnScreen==true){
                                    timeMedias = e.carrousselTime;
                                }
                                else{
                                    timeMedias = 0;
                                }
                            }
                            else if(e.name=="socialnetworks"){
                                if(e.isOnScreen==true){
                                    timeSocial = e.onScreenTime;
                                }
                                else{
                                    timeSocial = 0;
                                }
                            }
                    }
                }
            }

            var requestSocials = Query.Get("http://iotdata.yhdf.fr/api/web/socials");
            if(requestSocials == "Error" || String.IsNullOrEmpty(requestSocials)){
                IsErrorSocial = true;
            }
            else{
                IsErrorSocial = false;
                ResultSocialnetworks = JsonConvert.DeserializeObject<List<Social>>(requestSocials); 
            }

            var requestNews = Query.Get("http://iotdata.yhdf.fr/api/web/events");
            if(requestNews == "Error" || String.IsNullOrEmpty(requestNews)){
                IsErrorNews = true;
            }
            else{
                IsErrorNews = false;
                ResultNews = JsonConvert.DeserializeObject<List<News>>(requestNews); 
            }

            var requestCountdown = Query.Get("http://iotdata.yhdf.fr/api/web/countdowns");
            if(requestCountdown == "Error" || String.IsNullOrEmpty(requestCountdown)){
                IsErrorCountdown = true;
            }
            else{
                IsErrorCountdown = false;
                ResultCountdown = JsonConvert.DeserializeObject<List<CountDown>>(requestCountdown); 
                    var currentTimeStamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    CountDown lastCountdown;
                    foreach(var e in ResultCountdown){
                        lastCountdown = e;
                        if(lastCountdown.endingDateEvent > e.endingDateCountdown){
                            lastCountdown = e;
                        }
                        if (lastCountdown.beginningDateEvent <= currentTimeStamp && lastCountdown.endingDateEvent >= currentTimeStamp){
                            ResultLastCountdown = lastCountdown;
                        }        
                    }
            }

            var requestWall = Query.Get("http://iotdata.yhdf.fr/api/web/sensors");
            if(requestWall == "Error" || String.IsNullOrEmpty(requestWall)){
                IsErrorWall = true;
            }
            else{
                IsErrorWall = false;
                ResultWall = JsonConvert.DeserializeObject<List<Sensors>>(requestWall); 
            }
        }
    }
}