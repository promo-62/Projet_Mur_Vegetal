using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;

namespace Mur_Vegetal.Pages
{
    public class NewsModel : PageModel{

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

        public bool IsError { get; private set; }
        public List <News> Result { get; private set; }
        public void OnGet(){
            
            var requestNews = Query.Get("http://iotdata.yhdf.fr/api/web/events");
            if(requestNews == "Error" || String.IsNullOrEmpty(requestNews)){
                IsError = true;
            }
            else{
                IsError = false;
                Result = JsonConvert.DeserializeObject<List<News>>(requestNews); 
            }
        }
    }
}
