using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Mur_Vegetal.Pages
{
    public class IndexModel : PageModel
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

        public class Social
        {
            public string username { get; set; }
            public string pageWidget { get; set; }
            public string widget { get; set; }
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
        public string _ResultViewCountdown {get; private set;}
        public string _ResultViewNews { get; private set; }
        public string _ResultViewWall {get; private set;}
        public string _ResultViewSocialnetworks {get; private set;}

        public void OnGet()
        {
            var currentTimeStamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //Call request
            var requestWall = Query.Get("http://iotdata.yhdf.fr/api/web/sensors");
            if(requestWall == "Error" || String.IsNullOrEmpty(requestWall)){
                _ResultViewWall = "<div>Error API</div>";
            }
            else {
                var resultWall = JsonConvert.DeserializeObject<List<Sensors>>(requestWall);
                _ResultViewWall = "";
                foreach(var e in resultWall){
                    if(e.idSensorType==0){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/air.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Qualité d'air : </a></div> </div> ";
                    }
                    if(e.idSensorType==1){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/thermo.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Température : </a></div> </div> ";
                    }
                    if(e.idSensorType==2){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/pression.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Commande pompe : </a></div> </div> ";
                    }
                    if(e.idSensorType==3){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/hydro.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Humidité : </a></div> </div> ";
                    }
                    if(e.idSensorType==4){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/pression_ruche.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Pression ruche : </a></div> </div> ";
                    }
                    if(e.idSensorType==5){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/mouvement.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Flux entrant/sortant : </a></div> </div> ";
                    }
                    if(e.idSensorType==6){
                        _ResultViewWall += "<div class=\"wall-info\"> <div class=\"wall-icon\"><a href=\"~/Wall\"> <img src=\"/web/images/icones/thermo_ruche.png\"/> </a></div> <div class=\"wall-text\"><a href=\"~/Wall\"> Température ruche :  </a></div> </div> ";
                    }
                }
            }

            var requestSocial = Query.Get("http://iotdata.yhdf.fr/api/web/socials");
            if(requestSocial == "Error" || String.IsNullOrEmpty(requestSocial)){
                _ResultViewSocialnetworks = "<div>Error API</div>";
            }
            else {
                _ResultViewSocialnetworks = "";
                var resultSocial = JsonConvert.DeserializeObject<List<Social>>(requestSocial);
                foreach(var e in resultSocial){
                    if(e.pageWidget == "accueil"){
                        _ResultViewSocialnetworks += "<div class=\"socialnetworks-box box\"> <a href=\"~/Socialnetworks\"> <script src=\"https://snapwidget.com/js/snapwidget.js\"></script> <iframe src=\" " + e.widget + " \" class=\"snapwidget-widget\"></iframe> </a></div>";
                    }
                }
            }


            var requestCountdown = Query.Get("http://iotdata.yhdf.fr/api/web/countdowns");
            if(requestCountdown=="Error" || String.IsNullOrEmpty(requestCountdown)){
                _ResultViewCountdown = "<div>Error Api</div>";
            }
            else {
                var resultCountdown = JsonConvert.DeserializeObject<List<CountDown>>(requestCountdown);
                _ResultViewCountdown = "";
                CountDown lastCountdown = resultCountdown[0];
                foreach(var e in resultCountdown){
                    Console.Write(lastCountdown.endingDateEvent);
                    Console.Write(e.endingDateEvent);
                    if(lastCountdown.endingDateEvent >= e.endingDateEvent){
                        lastCountdown = e;
                    }
                    if (lastCountdown.beginningDateEvent <= currentTimeStamp && lastCountdown.endingDateEvent >= currentTimeStamp){
                        _ResultViewCountdown = "<div class=\"countdown-box \"> <a href=\"~/Countdown\"> <div class=\"countdown-image box\"> <img src=\" data:image/png;base64, " + lastCountdown.image + "  \"/> </div> <div class=\"countdown-text box \"> " + lastCountdown.text + " <div id=\"countdown-display\"> </div> <script> countDown(\" " + lastCountdown.endingDateCountdown + "  \",\"countdown-display\"); </script> </div> </a> </div> ";
                    }
                    else {
                    }
                }
            }


            var requestNews = Query.Get("http://iotdata.yhdf.fr/api/web/events");
            if(requestNews=="Error" || String.IsNullOrEmpty(requestNews)){
                _ResultViewNews = "<div>Error Api</div>";
            }
            else {
                _ResultViewNews = "";
                var resultNew = JsonConvert.DeserializeObject<List<News>>(requestNews);
                News lastNews = resultNew[0];
                foreach(var e in resultNew){
                    if(lastNews.beginningDate <= e.beginningDate){
                        lastNews = e;
                    }
                    if (lastNews.beginningDate <= currentTimeStamp && lastNews.endingDate >= currentTimeStamp){
                        if(String.IsNullOrEmpty(lastNews.text)){
                            _ResultViewNews = "<a href=\"~/News\"><div class=\"news-box\"> <div class=\"news-image box\"> <img src=\"data:image/png;base64, " +lastNews.eventImage + " \" alt=\" " + lastNews.name + " \"> </div></div></a>";
                        }
                        else {
                            _ResultViewNews = "<a href=\"~/News\"><div class=\"news-box\"> <div class=\"news-image box\"> <img src=\"data:image/png;base64, " +lastNews.eventImage + " \" alt=\" " + lastNews.name + " \"> </div><div class=\"news-text box\"> "+lastNews.text +"  </div> </div></a> ";
                        }
                    }
                    else {
                    }
                }
            }

        }
    }
}
