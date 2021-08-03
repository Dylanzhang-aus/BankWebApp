using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * this controller class to handle once the admin user login as an admin 
 */

namespace AdminWeb.Controllers
{
    [Route("/WDT/AdminLogin")]
    public class LoginController : Controller
    {   
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string loginID, string password)
        {
            var login = loginID;
            var pass = password;
            if (login != "admin" || pass != "admin")
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View();
            }

            HttpContext.Session.SetString("username", login);

            return RedirectToAction("AdminView", "Admin");
        }

        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout admin.
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
