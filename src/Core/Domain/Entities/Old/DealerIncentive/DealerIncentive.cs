using System;

public class DealerIncentive
{
    public object Dealer { get; set; } // or branch
    public List<FinanceManager> FinanceManagers { get; set; } // active only

    public bool Active { get; set; }
    public double IncentivePercentage { get; set; }
    public double MinimalToQualify { get; set; }
    public double Tier1Minimal { get; set; }
    public double Tier1Incentive { get; set; }
    public double Tier2Minimal { get; set; }
    public double Tier2Incentive { get; set; }
    public double Tier3Minimal { get; set; }
    public double Tier3Incentive { get; set; }
    public double Tier4Minimal { get; set; }
    public double Tier4Incentive { get; set; }
    public double Tier5Minimal { get; set; }
    public double Tier5Incentive { get; set; }
    public double Tier6Minimal { get; set; }
    public double Tier6Incentive { get; set; }
}
