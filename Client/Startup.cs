using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Assignment2_WDT.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Assignment2_WDT.BackgroundServices;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021.
 * @author Zalik Fakri - s3778065, RMIT 2021.
 */

namespace Assignment2_WDT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ConnectionKey")); 
                options.UseLazyLoadingProxies();
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                // make unique cookie's name
                options.Cookie.Name = "CustomerWeb";
                options.Cookie.IsEssential = true;
            });

            // Add people background service to automatically run in the background along-side the web-server.
            services.AddHostedService<PayBillBackgroundService>();
            services.AddHostedService<CheckLockBackgroundService>();
            services.AddHostedService<EmailSendBackgroundService>();
            services.AddControllersWithViews();


            //for dispaly money.
            var cultureInfo = new CultureInfo("en-AU");
            cultureInfo.NumberFormat.CurrencySymbol = "$";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //this is for expection page
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession(); 

            app.UseRouting();
            app.UseAuthorization();          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
