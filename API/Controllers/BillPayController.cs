using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    public class BillPayController : Controller
    {
        private readonly BillPayManager _BillPayRepo;

        public BillPayController(BillPayManager billPayRepo)
        {
            _BillPayRepo = billPayRepo;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<BillPay> Get()
        {
            return _BillPayRepo.GetAll();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public BillPay Get(int id)
        {
            return _BillPayRepo.Get(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] BillPay billPay)
        {
            _BillPayRepo.Add(billPay);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put([FromBody] BillPay billPay)
        {
            _BillPayRepo.Update(billPay.BillPayID, billPay);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public long Delete(int id)
        {
            return _BillPayRepo.Delete(id);
        }
    }
}
