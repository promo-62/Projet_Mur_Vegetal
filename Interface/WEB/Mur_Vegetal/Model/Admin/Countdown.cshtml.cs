using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Mur_Vegetal.Pages
{
    public class CountdownAdminModel : PageModel{
        public class CountDown{
            public string text { get; set; }
            public string name { get; set; }
            public int endingDateEvent { get; set; }
            public int beginningDateEvent { get; set; }
            public double endingDateCountdown { get; set; }
            public int position { get; set; }
            public string image { get; set; }
            public string id { get; set; }
        }

         public List <CountDown> Result { get; private set; }

         public IFormFile ImageUpload { get; set; }

         public bool IsError { get; private set; }
        public void OnGet(){
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){
                    var requestCountdown = Query.Get("http://iotdata.yhdf.fr/api/web/countdowns");
                    if(requestCountdown == "Error" || String.IsNullOrEmpty(requestCountdown)){
                        IsError = true;
                    }
                    else{
                        IsError = false;
                        Result = JsonConvert.DeserializeObject<List<CountDown>>(requestCountdown);
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
            var startdate = (Convert.ToDateTime(Request.Form["startdate"]).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var enddate = (Convert.ToDateTime(Request.Form["enddate"]).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var countdowndate = (Convert.ToDateTime(Request.Form["countdowndate"]).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;//transform string to date
            var countdowntime = TimeSpan.Parse(Request.Form["countdowntime"]).TotalSeconds;//transform string to time
            var totalcountdown = countdowndate + countdowntime - 7200; //mix date + time
            var submit = Request.Form["submit"];
            var id = Request.Form["id"];
            var result ="";
            if(submit == "add"){
                CountDown toAdd = new CountDown();
                toAdd.name = name;
                toAdd.beginningDateEvent = Convert.ToInt32(startdate);
                toAdd.endingDateEvent = Convert.ToInt32(enddate);
                toAdd.endingDateCountdown = totalcountdown;
                toAdd.text = text;
                if(ImageUpload != null){
                    using (var ms = new MemoryStream()){
                    ImageUpload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    toAdd.image = Convert.ToBase64String(fileBytes);
                    }
                }
                else{
                    toAdd.image = "";
                }
                var data =  JsonConvert.SerializeObject(toAdd);
                result = Query.Post("http://iotdata.yhdf.fr/api/web/countdowns/",data);
            }
            else if(submit == "edit"){
                var image = Request.Form["image"];
                CountDown toEdit = new CountDown();
                if (!String.IsNullOrEmpty(image)){
                    toEdit.image = image;
                }
                else {
                    toEdit.image = "";
                }
                toEdit.text = text;
                toEdit.name = name;
                toEdit.beginningDateEvent = Convert.ToInt32(startdate);
                toEdit.endingDateEvent = Convert.ToInt32(enddate);
                toEdit.endingDateCountdown = totalcountdown;
                toEdit.id = id;
                var data =  JsonConvert.SerializeObject(toEdit);
                result = Query.Put("http://iotdata.yhdf.fr/api/web/countdowns/"+toEdit.id,data);
            }
            else if(submit == "delete"){
                result = Query.Delete("http://iotdata.yhdf.fr/api/web/countdowns/"+id);
            }
            if(result=="Error"){
                //Code Alert
            }
            OnGet();
        }
    }
}
