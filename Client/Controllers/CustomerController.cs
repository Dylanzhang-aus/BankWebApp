using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Assignment2_WDT.Models;
using Microsoft.AspNetCore.Http;
using Assignment2_WDT.Filters;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class controls all activities in the customer class. For example, select to view the account.
 */

namespace Assignment2_WDT.Controllers
{
    [AuthorizeCustomer]
    public class CustomerController : Controller
    {
        private readonly DataContext _context;  
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public CustomerController(DataContext context) => _context = context;


        //display the customer view.
        public async Task<IActionResult> Index()
        {
            
            var customer = await _context.Customer.FindAsync(CustomerID);
            return View(customer);
        }


        //allow user to select which account they want to chose.
        public async Task<IActionResult> Select(int id)
        {
             var account = await _context.Account.FindAsync(id);            
             HttpContext.Session.SetInt32(nameof(Account.AccountNumber), account.AccountNumber);          
             return RedirectToAction("ViewAccount", "Account");
        }
    }
}
