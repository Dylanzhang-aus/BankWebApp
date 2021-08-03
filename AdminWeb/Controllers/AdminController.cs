using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using AdminWeb.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using AdminWeb.Filters;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * Show all menu once admin login successfully and All methods of View the statements, lock/unlock the user and blok the bill payments
 */

namespace AdminWeb.Controllers
{
    [AuthorizeAdmin]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        private Dictionary<int,List<Transaction>> TransactionsBox { get; set; }

        public AdminController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            TransactionsBox = new Dictionary<int, List<Transaction>>();
        }


        // GET: /<controller>/
        public async Task<IActionResult> AdminView()
        {
            var response = await Client.GetAsync("api/Admin");
           
            if (!response.IsSuccessStatusCode)
                throw new Exception();

            // Storing the response details received from web api.
            var result = await response.Content.ReadAsStringAsync();
            // Deserializing the response received from web api and storing into a list.
            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);         
           
            return View(customers);
        }


        // search method for transactions of one customer.
        [HttpGet]
        public async Task<IActionResult> Search(int? id)
        {
                //loading customer from database by using bankApi.
                var response = await Client.GetAsync("api/Admin/"+ id);
           
                if (!response.IsSuccessStatusCode)
                    throw new Exception();

                var result = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(result);
                var transactionList = new List<Transaction>();

                //looping the account list of this customer to add all transactions.
                foreach (var a in customer.Accounts)
                {
                  foreach (var t in a.Transactions)
                  {
                    transactionList.Add(t);
                  }
                }

            //add this transaction list and customerID into the dictionary.
            //cause I have to pass id for next step.
            TransactionsBox.Add(customer.CustomerID, transactionList);
            return View("SearchView", TransactionsBox);         
        }


        // bill payment methods, same thing with search transations.
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> SchedulePay(int? id)
        {   
            var response = await Client.GetAsync("api/Admin/" + id);

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            var billPayList = new List<BillPay>();
            foreach (var a in customer.Accounts)
            {
                foreach (var billpay in a.BillPays)
                {
                    billPayList.Add(billpay);
                }
            }
            return View("SchedulePayView", billPayList);
        }


        // blok the bill payments.
        // GET: Block/Edit/1
        public async Task<IActionResult> Block(int? id)
        {
            if (id == null)
                return NotFound();

            //get this billpay from database by using BankApi.
            var response = await Client.GetAsync("api/BillPay/" + id);

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var paybill = JsonConvert.DeserializeObject<BillPay>(result);

            return View("Block", paybill);
        }



        // POST: Movies/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Block(int id, BillPay billPay)
        {
            if (id != billPay.BillPayID)
                return NotFound();

            if (ModelState.IsValid)
            {
                //update the billpay by using api.
                var content = new StringContent(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");
                var response = Client.PutAsync($"api/BillPay/{id}", content).Result;

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("AdminView");
            }
            return View(billPay);
        }


        //release all account infromation for a customer.
        [HttpGet]
        public async Task<IActionResult> AccountView(int? id)
        {
            var response = await Client.GetAsync("api/Admin/" + id);

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);           
            return View("AccountView", customer.Accounts);

        }


        //loack a account which belong to a customer.
        public async Task<IActionResult> Lock(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await Client.GetAsync("api/Admin/"+id);
            //add a selection box for use to choose.
            List<bool> optionBox = new List<bool>
            {
                { false},
                { true }
            };
            ViewData["Lock"] = new SelectList(optionBox);
            if (!response.IsSuccessStatusCode)
                throw new Exception();
            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            //return the customer which returned by api.
            return View(customer);
        }


        //send the users input to api for updating the customer object.
        //because whatever admin chooses which account to lock,
        //then the account owner will not be able to log in. So lock the Customer Object directly.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id , [Bind("CustomerID,CustomerName,TFN,Address,City,State,PostCode,Phone, Is_NewCustomer, EmailAdress, Is_locked ")] Customer customer)
        {
            if (id != customer.CustomerID)
                return NotFound();

            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
                var response = Client.PutAsync($"api/Admin/{id}", content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ToString());
                }
                else
                {
                    return await AccountView(id);     
                }
            }
            return View(customer);
        }



        //display a time selection box for user to choose time period instead of enter a time period.
        public IActionResult PickTime(int id)
        {
            DatePicker dp = new DatePicker(id,DateTime.UtcNow.ToLocalTime(), DateTime.UtcNow.ToLocalTime());
            return View("PickTimeView",dp);
        }


        //return the time period to http require.
        [HttpPost]
        public async Task<IActionResult> PickTime(int id, DateTime startTime, DateTime endTime)
        {
                      
            var response = await Client.GetAsync("api/Admin/" + id);

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(result);

            //create a templete transaction box to store the accepted transaction.
            var transactionList = new List<Transaction>();

            //looping the transactions which belong to this customer
            //and if acceptable, add into temp box.
            foreach (var a in customer.Accounts)
            {
                foreach (var t in a.Transactions)
                {
                    transactionList.Add(t);
                }
            }
            var PrintList = new List<Transaction>();
            foreach (var t in transactionList)
            {
                if(t.ModifyDate.CompareTo(startTime)>=0 && t.ModifyDate.CompareTo(endTime)<=0)
               {
                    PrintList.Add(t);
                }
            }
            return View(PrintList);
        }



        // show all user's statements or transactions in one page.
        public async Task<IActionResult> AllTransaction()
        {
            var response = await Client.GetAsync("api/Admin/");

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);
            var transactionList = new List<Transaction>();

            foreach (var c in customers)
            {
                foreach (var a in c.Accounts)
                {
                    foreach (var t in a.Transactions)
                    {
                        transactionList.Add(t);
                    }
                }
            }

            //the last new one should be first.
            var orderedList= transactionList.OrderByDescending(x => x.ModifyDate);
            return View("AllTransactionView", orderedList);
        }



        //same approach with pack time method.
        public IActionResult GetTransaction()
        {
            DatePicker dp = new DatePicker(1,DateTime.UtcNow.ToLocalTime(), DateTime.UtcNow.ToLocalTime());
            return View("PickTimeAllView", dp);
        }



        [HttpPost]
        public async Task<IActionResult> GetTransaction(DateTime startTime, DateTime endTime)
        {
            var response = await Client.GetAsync("api/Admin/");

            if (!response.IsSuccessStatusCode)
                throw new Exception();

            var result = await response.Content.ReadAsStringAsync();

            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

            var transactionList = new List<Transaction>();

            foreach (var c in customers)
            {
                foreach (var a in c.Accounts)
                {

                    foreach (var t in a.Transactions)
                    {

                        transactionList.Add(t);
                    }
                }
            }
            var PrintList = new List<Transaction>();
            foreach (var t in transactionList)
            {
                if (t.ModifyDate.CompareTo(startTime) >= 0 && t.ModifyDate.CompareTo(endTime) <= 0)
                {
                    PrintList.Add(t);
                }
            }
            return View(PrintList);
        }
    }
}
