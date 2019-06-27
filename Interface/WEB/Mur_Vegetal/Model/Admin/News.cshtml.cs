using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Mur_Vegetal.Pages
{
    public class NewsAdminModel : PageModel{
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

        public List <News> Result { get; private set; }

        public IFormFile ImageUpload { get; set; }
        public bool IsError { get; private set; }
        public void OnGet(){
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){
                    var requestNews = Query.Get("http://iotdata.yhdf.fr/api/web/events");
                    if(requestNews == "Error" || String.IsNullOrEmpty(requestNews)){
                        IsError = true;
                    }
                    else{
                        IsError = false;

                        Result = JsonConvert.DeserializeObject<List<News>>(requestNews);
                    }
                }
                else{
                    Response.Redirect("/web/Admin/Login");
                }
            }
            else {
                Response.Redirect("/web/Admin/Login");

            }
        }

        public void OnPost(){
            var name = Request.Form["name"];
            var text = Request.Form["text"];
            var startdate = (((DateTimeOffset)DateTime.Parse(Request.Form["startdate"])).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var enddate = (((DateTimeOffset)DateTime.Parse(Request.Form["enddate"])).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var submit = Request.Form["submit"];
            var id = Request.Form["id"];
            var result = "";
            if(submit == "add"){
                News toAdd = new News();
                toAdd.name = name;
                toAdd.text = text;
                toAdd.beginningDate = Convert.ToInt32(startdate);
                toAdd.endingDate = Convert.ToInt32(enddate);
                using (var ms = new MemoryStream()){
                    ImageUpload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    toAdd.eventImage = Convert.ToBase64String(fileBytes);
                }
                var data =  JsonConvert.SerializeObject(toAdd);
                Console.WriteLine(text);
                result = Query.Post("http://iotdata.yhdf.fr/api/web/events/",data);
            }
            else if(submit == "edit"){
                var image = Request.Form["image"];
                News toEdit = new News();
                toEdit.eventImage = image;
                toEdit.name = name;
                toEdit.text = text;
                toEdit.beginningDate = Convert.ToInt32(startdate);
                toEdit.endingDate = Convert.ToInt32(enddate);
                toEdit.id = id;
                var data =  JsonConvert.SerializeObject(toEdit);
                result = Query.Put("http://iotdata.yhdf.fr/api/web/events/"+toEdit.id,data);
            }
            else if (submit == "delete"){
                result = Query.Delete("http://iotdata.yhdf.fr/api/web/events/"+id);
            }
            if(result=="Error"){
                //Code Alert
            }
            OnGet();
        }
    }

}

