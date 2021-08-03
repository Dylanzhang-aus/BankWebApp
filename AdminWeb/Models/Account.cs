using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 */

namespace AdminWeb.Models
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
        public Account() { }
    }
}
