using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Storage.DataInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorBoilerplate.Infrastructure.Storage.DataModels
{
    public partial class StatusHistory : ISoftDelete
    {
        [Key]
        public long StatusHistoryId { get; set; }

        public int OldStatus { get; set; }
        public int NewStatus { get; set; }

        //public Guid ChangedBy { get; set; }
        //public DateTime ChangedOn { get; set; }

        //public long? ClaimId { get; set; }
        //public long? TransactionId { get; set; }
    }
}