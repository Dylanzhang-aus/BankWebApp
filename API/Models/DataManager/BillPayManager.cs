using System.Collections.Generic;
using System.Linq;
using BankApi.Models.Repository;
using BankApi.Data;
using Microsoft.EntityFrameworkCore;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * this class consist of methods in order to get the data of billpay from the database then put into WebAPI (JSON data) 
 */

namespace BankApi.Models.DataManager
{
    public class BillPayManager : DataInterface<BillPay, int>
    {
        private readonly DataContext _dataContext;

        public BillPayManager(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        //get one specifical billpay.
        public BillPay Get(int id)
        {
            //return _dataContext.BillPay.Find(id);
            var billpay = _dataContext.BillPay.Find(id);

            _dataContext.BillPay.Where(a => a.BillPayID == id).Load();
            var billpays = _dataContext.BillPay.ToList();
            foreach (var a in billpays)
            {
                _dataContext.BillPay.Where(b => b.AccountNumber == a.AccountNumber).Load();
            }
            return billpay;
        }


        //get all billpays.
        public IEnumerable<BillPay> GetAll()
        {
            var billpays = _dataContext.BillPay.ToList();

            foreach (var a in billpays)
            {
                _dataContext.BillPay.Where(b => b.AccountNumber == a.AccountNumber).Load();
            }
            return billpays;
        }


        public int Add(BillPay billPay)
        {
            _dataContext.BillPay.Add(billPay);
            _dataContext.SaveChanges();

            return billPay.BillPayID;
        }


        public int Delete(int id)
        {
            _dataContext.BillPay.Remove(_dataContext.BillPay.Find(id));
            _dataContext.SaveChanges();

            return id;
        }


        public int Update(int id, BillPay billPay)
        {
            _dataContext.Update(billPay);
            _dataContext.SaveChanges();

            return id;
        }
    }
}
