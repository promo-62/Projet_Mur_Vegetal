using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;

namespace Mur_Vegetal.Pages{
    public class FrameAdminModel : PageModel{
        public class Frame{
            public int onScreenTime { get; set; }
            public bool isOnScreen { get; set; }
            public string name { get; set; }
            public int carrousselTime { get; set; }
            public string id { get; set; }
        }

        public List <Frame> Result { get; private set; }
        public bool IsError { get; private set; }
        public void OnGet(){
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){
                    var requestFrame = Query.Get("http://iotdata.yhdf.fr/api/web/tables");
                    if(requestFrame == "Error" || String.IsNullOrEmpty(requestFrame)){
                        IsError = true;
                    }
                    else{
                        IsError = false;
                        Result = JsonConvert.DeserializeObject<List<Frame>>(requestFrame); 
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
            var isOnScreen = Request.Form["isOnScreen"];
            var submit = Request.Form["submit"];
            var time = Request.Form["time"];
            var name = Request.Form["name"];
            var id = Request.Form["id"];
            var result = "";
            if(submit == "edit"){
                Frame toEdit = new Frame();
                if (name == "news" || name == "medias"){
                    toEdit.carrousselTime = Int32.Parse(time);
                }
                else if (name == "countdown" || name == "socialnetworks" || name == "wall"){
                    toEdit.onScreenTime = Int32.Parse(time);
                }
                if (isOnScreen == "on"){
                    toEdit.isOnScreen = true;
                }
                else {
                    toEdit.isOnScreen = false;
                }
                toEdit.name = name;
                toEdit.id = id;
                var data =  JsonConvert.SerializeObject(toEdit);
                result = Query.Put("http://iotdata.yhdf.fr/api/web/tables/"+toEdit.id,data);
            }
            if(result=="Error"){
                //Code Alert
            }
            OnGet();
        }
    }
}