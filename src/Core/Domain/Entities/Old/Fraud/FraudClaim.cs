using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Storage.DataInterfaces;
using BlazorBoilerplate.Shared.Dto;
using FluentValidation.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BlazorBoilerplate.Infrastructure.Storage.DataModels
{
    [Permissions(Actions.Delete)]
    public partial class FraudClaim : IAuditable, ISoftDelete
    {
        [Key]
        public long ClaimId { get; set; }

        public string ClaimNumber { get; set; }
        public string MemberName { get; set; }

        public string PersonNumber { get; set; }

        public long PhoneNumber { get; set; }
        public string Email { get; set; }

        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public CardStatuses CardStatus { get; set; }

        public ATMDisputeTypes ATMDisputeType { get; set; }
        public FraudClaimTypes ClaimType { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        public string BranchName { get; set; }

        public double TotalTransactionsCleared { get; set; }
        public int TotalNumberOfPendingTransaction { get; set; }
        public double TotalTransactionsPending { get; set; }
        public double TotalClaimDispute { get; set; } //ATM NET DISPUTE
        public double AtmIntendedDeposit { get; set; }
        public double AtmIntendedWithdrawal { get; set; }
        public double AtmTotalTransactionAmount { get; set; }

        public TransactionTypes TransactionType { get; set; }
        public MerchantTypes MerchantTransactionType { get; set; }
        public DateTime MemberContactedMerchant { get; set; }
        public string MerchantNote { get; set; }

        public DateTime FiNotified { get; set; }
        public DateTime MemberFoundOutCardWasLostOrStolen { get; set; }
        public DateTime MemberFoundOutUnauthorizedCharges { get; set; }
        public DateTime DateFirstTransaction { get; set; }
        public DateTime MemberContactedFi { get; set; } //Same as FiNotified??
        public DateTime StatementDate { get; set; }
        public DateTime ProvisionalCreditDeadline { get; set; }
        public DateTime FinalDecisionDate { get; set; }
        public DateTime DateProvisionalCreditGiven { get; set; }
        //public DateTime MyProperty { get; set; }

        public double Bucket0 { get; set; }
        public double Bucket100 { get; set; }
        public double Bucket50 { get; set; } //=0
        public double Bucket500 { get; set; }

        public bool Liability50 { get; set; }
        public bool Liability500 { get; set; }

        public string MemberDisputeStatement { get; set; }
        public ClaimStatuses ClaimStatus { get; set; }

        //Table Linking
        public ICollection<DebitCardFraudTransaction> Transactions { get; set; }
        public ICollection<DebitCardFraudComment> Comments { get; set; }
    }
}