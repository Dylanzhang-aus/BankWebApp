using System;
using System.Threading;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Assignment2_WDT.Models;
using System.Linq;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class is for pay bill.
 */

namespace Assignment2_WDT.BackgroundServices
{
    public class PayBillBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PayBillBackgroundService> _logger;

        public PayBillBackgroundService(IServiceProvider services, ILogger<PayBillBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Paybill Background Service is running.");

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);

                _logger.LogInformation("Paybill Background Service is waiting a minute.");

                await Task.Delay(TimeSpan.FromSeconds(65), cancellationToken);
            }
        }


        private async Task DoWork(CancellationToken cancellationToken)
        {

            _logger.LogInformation("Paybill Background Service is working.");

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            //loading data from database.
            var paybills = await context.BillPay.ToListAsync(cancellationToken);
            var accounts = await context.Account.ToListAsync(cancellationToken);

            // check the PayBill table for rows that need to be process.
            foreach (var paybill in paybills)
            {
                if (paybill.Is_blocked == false)
                {
                    // compare the date.now with date from table
                    var date1 = DateTime.Now.ToString("yyyy-MM-dd");
                    var date2 = paybill.ScheduleDate.ToString("yyyy-MM-dd");

                    if (date1.Equals(date2) || date2.CompareTo(date1) <= 0)
                    {
                        foreach (var account in accounts)
                        {
                            if (account.AccountNumber == paybill.AccountNumber)
                            {
                                if (account.AccountType.Equals(AccountType.Checking))
                                {
                                    decimal minBalance = 200;
                                    if (minBalance > (account.Balance - paybill.Amount))
                                    {
                                        var status = context.BillPay.Where(a => a.AccountNumber == paybill.AccountNumber && a.BillPayID == paybill.BillPayID).FirstOrDefault();
                                        if (status != null)
                                        {
                                            status.Status = "Payment was Failed";
                                            context.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        account.Balance -= paybill.Amount;
                                        account.Transactions.Add(
                                            new Transaction
                                            {
                                                TransactionType = TransactionType.BillPay,
                                                AccountNumber = paybill.AccountNumber,
                                                Amount = paybill.Amount,
                                                Comment = "Payment was successful",
                                                ModifyDate = DateTime.UtcNow.ToLocalTime(),
                                                Is_New = true,
                                                Is_Debit = true
                                            });

                                        // delete row in BillPay table once the bill payment success.
                                        var deletePayBillRow = context.BillPay.Where(a => a.AccountNumber == paybill.AccountNumber && a.BillPayID == paybill.BillPayID).FirstOrDefault();
                                        if (deletePayBillRow != null)
                                        {
                                            context.BillPay.Remove(deletePayBillRow);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    decimal minBalance = 0;
                                    if (minBalance > (account.Balance - paybill.Amount))
                                    {
                                        var status = context.BillPay.Where(a => a.AccountNumber == paybill.AccountNumber && a.BillPayID == paybill.BillPayID).FirstOrDefault();
                                        if (status != null)
                                        {
                                            status.Status = "Payment was Failed";
                                            context.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        account.Balance -= paybill.Amount;
                                        account.Transactions.Add(
                                            new Transaction
                                            {
                                                TransactionType = TransactionType.BillPay,
                                                AccountNumber = paybill.AccountNumber,
                                                Amount = paybill.Amount,
                                                Comment = "Payment was successfull",
                                                ModifyDate = DateTime.UtcNow.ToLocalTime(),
                                                Is_New = true,
                                                Is_Debit = true
                                            });

                                        // delete row in BillPay table once the bill payment success.
                                        var deletePayBillRow = context.BillPay.Where(a => a.AccountNumber == paybill.AccountNumber && a.BillPayID == paybill.BillPayID).FirstOrDefault();
                                        if (deletePayBillRow != null)
                                        {
                                            context.BillPay.Remove(deletePayBillRow);
                                            context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Paybill Background Service is work complete.");
        }
    }
}

