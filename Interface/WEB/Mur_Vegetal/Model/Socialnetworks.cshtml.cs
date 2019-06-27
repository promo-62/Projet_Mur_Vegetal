using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;

namespace Mur_Vegetal.Pages
{
    public class SocialnetworksModel : PageModel
    {
        public class Social
        {
            public string username { get; set; }
            public string pageWidget { get; set; }
            public string widget { get; set; }
            public string id { get; set; }
        }
        public List <Social> Result { get; private set; }
        public bool IsError { get; private set; }
        public void OnGet(){
            var requestSocialnetworks = Query.Get("http://iotdata.yhdf.fr/api/web/socials");
            if(requestSocialnetworks == "Error" || String.IsNullOrEmpty(requestSocialnetworks)){
                IsError = true;
            }
            else{
                IsError = false;
                Result = JsonConvert.DeserializeObject<List<Social>>(requestSocialnetworks); 
            }
        }
    }
}
