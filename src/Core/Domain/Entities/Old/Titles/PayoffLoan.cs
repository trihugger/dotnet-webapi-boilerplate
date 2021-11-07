using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PEXHub.Models
{
    public enum ReleaseType
    {
        Immediate,
        [Display(Name = "10 Days")]
        TenDays
    }

    public enum LoanReleaseStatus
    {
        Pending, // All loans that come in - Step1
        Recovered, // Loans waiting on MAG for Release - Step 2 from 1
        Approved, // Approved by Loan Ops to Process - Step 2
        Manual, // Loan that must be processed Manually by a person. E.g. Paper Titles - Step 2
        Processed, // Processed by Robot or Human - Step 3

        // Chaged Off loans go straight to Complete Status
        Completed // Final Step - Step 4
    }

    public class PayoffLoan
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        [StringLength(15)]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Paid Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PaidDate { get; set; }
        [StringLength(100)]
        public string Product { get; set; }
        [Required]
        [StringLength(100)]
        public string Borrower { get; set; }
        [Display(Name = "Co-Borrower")]
        [StringLength(100)]
        public string CoBorrower { get; set; }
        [Required]
        [Display(Name = "VIN #")]
        [StringLength(17)]
        public string VIN { get; set; }

        [Required]
        [Display(Name = "Payoff Amount")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount Format. E.g. 123.12")]
        public double PayOffAmount { get; set; }

        [Display(Name = "Charged Off")]
        public bool ChargeOff { get; set; }
        public bool Recovered { get; set; }
        public bool Perfected { get; set; }
        public bool Paper { get; set; }

        [Required]
        [Display(Name = "Release Type")]
        public ReleaseType ReleaseType { get; set; }
        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [StringLength(100)]
        public string Processor { get; set; }
        [StringLength(100)]
        public string Approver { get; set; }
        [StringLength(100)]
        public string Reviewer { get; set; }

        [Display(Name = "Request Status")]
        public LoanReleaseStatus ReleaseStatus { get; set; }

        public ICollection<PayoffFund> PayoffFunds { get; set; }
        public ICollection<LoanNote> LoanNotes { get; set; }
    }
}
