using DN.WebApi.Domain.Contracts;
using DN.WebApi.Domain.Enums;
using DN.WebApi.Domain.Extensions;

namespace DN.WebApi.Domain.Entities.Core
{
    public class Account : AuditableEntity, IMustHaveTenant
    {
        public string AccountNumber { get; set; }
        public AccountStatus Status { get; set; }
        public Major Major { get; set; }
        public string Minor { get; set; }

        public string TenantKey { get; set; }
    }
}
