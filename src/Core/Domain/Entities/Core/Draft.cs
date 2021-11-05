using DN.WebApi.Domain.Contracts;
using DN.WebApi.Domain.Extensions;

namespace DN.WebApi.Domain.Entities.Core
{
    public class Draft : AuditableEntity, IMustHaveTenant
    {
        public string TenantKey { get; set; }
    }
}
