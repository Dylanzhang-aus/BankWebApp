using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 */

namespace AdminWeb.Models
{
    public class BillPay
    {
        public int BillPayID { get; set; }

        [Required]
        [ForeignKey("AccountForeighKey")]
        public int AccountNumber { get; set; }

        [Required]
        [ForeignKey("PayeeForeighKey")]
        public int PayeeID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime ScheduleDate { get; set; }

        [Required]
        public string Period { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime ModifyDate { get; set; }

        [Required]
        public bool Is_blocked { get; set; }

        public string Status { get; set; }

        //navigation properties
        public virtual Account Account { get; set; }
        public virtual Payee Payee { get; set; }
        public BillPay() { }
    }
}
