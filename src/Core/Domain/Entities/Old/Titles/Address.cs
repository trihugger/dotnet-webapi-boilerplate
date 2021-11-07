using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PEXHub.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please type in the Name for the Dealer or Other Party.")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please type in the Street information.")]
        [Display(Name = "Street Line 1")]
        [StringLength(50)]
        public string Street1 { get; set; }
        [Display(Name = "Street Line 2")]
        [StringLength(50)]
        public string Street2 { get; set; }
        [Required(ErrorMessage = "Please type in the City Name.")]
        [StringLength(25)]
        public string City { get; set; }
        [Required(ErrorMessage = "Please type in the State.")]
        [RegularExpression(@"^[A-Z]{2}$")]
        [StringLength(2)]
        public string State { get; set; }
        [Required(ErrorMessage = "Please type in a Zip Code.")]
        [StringLength(10)]
        public string Zip { get; set; }

        public int GuaranteedID { get; set; }
        public GuaranteedFund GuaranteedFund { get; set; }
    }
}
