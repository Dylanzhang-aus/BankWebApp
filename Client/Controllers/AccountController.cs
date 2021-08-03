using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Assignment2_WDT.Models;
using Microsoft.AspNetCore.Http;
using Assignment2_WDT.Filters;
using Assignment2_WDT.Utilities;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class controls all activity in an account session, such as ATM Server.
 */

namespace Assignment2_WDT.Controllers
{
    [AuthorizeCustomer]
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private int AccountNumber => HttpContext.Session.GetInt32(nameof(Account.AccountNumber)).Value;
        public AccountController(DataContext context) => _context = context;

    
        public async Task<IActionResult> ViewAccount()
        {
            var account = await _context.Account.FindAsync(AccountNumber);           
            return View(account);   
        }


        public IActionResult BackCustomer()
        {
           
            return RedirectToAction("Index", "Customer");
        }


        // Deposit money view.
        public async Task<IActionResult> Deposit() => View(await _context.Account.FindAsync(AccountNumber));


        // deposit send http require.
        [HttpPost]
        public async Task<IActionResult> Deposit(decimal amount)
        {
            var account = await _context.Account.FindAsync(AccountNumber);
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);    
            }

            account.Deposit(amount);
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    ModifyDate = DateTime.UtcNow.ToLocalTime(),
                    Is_New = true,
                    Is_Debit = false
                });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewAccount));
        }



        //Withdraw money view.
        public async Task<IActionResult> Withdraw()
        {
            var account = await _context.Account.FindAsync(AccountNumber);
            return View(account);
        }


        // send withdraw http require.
        [HttpPost]
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            //checking the user input.
            var account = await _context.Account.FindAsync(AccountNumber);
            if (amount <= 0)
            {
                ModelState.AddModelError(nameof(amount), "Amount must be positive");
            }
            if (amount.HasMoreThanTwoDecimalPlaces())
            {
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            //checking the account type.
            //if type is saving, the minimue balance is 0, else the minimue balance is 200.
            if (account.AccountType.Equals(AccountType.Saving))
            {
                var result = account.Withdraw(amount, 0, 0.1m);
                if (result == false)
                {
                    ModelState.AddModelError("WithDrawFailed", "Withdraw failed, Check you balance and amount.");
                    ViewBag.Amount = amount;
                    return View(account);
                }
                else
                {
                    //when the withdraw success, add new transaction.
                    account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.Withdraw,
                            AccountNumber = account.AccountNumber,
                            Amount = amount,
                            Comment = "Withdraw server",
                            ModifyDate = DateTime.UtcNow.ToLocalTime(),
                            Is_New = true,
                            Is_Debit = true
                        });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewAccount));
                }
            }
            else
            {
                var result = account.Withdraw(amount, 200, 0.1m);
                if (result == false)
                {
                    ModelState.AddModelError("WithDrawFailed", "Failed, Check you balance and amount.");
                    ViewBag.Amount = amount;
                    return View(account);
                }
                else
                {
                    account.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.Withdraw,
                            AccountNumber = account.AccountNumber,
                            Amount = amount,
                            Comment = "Withdraw server",
                            ModifyDate = DateTime.UtcNow.ToLocalTime(),
                            Is_New = true,
                            Is_Debit = true
                        });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewAccount));
                }
            }
        }


        // show my statement, send http get require.
        public async Task<IActionResult> ViewTransaction(int? page = 1)
        {
            var account = await _context.Account.FindAsync(AccountNumber);
            if (account == null)
                return RedirectToAction(nameof(ViewAccount)); // (Index)
            ViewBag.Account = account;

            //sort transaction by ModifyDate
            var transactions = _context.Transaction.OrderByDescending(x => x.ModifyDate);

            // Page the orders, maximum of 4 per page.
            const int pageSize = 4;
            var pagedList = await transactions.Where(x => x.AccountNumber == AccountNumber).
            ToPagedListAsync(page, pageSize);
                        
            return View(pagedList);
        }


        //To show the Transfer money view
        public async Task<IActionResult> Transfer()
        {
            var account = await _context.Account.FindAsync(AccountNumber);
            return View(account);
        }


        //send the transfer require.
        [HttpPost]
        public async Task<IActionResult> Transfer(int destAccountNumber, decimal amount, string comment)
        {
            //loading the account and destination account in the database.
            var account = await _context.Account.FindAsync(AccountNumber);
            var destAccount = await _context.Account.FindAsync(destAccountNumber);

            //check the user input.
            if (amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount must be positive");
                return View(account);
            }
            if (amount.HasMoreThanTwoDecimalPlaces())
            {
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
                return View(account);
            }
            if (destAccount == null || destAccountNumber == AccountNumber)
            {
                ModelState.AddModelError("DestAccountNumber", "invalid Account Number.");
                return View(account);
            }

            //check the account type.
            if (account.AccountType.Equals(AccountType.Checking))
            {
                //call the method in model to withdraw money from account.
                var result = account.Withdraw(amount, 200, 0.2m);
                if (result == false)
                {
                    ModelState.AddModelError("FailedTransfer", "Filed, Check you amount and Balance.");
                    return View(account);
                }
                else
                {
                    //call the method in model to deposit money into destination account.
                    destAccount.Deposit(amount);

                    //add trancation into both accounts.
                    account.Transactions.Add(
                    new Transaction
                    {
                        TransactionType = TransactionType.Transfer,
                        AccountNumber = account.AccountNumber,
                        DestAccountNumber = destAccountNumber,
                        Amount = amount,
                        Comment = comment,
                        ModifyDate = DateTime.UtcNow.ToLocalTime(),
                        Is_New = true,
                        Is_Debit = true
                    });
                    destAccount.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.Transfer,
                            AccountNumber = account.AccountNumber,
                            Amount = amount,
                            Comment = comment,
                            ModifyDate = DateTime.UtcNow.ToLocalTime(),
                            Is_New = true,
                            Is_Debit = false
                        });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewAccount));
                }
            }
            else
            {
                var result = account.Withdraw(amount, 0, 0.2m);
                if (result == false)
                {
                    ModelState.AddModelError("FailedTransfer", "Check you amount and Balance.");
                    return View(account);
                }          
                else
                {
                    destAccount.Deposit(amount);
                    account.Transactions.Add(
                     new Transaction
                     {
                        TransactionType = TransactionType.Transfer,
                        AccountNumber = account.AccountNumber,
                        DestAccountNumber = destAccountNumber,
                        Amount = amount,
                        Comment = comment,
                        ModifyDate = DateTime.UtcNow.ToLocalTime(),
                        Is_New = true,
                        Is_Debit = true
                     });

                    destAccount.Transactions.Add(
                        new Transaction
                        {
                            TransactionType = TransactionType.Transfer,
                            AccountNumber = account.AccountNumber,
                            Amount = amount,
                            Comment = comment,
                            ModifyDate = DateTime.UtcNow.ToLocalTime(),
                            Is_New = true,
                            Is_Debit = false
                        });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ViewAccount));
                }
            }  
        }


        //display create billPay view.
        public IActionResult BillPays()
        {
            ViewData["AccountNumber"] = new SelectList(_context.Account, "AccountNumber", "AccountNumber");
            ViewData["PayeeID"] = new SelectList(_context.Payee, "PayeeID", "PayeeName");

            //create a view bag box for user selection.
            List<string> PeriodSelect = new List<string>
            {
                "M",
                "O",
                "Q"
            };
            ViewData["Period"] = new SelectList(PeriodSelect);
            return View();            
        }


        //send create http require.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BillPays([Bind("BillPayID,AccountNumber,PayeeID,Amount,ScheduleDate,Period,ModifyDate, Is_blocked")] BillPay billPay)
        {
            var account = await _context.Account.FindAsync(AccountNumber);

            //Initializes the necessary properties in BillPay.
            billPay.AccountNumber = account.AccountNumber;
            billPay.ModifyDate = DateTime.UtcNow.ToLocalTime();
            billPay.Is_blocked = false;

            if (billPay.Amount <= 0 || account.Balance < billPay.Amount)
               ModelState.AddModelError("AmountError", "invalid Amount or the amount exceeded the minimum balance.");

            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32(nameof(BillPay.AccountNumber), account.AccountNumber);
                return RedirectToAction("ViewPayBill", "BillPay");
            }

            //If the addition is not successful, you need to return the page.
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



        public async Task<IActionResult> MyBillPays()
        {
            var account = await _context.Account.FindAsync(AccountNumber);

            HttpContext.Session.SetInt32(nameof(BillPay.AccountNumber), account.AccountNumber);

            return RedirectToAction("ViewPayBill", "BillPay");
        }


        public IActionResult Privacy()
        {
            return View("Privacy");
        }
    }
}
