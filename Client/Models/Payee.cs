using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021.
 * @author Zalik Fakri - s3778065, RMIT 2021.
 */

namespace Assignment2_WDT.Models
{
    public class Payee
    {
        public int PayeeID { get; set; }

        [Required, StringLength(50)]
        public string PayeeName { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(20)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostCode { get; set; }
        
        [Required, StringLength(15)]
        [RegularExpression(@"^(\+)?(61)?\(0)\d{0,9}$")]
        public string Phone { get; set; }


        //navigation properties
        public virtual List<BillPay> BillPays { get; set; }

        public Payee(){}
    }
}
