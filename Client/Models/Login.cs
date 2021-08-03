using System;
using System.ComponentModel.DataAnnotations;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021.
 * @author Zalik Fakri - s3778065, RMIT 2021.
 */

namespace Assignment2_WDT.Models
{
    public record Login
    {
        [Required]
        public string LoginID { get; init; }

        [Required]
        public int CustomerID { get; init; }

        [Required, StringLength(64)]
        public string PasswordHash { get; init; } 

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime ModifyDate { get; set; }

        //navigation properties
        public virtual Customer Customer { get; set; }

        public Login(){}
    }
}
