using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SimpleHashing;
using Assignment2_WDT.Data;
using Assignment2_WDT.Models;
using Microsoft.AspNetCore.Http;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class for check password and login ID when the user login
 */

namespace Assignment2_WDT.Controllers
{
    [Route("/WDT/SecureLogin")]
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        public LoginController(DataContext context) => _context = context;


        //display login page for user to enter ID and password.
        public IActionResult Login() => View();


        //send the http require.
        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var login = await _context.Login.FindAsync(loginID);
            if (password == null)
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });             
            }
            else
            {
                if (login == null || !PBKDF2.Verify(login.PasswordHash, password))
                {
                    ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                    return View(new Login { LoginID = loginID });
                }
                else
                {
                    //If a user is locked, prevent that user from logging in.
                    var customer = _context.Customer.Find(login.CustomerID);
                    if (customer.Is_locked == true)
                    {
                        ModelState.AddModelError("Locked", "You account was locked. waitting 1 minute.");
                        return View(new Login { LoginID = loginID });
                    }

                    // Login customer.
                    HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
                    HttpContext.Session.SetString(nameof(Customer.CustomerName), login.Customer.CustomerName);
                    return RedirectToAction("Index", "Customer");
                }
            }
        }


        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
