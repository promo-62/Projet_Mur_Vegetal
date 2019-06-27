using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mur_Vegetal.Pages
{
    public class AdminModel : PageModel
    {
        public void OnGet()
        {
            if( Request.Cookies["communication"] != null ){
                var value = Request.Cookies["communication"].ToString();
                if (Auth.CalculateMD5Hash(Auth.CommPass) == value){
                }
                else{
                    Response.Redirect("/web/Admin/Login");
                }
            }
            else {
                Response.Redirect("/web/Admin/Login");
            }
        }
    }
}
