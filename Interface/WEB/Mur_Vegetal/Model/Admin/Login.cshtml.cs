using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace Mur_Vegetal.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet(){
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){

                    Response.Redirect("/web/Admin/Admin");

                }
                else{
                }
            }
            if( Request.Cookies["administration"] != null ){
                var value = Request.Cookies["administration"];
                if (Auth.CalculateMD5Hash(Auth.AdminPass) == value){

                    Response.Redirect("/web/AdminWall");

                }
                else{
                }
            }
        }

        public void OnPost(){
            var login = Request.Form["login"];
            var password = Auth.CalculateMD5Hash(Request.Form["password"]);
            var submit = Request.Form["submit"];
            if (submit == "connexion"){
                if (login == "communication"){
                    if (password == Auth.CalculateMD5Hash(Auth.CommPass)){
                        var cookieOptions = new CookieOptions{
                            Expires = DateTime.Now.AddHours(1)
                        };
                        Response.Cookies.Append("communication",password , cookieOptions);
                        Response.Redirect("/Admin/Admin");
                    }
                }
                if (login == "administration"){
                    if (password == Auth.CalculateMD5Hash(Auth.AdminPass)){
                        var cookieOptions = new CookieOptions{
                            Expires = DateTime.Now.AddHours(1)
                        };
                        Response.Cookies.Append("administration",password , cookieOptions);
                        Response.Redirect("/AdminWall");
                    }
                }
            }
        }

    }
}
