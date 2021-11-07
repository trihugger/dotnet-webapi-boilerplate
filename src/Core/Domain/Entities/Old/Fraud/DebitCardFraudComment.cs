using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using BlazorBoilerplate.Infrastructure.Storage.DataInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorBoilerplate.Infrastructure.Storage.DataModels
{
    public partial class DebitCardFraudComment : IAuditable, ISoftDelete
    {
        [Key]
        public long CommentId { get; set; }
        public string Comment { get; set; }
        public long FraudClaimClaimId { get; set; }
    }
}