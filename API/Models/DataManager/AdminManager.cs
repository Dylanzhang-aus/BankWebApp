using System.Collections.Generic;
using System.Linq;
using BankApi.Models.Repository;
using BankApi.Data;
using Microsoft.EntityFrameworkCore;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * this class consist of methods in order to get the data of customer from the database then put into WebAPI (JSON data) 
 */

namespace BankApi.Models.DataManager
{
    public class AdminManager : DataInterface<Customer, int>
    {
        private readonly DataContext _dataContext;

        public AdminManager(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        //get one specifical customer from database.
        public Customer Get(int id)
        {
            //get the data from database by using id.
            var customer = _dataContext.Customer.Find(id);


            //using eager to load all information belong to this customer.account -> transaction -> Billpay.
            _dataContext.Account.Where(a => a.CustomerID == id).Load();
            var accounts = _dataContext.Account.Include(x => x.BillPays).Include(x => x.Transactions).ToList();
            foreach (var a in accounts)
            {
                _dataContext.Transaction.Where(t => t.AccountNumber == a.AccountNumber).Load();
                _dataContext.BillPay.Where(b => b.AccountNumber == a.AccountNumber).Load();
            }
            return customer;
        }


        //get all customers in database.
        public IEnumerable<Customer> GetAll()
        {
            var customers = _dataContext.Customer.Include(x => x.Accounts).ToList();
           
            foreach (var c in customers)
            {
               _dataContext.Account.Where(a => a.CustomerID == c.CustomerID).Load();
                
                foreach (var a in c.Accounts)
                {
                    _dataContext.Transaction.Where(t => t.AccountNumber == a.AccountNumber).Load();
                    _dataContext.BillPay.Where(b => b.AccountNumber == a.AccountNumber).Load();

                }
            }
            return customers;
        }


        public int Add(Customer customer)
        {
            _dataContext.Customer.Add(customer);
            _dataContext.SaveChanges();

            return customer.CustomerID;
        }


        public int Delete(int id)
        {
            _dataContext.Customer.Remove(_dataContext.Customer.Find(id));
            _dataContext.SaveChanges();

            return id;
        }


        public int Update(int id, Customer customer)
        {
            _dataContext.Update(customer);
            _dataContext.SaveChanges();
            return id;
        }
    }
}
