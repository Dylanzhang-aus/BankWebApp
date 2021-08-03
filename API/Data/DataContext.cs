using Microsoft.EntityFrameworkCore;
using BankApi.Models;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 * the create schema using fluent API
 */

namespace BankApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Login> Login { get; set; }
        public DbSet<BillPay> BillPay { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Payee> Payee { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
 
            builder.Entity<BillPay>().HasOne(x => x.Account).WithMany(x => x.BillPays).HasForeignKey(x => x.AccountNumber);
            builder.Entity<BillPay>().HasOne(x => x.Payee).WithMany(x => x.BillPays).HasForeignKey(x => x.PayeeID);
            builder.Entity<Transaction>().HasOne(x => x.Account).WithMany(x => x.Transactions).HasForeignKey(x => x.AccountNumber);

            builder.Entity<Customer>().HasCheckConstraint("CH_Customer_PostCode", "len(PostCode) = 4");
            builder.Entity<Login>().HasCheckConstraint("CH_Login_CustomerID", "len(CustomerID) = 4");
            builder.Entity<Login>().HasCheckConstraint("CH_Login_LoginID", "len(LoginID) = 8");
            builder.Entity<Account>().HasCheckConstraint("CH_Account_Balance", "Balance >= 0");
            builder.Entity<Account>().HasCheckConstraint("CH_Account_AccountNumber", "len(AccountNumber) = 4");
            builder.Entity<Transaction>().HasCheckConstraint("CH_Transaction_Amount", "Amount > 0");
            builder.Entity<BillPay>().HasCheckConstraint("CH_BillPay_Amount", "Amount > 0");
            builder.Entity<Payee>().HasCheckConstraint("CH_Payee_PostCode", "len(PostCode) = 4");

        }
    }
}
