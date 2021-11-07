using System;

public class DealerIncentiveSheet
{
    public string InvoiceNumber { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }    

    public DateTime ProcessedDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime CompletedDate { get; set; }

    public object Dealer { get; set; } // could be a branch if the incentive is broken down by branch

    public double TotalFinanced { get; set; }
    public double TotalChargedBack { get; set; }
    public double TotalNetFinanced { get; set; }
    public double Incentive { get; set; }

    public int NumberOfLoans { get; set; } // get from SheetEntry
  
    public int FinaceManagers { get; set; } // Get from Dealer Incentive
    public double FinanceManagerIncentive { get; set; } // Get from Dealer Incentive
    public List<FileManagers> FileManagers { get; set; } // Get the list of FileManagers used on this sheet.

    public List<DealerIncentiveSheetEntry> IncentiveEntries { get; set; }
}
