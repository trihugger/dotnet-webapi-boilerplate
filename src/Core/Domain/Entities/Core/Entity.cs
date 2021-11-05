using DN.WebApi.Domain.Contracts;
using DN.WebApi.Domain.Enums;
using DN.WebApi.Domain.Extensions;

namespace DN.WebApi.Domain.Entities.Core
{
    public class Entity : AuditableEntity, IMustHaveTenant
    {
        public string TaxId { get; set; }
        public string Name { get; set; }
        public EntityType EntityType { get; set; }
        public string TenantKey { get; set; }
    }
}
