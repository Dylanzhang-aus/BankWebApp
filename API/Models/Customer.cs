using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

/*
 * @author Hanyuan Zhang - s3757573, RMIT 2021
 * @author Zalik Fakri - s3778065, RMIT 2021
 */

namespace BankApi.Models
{
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string CustomerName { get; set; }

        [StringLength(11)]
        public string TFN { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string City { get; set; }

        [StringLength(20)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostCode { get; set; }

        [Required, StringLength(15)]
        public string Phone { get; set; }

        [Required]
        public bool Is_locked { get; set; }

        [Required]
        public bool Is_NewCustomer { get; set; }

        [Required, StringLength(50)]
        public string EmailAdress { get; set; }


        //navigation properties
        public virtual List<Account> Accounts { get; set; }

        public Customer(){ }
    }
}
