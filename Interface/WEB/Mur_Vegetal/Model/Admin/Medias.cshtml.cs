using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace Mur_Vegetal.Pages{
    public class MediasAdminModel : PageModel{
         public class Medias{
            public string name { get; set; }
            public int beginningDate { get; set; }
            public int endingDate { get; set; }
            public string video { get; set; }
            public string image { get; set; }
            public string id { get; set; }
        }
        public IFormFile ImageUpload { get; set; }
        public List <Medias> Result { get; private set; }
        public bool IsError { get; private set; }
        public void OnGet(){
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){
                    var requestMedias = Query.Get("http://iotdata.yhdf.fr/api/web/medias");
                    if(requestMedias == "Error" || String.IsNullOrEmpty(requestMedias)){
                        IsError = true;
                    }
                    else{
                        IsError = false;
                        Result = JsonConvert.DeserializeObject<List<Medias>>(requestMedias); 
                    }
                }
                else{
                    Response.Redirect("/Admin/Login");
                }
            }
            else {
                Response.Redirect("/Admin/Login");
            }
        }
        public void OnPost(){
            var name = Request.Form["name"];
            var video = Request.Form["video"];
            var startdate = (((DateTimeOffset)DateTime.Parse(Request.Form["startdate"])).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var enddate = (((DateTimeOffset)DateTime.Parse(Request.Form["enddate"])).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var submit = Request.Form["submit"];
            var id = Request.Form["id"];
            var result ="";
            if(submit == "add"){
                Medias toAdd = new Medias();
                toAdd.name = name;
                toAdd.beginningDate = Convert.ToInt32(startdate);
                toAdd.endingDate = Convert.ToInt32(enddate);
                if (ImageUpload != null){
                    using (var ms = new MemoryStream()){
                    ImageUpload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    toAdd.image = Convert.ToBase64String(fileBytes);
                    }
                    toAdd.video = "";
                }
                else if (!String.IsNullOrEmpty(video)){
                    toAdd.image = "";
                    toAdd.video = video;
                }
                var data =  JsonConvert.SerializeObject(toAdd);
                result = Query.Post("http://iotdata.yhdf.fr/api/web/medias/",data);
            }
            else if(submit == "edit"){
                var image = Request.Form["image"];
                Medias toEdit = new Medias();
                if (!String.IsNullOrEmpty(video)){
                    toEdit.image = "";
                    toEdit.video = video;
                }
                else if (!String.IsNullOrEmpty(image)){
                    toEdit.image = image;
                    toEdit.video = "";
                }
                toEdit.name = name;
                toEdit.beginningDate = Convert.ToInt32(startdate);
                toEdit.endingDate = Convert.ToInt32(enddate);
                toEdit.id = id;
                var data =  JsonConvert.SerializeObject(toEdit);
                result = Query.Put("http://iotdata.yhdf.fr/api/web/medias/"+toEdit.id,data);
            }
            else if (submit == "delete"){
                result = Query.Delete("http://iotdata.yhdf.fr/api/web/medias/"+id);
            }
            if(result=="Error"){
                //Code Alert
            }
            OnGet();
        }
    }
}