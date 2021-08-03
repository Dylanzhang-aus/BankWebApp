using System;
using System.ComponentModel.DataAnnotations;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021.
 * @author Zalik Fakri - s3778065, RMIT 2021.
 * this class for user to select a time period.
 */

namespace AdminWeb.Models
{
    public class DatePicker
    {
        [Required]
        public int CustomerID { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime StartTime { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        public DateTime EndTime { get; set; }

        public DatePicker(int customerID, DateTime startTime, DateTime endTime)
        {
            CustomerID = customerID;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}

