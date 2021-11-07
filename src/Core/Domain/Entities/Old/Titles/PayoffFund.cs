using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PEXHub.Models
{
    public enum FundType
    {
        ElFunds,
        Check,
        Cash,
        ACH
    }

    public class PayoffFund
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount Format. E.g. 123.12")]
        public double FundAmount { get; set; }

        public FundType FundType { get; set; }
        [Display(Name = "Description")]
        [StringLength(100)]
        public string FundDescription { get; set; }

        [Display(Name = "Teller")]
        [StringLength(100)]
        public string TellerName { get; set; }
        [Display(Name = "Branch")]
        [StringLength(100)]
        public string BranchName { get; set; }

        [Display(Name = "Routing Number")]
        [StringLength(19)]
        public string RoutingNumber { get; set; }
        [Display(Name = "Account Number")]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        public byte[] CheckImage { get; set; }

        public int PayoffLoanID { get; set; }
        public PayoffLoan PayoffLoan { get; set; }
    }
}
