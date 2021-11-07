using System;

public class DealerIncentiveSheetEntry
{
    public DateTime FundDate { get; set; }
    public DateTime ClosedDate { get; set; }

    public DateTime ProcessedDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime CompletedDate { get; set; }

    public object LoanId { get; set; }
    public bool ClosedEarly 
    { 
        get
        {
            return ClosedDate - Fundate <= 30;
        }
    }
    // add app# to the loan model

    public double LoanAmount { get; set; }
    public double LoanCost { get; set; } // added after the sheet is processed (rounded up)
}
