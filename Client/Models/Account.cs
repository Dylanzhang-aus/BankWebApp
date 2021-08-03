using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021.
 * @author Zalik Fakri - s3778065, RMIT 2021.
 */

namespace Assignment2_WDT.Models
{
    public enum AccountType
    {
        Checking = 1,
        Saving = 2
    }

    public class Account
    {
        [Key, Display(Name = "Account Number")]
        [ForeignKey("AccountNumber")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountNumber { get; set; }

        [Required, StringLength(1)]
        [Display(Name = "Type")]
        public AccountType AccountType { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime ModifyDate { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }


        //navigation properties
        public virtual Customer Customer { get; set; }
        public virtual List<BillPay> BillPays { get; set; }
        public virtual List<Transaction> Transactions { get; set; }

        public Account(){}


        public void Deposit(decimal amount)
        {
            
            Balance += amount;
            ModifyDate = DateTime.UtcNow.ToLocalTime();
        }


        public bool Withdraw(decimal amount, decimal minBalance, decimal chargeFee)
        {

            _ = new decimal(0);

            //to count the number of transaction.
            //if debit transaction more then 4, the chargeFee will be added.
            List<Transaction> tempTransactionBox = new List<Transaction>();
            foreach (var t in Transactions)
            {
                if (!t.TransactionType.Equals(TransactionType.Deposit))
                {
                    tempTransactionBox.Add(t);
                }
            }

            decimal totalAmount;
            if (tempTransactionBox.Count > 3)
            {
                totalAmount = amount + chargeFee;
                if(Balance - totalAmount >= minBalance)
                {
                    Balance -= totalAmount;
                    ModifyDate = DateTime.UtcNow.ToLocalTime();
                    Transactions.Add(
                            new Transaction
                            {
                                TransactionType = TransactionType.ServiceCharge,
                                AccountNumber = AccountNumber,
                                Amount = chargeFee,
                                Comment = "Charge Fee",
                                ModifyDate = DateTime.UtcNow.ToLocalTime(),
                                Is_New = true,
                                Is_Debit = true
                            });
                    return true;
                }
                else
                {
                    return false;
                }   
            }
            else
            {
                totalAmount = amount;
                if (Balance - totalAmount >= minBalance)
                {
                    Balance -= totalAmount;
                    ModifyDate = DateTime.UtcNow.ToLocalTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }      
        }
    }
}
