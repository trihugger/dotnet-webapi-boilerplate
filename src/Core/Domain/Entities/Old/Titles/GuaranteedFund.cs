using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PEXHub.Models
{
    public class GuaranteedFund
    {
        public int ID { get; set; }

        [Display(Name = "Entity Name")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Display(Name = "Official Funds")]
        public bool GFunds { get; set; }
        [Display(Name = "Authorized Dealer")]
        public bool Dealer { get; set; }

        [Display(Name = "Routing Number")]
        [StringLength(19)]
        public string RoutingNumber { get; set; }
        [Display(Name = "Account Number")]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }
}
