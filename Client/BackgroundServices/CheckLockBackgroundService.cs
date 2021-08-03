using System;
using System.Threading;
using System.Threading.Tasks;
using Assignment2_WDT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * This class checks to see if there are any accounts that have been locked
 */

namespace Assignment2_WDT.BackgroundServices
{
    public class CheckLockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public CheckLockBackgroundService(IServiceProvider services)
        {
            _services = services;        
        }


        //excute the checking every 1 minutes.
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            
            while (!cancellationToken.IsCancellationRequested)
            {
                await DoWork(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }


        private async Task DoWork(CancellationToken cancellationToken)
        {

            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            //loading data from database.
            var customers = await context.Customer.ToListAsync(cancellationToken);


            //Check to see if there are any locked users
            foreach (var c in customers)
            {
                if(c.Is_locked == true)
                {
                    //Automatic unlock
                    c.Is_locked = false;
                }
            }

            //saving the change.
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
