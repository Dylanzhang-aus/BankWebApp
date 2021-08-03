using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 */

namespace AdminWeb.Models
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3,
        ServiceCharge = 4,
        BillPay = 5
    }

    public class Transaction
    {
        public int TransactionID { get; set; }

        [Required, StringLength(1)]
        public TransactionType TransactionType { get; set; }

        [Required]
        [ForeignKey("AccountForeighKey")]
        public int AccountNumber { get; set; }

        [ForeignKey("DestAccountForeighKey")]
        public int? DestAccountNumber { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime ModifyDate { get; set; }

        [Required]
        public bool Is_New { get; set; }

        [Required]
        public bool Is_Debit { get; set; }

        //navigation properties
        public virtual Account Account { get; set; }

        public Transaction(){}
    }
}
