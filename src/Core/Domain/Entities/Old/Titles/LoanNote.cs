using System;
using System.ComponentModel.DataAnnotations;

namespace PEXHub.Models
{
    public class LoanNote
    {
        public int ID { get; set; }

        public string Note { get; set; }

        [Display(Name = "Note Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; }

        [StringLength(100)]
        public string User { get; set; }

        public int PayoffLoanID { get; set; }
        public PayoffLoan PayoffLoan { get; set; }
    }
}
