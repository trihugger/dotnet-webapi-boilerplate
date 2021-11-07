using DN.WebApi.Domain.Contracts;
using DN.WebApi.Domain.Enums.Core;
using DN.WebApi.Domain.Extensions;

namespace DN.WebApi.Domain.Entities.Core
{
    public class Address : AuditableEntity, IMustHaveTenant
    {
        public AddressType AddressType { get; set; }
        public string AttentionLine { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip
        {
            get
            {
                return Zip5 + (!string.IsNullOrEmpty(Zip4) ? "-" + Zip4 : string.Empty);
            }
            set
            {
                string zip = value.Replace("-", string.Empty);
                Zip5 = zip.Substring(0, 5);
                Zip4 = zip.Substring(5);
            }
        }

        public string Zip5 { get; private set; }
        public string Zip4 { get; private set; }

        public string TenantKey { get; set; }
    }
}
