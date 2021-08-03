using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BankApi.Models.DataManager;
using BankApi.Models;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * this controller class is to kick of the action in the WebApi methods (DataManager)
 */

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly AdminManager _AdminRepo;
        
        public AdminController(AdminManager adminRepo)
        {
            _AdminRepo = adminRepo;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _AdminRepo.GetAll();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _AdminRepo.Get(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Customer customer)
        {
            _AdminRepo.Add(customer);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put([FromBody] Customer customer)
        {
            _AdminRepo.Update(customer.CustomerID, customer);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _AdminRepo.Delete(id);
        }
    }
}
