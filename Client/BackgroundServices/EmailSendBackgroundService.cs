using System;
using System.Threading;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MailKit.Net.Smtp;
using System.Collections.Generic;
using Assignment2_WDT.Models;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class is for sending email to user when they account has new situation.
 */

namespace Assignment2_WDT.BackgroundServices
{
    public class EmailSendBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public EmailSendBackgroundService(IServiceProvider services)
        {
            _services = services;
        }



        //excute the checking every 3 minutes
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
            }
        }


       
        private async Task DoWork(CancellationToken cancellationToken)
        {

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            //loading data from database.
            var customers = await context.Customer.ToListAsync(cancellationToken);

            //looping every user
            foreach (var c in customers)
            {

                //Create a temporary data storage container
                Dictionary<int, List<Transaction>> MessageBox = new Dictionary<int, List<Transaction>>();
                Dictionary<int, decimal> BlanceChange = new Dictionary<int, decimal>();
                Dictionary<int, decimal> CurrentBlance = new Dictionary<int, decimal>();


                //Check if the user is new and, if so, send the first report
                if (c.Is_NewCustomer == true)
                {
                    foreach(var a in c.Accounts)
                    {
                        List<Transaction> newtransactionBox = new List<Transaction>();
                        foreach(var t in a.Transactions)
                        {
                            if(t.Is_New == true)
                            {
                                t.Is_New = false;
                                newtransactionBox.Add(t);
                            }
                        }
                        if (newtransactionBox.Count > 0)
                        {
                            MessageBox.Add(a.AccountNumber, newtransactionBox);
                            CurrentBlance.Add(a.AccountNumber, a.Balance);
                        }
                    }

                    //set user status to old customer.
                    c.Is_NewCustomer = false;

                    //if there is transaction in customer accounts. we create a email.
                    if (MessageBox.Count > 0)
                    {
                        var message = new MimeMessage();
                        var builder = new BodyBuilder();
                        message.From.Add(new MailboxAddress("NWA Bank", "zhanghanyuan0830@gmail.com"));
                        message.To.Add(new MailboxAddress("s3757573", c.EmailAdress));
                        message.Subject = "First Transaction Report";
                        message.Date = DateTime.UtcNow.ToLocalTime();

                        //email body create.
                        builder.HtmlBody += @"<div style='text-align:center'>";
                        builder.HtmlBody += @$"<h4 style='color:yellow;'>NWA Bank Classification: Trusted</h4>";
                        builder.HtmlBody += @"</div>";
                        builder.HtmlBody += @$"<h4>Dear {c.CustomerName} ,</h4>";
                        builder.HtmlBody += @"<p>This is your first Transaction report, which show your all Transaction history in the pass.</br>";
                        foreach (var m in MessageBox)
                        {
                            builder.HtmlBody += @"<table style='background-color:powderblue;'>";
                            builder.HtmlBody += @"<p style='color:red;'>--------------------------------------------------------------------------------------></br>";
                            builder.HtmlBody += @$"<p>Acount Number  :   {m.Key} __ Currect Balance: {CurrentBlance.GetValueOrDefault(m.Key):0.00}$</br>";
                            builder.HtmlBody += @"<tr>";
                            builder.HtmlBody += @"<th>TransactionID  </th>";
                            builder.HtmlBody += @"<th>Amount </th>";
                            builder.HtmlBody += @"<th>Transaction Type </th>";
                            builder.HtmlBody += @"<th>Modify Date   </th>";
                            builder.HtmlBody += @"<th>Comment </th>";
                            builder.HtmlBody += @"</tr>";
                            foreach (var t in m.Value)
                            {
                                builder.HtmlBody += @"<tr>";
                                builder.HtmlBody += @$"<td>{t.TransactionID}</td>";
                                builder.HtmlBody += @$"<td>{t.Amount}$</td>";
                                builder.HtmlBody += @$"<td>{t.TransactionType}</td>";
                                builder.HtmlBody += @$"<td>{t.ModifyDate}</td>";
                                builder.HtmlBody += @$"<td>{t.Comment}</td>";
                                builder.HtmlBody += @"</tr>";
                            }
                            builder.HtmlBody += @"</table>";
                        }
                        builder.HtmlBody += @"<p style='color:red;'>--------------------------------------------------------------------------------------></br>";
                        builder.HtmlBody += @"<p>Kind Regards,</br>";
                        builder.HtmlBody += @"<p>NWA Bank Team</br>";
                        builder.HtmlBody += @"<a href='https://localhost:5001/'>Login To Check Detial</a>";
                        message.Body = builder.ToMessageBody();

                        //send this email to the customer.
                        using var client = new SmtpClient();
                        client.Connect("smtp.gmail.com", 587, false, cancellationToken);
                        client.Authenticate("zhanghanyuan0830@gmail.com", "Zhanghanyuan007", cancellationToken);
                        client.Send(message);
                        await context.SaveChangesAsync(cancellationToken);
                        client.Disconnect(true, cancellationToken);

                    }
                }

                //if the customer is old customer. we just sent the change of the transaction in email.
                else
                {                  
                    foreach (var a in c.Accounts)
                    {
                        decimal totalAmount = 0;
                        List<Transaction> newtransactionBox = new List<Transaction>();
                        foreach (var t in a.Transactions)
                        {
                            if (t.Is_New == true)
                            {
                                t.Is_New = false;
                                if (t.Is_Debit)
                                {
                                    totalAmount -= t.Amount;
                                }
                                else
                                {
                                    totalAmount += t.Amount;
                                }                                
                                newtransactionBox.Add(t);
                            }
                        }
                        if (newtransactionBox.Count > 0)
                        {
                            MessageBox.Add(a.AccountNumber, newtransactionBox);
                            BlanceChange.Add(a.AccountNumber, totalAmount);
                            CurrentBlance.Add(a.AccountNumber, a.Balance);
                        }
                    }
                    if (MessageBox.Count > 0)
                    {                       
                        var message = new MimeMessage();
                        var builder = new BodyBuilder();
                        message.From.Add(new MailboxAddress("NWA Bank", "zhanghanyuan0830@gmail.com"));
                        message.To.Add(new MailboxAddress("s3757573", c.EmailAdress));
                        message.Subject = "New Transaction";
                        message.Date = DateTime.UtcNow.ToLocalTime();
                        builder.HtmlBody += @"<div style='text-align:center'>";
                        builder.HtmlBody += @$"<h4 style='color:yellow;'>NWA Bank Classification: Trusted</h4>";
                        builder.HtmlBody += @"</div>";
                        builder.HtmlBody += @$"<h4>Dear {c.CustomerName} ,</h4>";
                        builder.HtmlBody += @"<p>This is the Transaction report. You have new transaction in you accounts.
                                                 Please verify that your account is secure.</br>";
                        foreach (var m in MessageBox)
                        {
                            builder.HtmlBody += @"<table style='background-color:powderblue;'>";
                            builder.HtmlBody += @"<p style='color:red;'>--------------------------------------------------------------------------------------></br>";
                            builder.HtmlBody += @$"<p>Acount Number  :   {m.Key} ___ Current Balance : {CurrentBlance.GetValueOrDefault(m.Key):0.00}$</br>"; 
                            builder.HtmlBody += @"<tr>";
                            builder.HtmlBody += @"<th>TransactionID  </th>";
                            builder.HtmlBody += @"<th>Amount </th>";
                            builder.HtmlBody += @"<th>Transaction Type </th>";
                            builder.HtmlBody += @"<th>Modify Date </th>";
                            builder.HtmlBody += @"<th>Comment </th>";
                            builder.HtmlBody += @"</tr>";
                            foreach (var t in m.Value)
                            {
                                builder.HtmlBody += @"<tr>";
                                builder.HtmlBody += @$"<td>{t.TransactionID}</td>";
                                builder.HtmlBody += @$"<td>{t.Amount}$</td>";   
                                builder.HtmlBody += @$"<td>{t.TransactionType}</td>";
                                builder.HtmlBody += @$"<td>{t.ModifyDate}</td>";
                                builder.HtmlBody += @$"<td>{t.Comment}</td>";
                                builder.HtmlBody += @"</tr>";
                            }
                            builder.HtmlBody += @$"<p>Totale Balance Change :  {BlanceChange.GetValueOrDefault(m.Key):0.00}$</br>";
                            builder.HtmlBody += @"</table>";
                        }
                        builder.HtmlBody += @"<p style='color:red;'>--------------------------------------------------------------------------------------></br>";
                        builder.HtmlBody += @"<p>Kind Regards,</br>";
                        builder.HtmlBody += @"<p>NWA Bank Team</br>";
                        builder.HtmlBody += @"<a href='https://localhost:5001/'>Login To Check Detial</a>";                       
                        message.Body = builder.ToMessageBody();

                        using var client = new SmtpClient();
                        client.Connect("smtp.gmail.com", 587, false, cancellationToken);
                        client.Authenticate("zhanghanyuan0830@gmail.com", "Zhanghanyuan007", cancellationToken);
                        client.Send(message);
                        await context.SaveChangesAsync(cancellationToken);
                        client.Disconnect(true, cancellationToken);
                    }
                }
            }          
        }       
    }
}
