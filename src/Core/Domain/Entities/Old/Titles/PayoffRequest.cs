using System;
using System.ComponentModel.DataAnnotations;

namespace PEXHub.Models
{
    public class PayoffRequest
    {
        public int ID { get; set; }

        [Display(Name ="Request Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime PayoffDate { get; set; }

        [Display(Name = "Loan Number")]
        [Required]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        public string Supervisor { get; set; }

        [Display(Name = "Withdrawn from FCFCU Account:")]
        [StringLength(15)]
        public string DepositAccount { get; set; }

        [Display(Name = "Requestor")]
        [StringLength(100)]
        public string User { get; set; }

        [Display(Name = "Mail To Other Party")]
        public bool Dealer { get; set; }

        [IsDealerRequired(ErrorMessage ="Please type in the Name for the Dealer or Other Party.")]
        [StringLength(100)]
        public string Name { get; set; }
        [IsDealerRequired(ErrorMessage = "Please type in the Street information.")]
        [Display(Name = "Street Line 1")]
        [StringLength(50)]
        public string Street1 { get; set; }
        [Display(Name = "Street Line 2")]
        [StringLength(50)]
        public string Street2 { get; set; }
        [IsDealerRequired(ErrorMessage = "Please type in the City Name.")]
        [StringLength(25)]
        public string City { get; set; }
        [IsDealerRequired(ErrorMessage = "Please type in the State.")]
        [RegularExpression(@"^[A-Z]{2}$")]
        [StringLength(2)]
        public string State { get; set; }
        [IsDealerRequired(ErrorMessage = "Please type in a Zip Code.")]
        [StringLength(10)]
        public string Zip { get; set; }
    }

    // Customer Validation (server side) for fields that are required if Dealer is Checked.
    public class IsDealerRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PayoffRequest dealer = (PayoffRequest)validationContext.ObjectInstance;

            if (dealer.Dealer == true && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
