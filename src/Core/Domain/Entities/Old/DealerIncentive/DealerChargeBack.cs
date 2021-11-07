using System;

public class DealerChargeBack
{
    public object LoanId { get; set; }
    public object FinanceManagerId { get; set; }

    public DateTime ProcessedDate { get; set; }
    public bool Complete 
    { 
        get 
        {
            return ProcessedDate >= DateTime.Now.AddYears(-1000);
        }
    }

    public double IncentiveAmount { get; set; }
}
