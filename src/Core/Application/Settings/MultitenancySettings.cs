namespace DN.WebApi.Application.Settings
{
    public class MultitenancySettings
    {
        public string DBProvider { get; set; }
        public string ConnectionString { get; set; }
        public bool UseAD { get; set; }
        public string AdDomain { get; set; }
    }
}