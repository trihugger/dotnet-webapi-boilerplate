namespace DN.WebApi.Application.Settings
{
    public class MultitenancySettings
    {
        public string DBProvider { get; set; }
        public string ConnectionString { get; set; }
        public bool UseAD { get; set; } = false;
        public bool RoleFromGroup { get; set; } = true;
        public bool RoleFromDepartment { get; set; } = true;
        public bool AuthorizeOffline { get; set; } = true;
    }
}