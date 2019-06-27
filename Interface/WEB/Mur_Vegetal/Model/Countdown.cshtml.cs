using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Mur_Vegetal.Pages
{
    public class CountdownModel : PageModel{

        public class CountDown{
            public string text { get; set; }
            public string name { get; set; }
            public int endingDateEvent { get; set; }
            public int beginningDateEvent { get; set; }
            public int endingDateCountdown { get; set; }
            public int position { get; set; }
            public string image { get; set; }
            public string id { get; set; }
        }
        public List <CountDown> ResultCountdown { get; private set; }
        public CountDown ResultLastCountdown {get; private set;}
        public bool IsError { get; private set; }
        public void OnGet(){
            var requestCountdown = Query.Get("http://iotdata.yhdf.fr/api/web/countdowns");
            if(requestCountdown == "Error" || String.IsNullOrEmpty(requestCountdown)){
                IsError = true;
            }
            else{
                IsError = false;
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
        }
    }
}
