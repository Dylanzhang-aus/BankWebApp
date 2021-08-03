using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using BankApi.Models;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * this class for seed the data into the database
 */

namespace BankApi.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();

            if (context.Customer.Any())
            {
                return;
            }
            else
            {
                SeedCustomer(context);
                SeedLogin(context);
                SeedAccount(context);
                SeedTransaction(context);
                SeedPayee(context);
            }
        }

        public static void SeedCustomer(DataContext context)
        {
            context.Customer.AddRange(
                new Customer
                {
                    CustomerID = 2100,
                    CustomerName = "Matthew Bolger",
                    Address = "123 Fake Street",
                    City = "Melbourne",
                    PostCode = "3000",
                    Phone = "61+ 0123416722",
                    Is_locked = false,
                    Is_NewCustomer = true,
                    EmailAdress = "s3757573@student.rmit.edu.au"
                },
                 new Customer
                 {
                     CustomerID = 2200,
                     CustomerName = "Rodney Cocker",
                     Address = "456 Real Road",
                     City = "Melbourne",
                     PostCode = "3005",
                     Phone = "61+ 0123416733",
                     Is_locked = false,
                     Is_NewCustomer = true,
                     EmailAdress = "dfhasndf@gmail.com"
                 },
                 new Customer
                 {
                     CustomerID = 2300,
                     CustomerName = "Shekhar Kalra",
                     Phone = "61+ 0123416744",
                     Is_locked = false,
                     Is_NewCustomer = true,
                     EmailAdress = "babababa@gmail.com"
                 });
            context.SaveChanges();
        }

        public static void SeedLogin(DataContext context)
        {
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            context.Login.AddRange(
                new Login
                {
                    LoginID = "12345678",
                    CustomerID = 2100,
                    PasswordHash = "YBNbEL4Lk8yMEWxiKkGBeoILHTU7WZ9n8jJSy8TNx0DAzNEFVsIVNRktiQV+I8d2",
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
                },
                 new Login
                 {
                     LoginID = "38074569",
                     CustomerID = 2200,
                     PasswordHash = "EehwB3qMkWImf/fQPlhcka6pBMZBLlPWyiDW6NLkAh4ZFu2KNDQKONxElNsg7V04",
                     ModifyDate = DateTime.ParseExact("09/06/2020 01:00:00 PM", format, null)
                 },
                 new Login
                 {
                     LoginID = "17963428",
                     CustomerID = 2300,
                     PasswordHash = "LuiVJWbY4A3y1SilhMU5P00K54cGEvClx5Y+xWHq7VpyIUe5fe7m+WeI0iwid7GE",
                     ModifyDate = DateTime.ParseExact("09/06/2020 02:00:00 PM", format, null)
                 });
            context.SaveChanges();
        }


        public static void SeedAccount(DataContext context)
        {
            const string format = "dd/MM/yyyy hh:mm:ss tt";
            Account s1 = new Account
            {
                AccountNumber = 4100,
                AccountType = AccountType.Saving,
                CustomerID = 2100,
                Balance = 100,
                ModifyDate = DateTime.ParseExact("09/06/2020 09:00:00 AM", format, null)
            };

            Account c1 = new Account
            {
                AccountNumber = 4101,
                AccountType = AccountType.Checking,
                CustomerID = 2100,
                Balance = 500,
                ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null)
            };

            Account s2 = new Account
            {
                AccountNumber = 4200,
                AccountType = AccountType.Saving,
                CustomerID = 2200,
                Balance = 500.95m,
                ModifyDate = DateTime.ParseExact("09/06/2020 01:00:00 PM", format, null)
            };
            Account c2 = new Account
            {
                AccountNumber = 4300,
                AccountType = AccountType.Checking,
                CustomerID = 2300,
                Balance = 1250.50m,
                ModifyDate = DateTime.ParseExact("09/06/2020 02:00:00 PM", format, null)
            };
            context.Add(s1);
            context.Add(s2);
            context.Add(c1);
            context.Add(c2);
            context.SaveChanges();
        }

        public static void SeedTransaction(DataContext context)
        {
            const string initialDeposit = "Initial Deposit";
            const string format = "dd/MM/yyyy hh:mm:ss tt";

            context.Transaction.AddRange(
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4101,
                    Amount = 500,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("08/06/2020 08:00:00 PM", format, null),
                    Is_New = true,
                    Is_Debit = false
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4100,
                    Amount = 100,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 09:00:00 AM", format, null),
                    Is_New = true,
                    Is_Debit = false
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4200,
                    Amount = 500.95m,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 01:00:00 PM", format, null),
                    Is_New = true,
                    Is_Debit = false
                },
                new Transaction
                {
                    TransactionType = TransactionType.Deposit,
                    AccountNumber = 4300,
                    Amount = 1250.50m,
                    Comment = initialDeposit,
                    ModifyDate = DateTime.ParseExact("09/06/2020 02:00:00 PM", format, null),
                    Is_New = true,
                    Is_Debit = false
                });
            context.SaveChanges();
        }

        public static void SeedPayee(DataContext context)
        {
            context.Payee.AddRange(
                new Payee
                {
                    PayeeName = "Telstra",
                    Address = "567 Exhibition Street",
                    City = "Melbourne",
                    PostCode = "3000",
                    Phone = "61+ 0123234235",
                    State = "VIC"
                },
                 new Payee
                 {
                     PayeeName = "RMIT",
                     Address = "124 La Trobe Street",
                     City = "Melbourne",
                     PostCode = "3000",
                     Phone = "61+ 0124765893",
                     State = "VIC"
                 });
            context.SaveChanges();
        }
    }
}
