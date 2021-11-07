using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Storage.DataInterfaces;
using BlazorBoilerplate.Shared.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorBoilerplate.Infrastructure.Storage.DataModels
{
    public partial class DebitCardFraudTransaction : IAuditable, ISoftDelete
    {
        [Key]
        public long TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
        public string State { get; set; }
        public double Amount { get; set; } //100 - ATM the net dispute

        public bool Bucket0 { get; set; }
        public bool Bucket100 { get; set; }
        public bool Bucket50 { get; set; } //true
        public bool Bucket500 { get; set; }

        public double AmountRecovered { get; set; } //50
        public double AmountLoss { get; set; } //50
        public double ProvisionalCreditGiven { get; set; }
        public double ProvisionalCreditRecovered { get; set; }

        //public bool WasRecovered
        //{
        //    get
        //    {
        //        if (AmountRecovered > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //} //False = Loss

        public double AtmIntendedDeposit { get; set; }
        public double AtmIntendedWithdrawal { get; set; }
        public double ProcessedAmount { get; set; }

        public bool ProvisionalLetterSent { get; set; }
        public DateTime ProvisionalLetterSentOn { get; set; }
        public bool FinalLetterSent { get; set; }
        public DateTime FinalLetterSentOn { get; set; }

        public DecisionStatuses DecisionStatus { get; set; }
        public TransactionStatuses TransactionStatus { get; set; }

        public string CycleDate { get; set; }
        public long CoreTranId { get; set; }
        public long FraudClaimClaimId { get; set; }
    }
}

