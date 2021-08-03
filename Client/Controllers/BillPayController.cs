using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Assignment2_WDT.Models;
using Microsoft.AspNetCore.Http;
using Assignment2_WDT.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class controls all activities in the BillPay class. For example, delete and modify billpay.
 */

namespace Assignment2_WDT.Controllers
{
    [AuthorizeCustomer]
    public class BillPayController : Controller
    {
        private readonly DataContext _context;       
        private int AccountNumber => HttpContext.Session.GetInt32(nameof(BillPay.AccountNumber)).Value;


        public BillPayController(DataContext context)
        {
           _context = context;    
        }


        public async Task<IActionResult> ViewPayBill()
        {
            var billPay = await _context.BillPay.Where(b => b.AccountNumber == AccountNumber).ToListAsync();
            return View(billPay);
        }



        public IActionResult BackAccount()
        {
            return RedirectToAction("ViewAccount", "Account");
        }


        //allow user to modify the information of their billpay
        public async Task<IActionResult> Modify(int id)
        {         
            var billPay = await _context.BillPay.FindAsync(id);
            ViewData["AccountNumber"] = new SelectList(_context.Account, "AccountNumber", "AccountNumber", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeName", billPay.PayeeID);
            List<string> PeriodSelect = new List<string>
            {
                "O",
                "Q",
                "M"
            };
            ViewData["Period"] = new SelectList(PeriodSelect);
            return View(billPay);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(int id, [Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleDate,Period,ModifyDate, Is_blocked")] BillPay billPay)
        {
            var account = await _context.Account.FindAsync(AccountNumber);
            if(billPay.Amount <= 0 || account.Balance < billPay.Amount)
                ModelState.AddModelError("AmountError", "Invalid Amount.");

            if (ModelState.IsValid)
            {               
                _context.Update(billPay);
                await _context.SaveChangesAsync();             
                return RedirectToAction(nameof(ViewPayBill));
            }
            ViewData["AccountNumber"] = new SelectList(_context.Account, "AccountNumber", "AccountNumber", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeName", billPay.PayeeID);
            List<string> PeriodSelect = new List<string>
            {
                "O",
                "Q",
                "M"
            };
            ViewData["Period"] = new SelectList(PeriodSelect);
            return View(billPay);
        }



        //allow user to delete their billpay.
        public async Task<IActionResult> Delete(int id)
        {           
            var billPay = await _context.BillPay
                .Include(b => b.Account)
                .Include(b => b.Payee)
                .FirstOrDefaultAsync(m => m.BillPayID == id);
            return View(billPay);
        }


        //allow user to make sure if they really want to delete this billpay.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var billPay = await _context.BillPay.FindAsync(id);
            _context.BillPay.Remove(billPay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewPayBill));
        }
    }
}
