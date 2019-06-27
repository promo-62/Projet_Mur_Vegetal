using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Mur_Vegetal.Pages
{
    public class MediasModel : PageModel{
        public class Medias
        {
            public string name { get; set; }
            public int beginningDate { get; set; }
            public int endingDate { get; set; }
            public string video { get; set; }
            public string image { get; set; }
            public string id { get; set; }
        }
        public bool IsError { get; private set; }
        public List <Medias> Result {get; private set;}

        public Match RegexMatch {get; set;}
        public void OnGet(){

            var requestMedias = Query.Get("http://iotdata.yhdf.fr/api/web/medias");
            if(requestMedias == "Error" || String.IsNullOrEmpty(requestMedias)){
                IsError = true;
            }
            else{
                IsError = false;
                Result = JsonConvert.DeserializeObject<List<Medias>>(requestMedias); 
            }
        }

    }
}
