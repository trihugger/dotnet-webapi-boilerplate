using DN.WebApi.Application.Abstractions.Services.General;
using DN.WebApi.Application.Abstractions.Services.Identity;
using DN.WebApi.Application.Wrapper;
using DN.WebApi.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

namespace DN.WebApi.Infrastructure.Identity.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly string[] _apiRoles = new string[] { "Basic" }; // list of roles not managed in AD
        private readonly string[] _excludeAdRoles = new string[] { "Administrators", "Users", "Domain Users", "ORA_DBA", "ORA_OraDB19Home1_SYSKM", "ORA_OraDB19Home1_SYSDG", "ORA_OraDB19Home1_SYSBACKUP", "ORA_ASMDBA" }; // list of roles from AD to exclude from users
        private readonly Dictionary<string, string> _adRolesMapping = new Dictionary<string, string>
        {
            { "Administrators", "Admin" },
            { "ProcessExcellence", "Process Excellence" }
        }; // Roles Mapping that you would like mapped to the Api's roles
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantService _tenantService;
        private UserPrincipal _user;
        private Domain _domain;

        public ActiveDirectoryService()
        {
            _domain = GetCurrentDomainPath();
        }

        public ActiveDirectoryService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ITenantService tenantService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tenantService = tenantService;
            _domain = GetCurrentDomainPath();
        }

        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            bool userProcessed = false;

            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain.DomainName, userName, password);
                _user = UserPrincipal.FindByIdentity(context, userName);
                if(_user != null) userProcessed = await ProcessUserAsync(userName, password, _tenantService.RolesFromDepartment(), _tenantService.RolesFromGroups());
                context.Dispose();
                return userProcessed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something is wrong: " + ex.Message);
                return false;
            }

            // using (var context = new PrincipalContext(ContextType.Domain, _domain.DomainName, userName, password))
            // {
            //
            //    _user = UserPrincipal.FindByIdentity(context, userName);
            // if (_user != null)
            //    {
            //        bool userProcessed = await ProcessUserAsync(userName, password, _tenantService.RolesFromDepartment(), _tenantService.RolesFromGroups());
            //        return userProcessed;
            //    }
            // }
        }

        public async Task<IResult> ImportAdUsersAsync()
        {
            var adUsers = GetAllUsers();

            foreach(AdUser user in adUsers)
            {
                bool processed = await ProcessUserAsync(user.UserName, rolesFromDeparment: _tenantService.RolesFromDepartment(), rolesFromGroups: _tenantService.RolesFromGroups());
                if (!processed) return Result.Fail();
            }

            return Result.Success();
        }

        public async Task<IResult> UpdateUserAsync(string userName, string password, bool rolesFromDeparment = true, bool rolesFromGroups = true)
        {
            if (_user == null) return Result.Fail(); // User was never authenticated
            bool processedUser = await ProcessUserAsync(userName, password, rolesFromDeparment, rolesFromGroups);
            if (processedUser) return Result.Success();
            else return Result.Fail();
        }

        #region USER IDENTITY METHODS
        private async Task<bool> ProcessUserAsync(string userName, string password = "", bool rolesFromDeparment = true, bool rolesFromGroups = true)
        {
            IdentityResult response = null;
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            var adUser = await GetUserAsync();
            if (user == null)
            {
                bool addUserResponse = await AddUserAsync(adUser); // Add User
                if (addUserResponse) user = await _userManager.FindByNameAsync(userName);
                else return false;
                if (user == null) return false;
            }
            else
            {
                bool updatedUserResponse = await UpdateUserAsync(user, adUser); // Update User
                if (updatedUserResponse) user = await _userManager.FindByNameAsync(userName);
                else return false;
                if(user == null) return false;
            }

            if (!string.IsNullOrEmpty(password))
            {
                response = await _userManager.AddPasswordAsync(user, password); // Add Password
                if (!response.Succeeded)
                {
                    if (response.ToString().Contains("UserAlreadyHasPassword"))
                    {
                        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        response = await _userManager.ResetPasswordAsync(user, token, password); // Update Password
                        if (!response.Succeeded) return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if(rolesFromDeparment || rolesFromGroups)
            {
                List<string> roles = new List<string>();
                if (rolesFromDeparment) roles.Add(adUser.Department);
                if(rolesFromGroups)
                    foreach (string role in adUser.Groups) roles.Add(role);
                bool rolesProccessed = await ProcessRolesAsync(user, roles); // Process Roles
                if(!rolesProccessed) return false;
            }

            return true;
        }

        private async Task<AdUser> GetUserAsync(string username = "", string email = "")
        {
            List<AdUser> adUsers = await Task.Run(() => GetAllUsers());
            AdUser newUser = new AdUser();
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(email))
            {
                if (!string.IsNullOrEmpty(username)) newUser = adUsers.Find(t => t.UserName == username);
                if (!string.IsNullOrEmpty(email)) newUser = adUsers.Find(t => t.Email == email);
            }
            else if (_user != null)
            {
                newUser = adUsers.Find(t => t.UserName == _user.SamAccountName);
            }

            return newUser;
        }

        private async Task<bool> AddUserAsync(AdUser newUser)
        {
            ApplicationUser user = new ApplicationUser();

            user.UserName = newUser.UserName;
            user.Email = newUser.Email;
            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            user.NormalizedEmail = newUser.Email;
            user.NormalizedUserName = newUser.UserName.ToLower();
            user.IsActive = true;
            user.EmailConfirmed = true;
            user.TenantKey = _tenantService.GetCurrentTenant()?.Key;
            var response = await _userManager.CreateAsync(user);
            if (!response.Succeeded)
            {
                Console.WriteLine("ERROR: Could not create user " + newUser.UserName);
                return false;
            }

            return true;
        }

        private async Task<bool> UpdateUserAsync(ApplicationUser user, AdUser updatedUser)
        {
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            var response = await _userManager.UpdateAsync(user);
            if (response.Succeeded)
            {
                user = await _userManager.FindByNameAsync(updatedUser.UserName);
                if (user == null) return false;
            }
            else
            {
                Console.WriteLine("ERROR: Could not update user " + user.UserName);
                return false;
            }

            return true;
        }
        #endregion

        #region ROLES IDENTITY METHODS
        private async Task<bool> ProcessRolesAsync(ApplicationUser user, List<string> roles)
        {
            roles = StandarizeRoles(roles); // Remove Excludes and Changed the Mapped roles
            IList<string> userRoles = await GetUserRolesAsync(user); // Get all roles for User
            foreach (string role in userRoles)
            {
                if(!roles.Contains(role) && !_apiRoles.Contains(role))
                {
                    bool deleted = await DeleteUserRoleAsync(user, role);
                    if (!deleted) return false;
                }
            } // Delete Old Roles

            foreach (string role in roles)
            {
                if (!userRoles.Contains(role))
                {
                    bool processed = await ProcessRoleAsync(user, role);
                    if (!processed) return false;
                }
            } // Attach new Roles

            return true;
        }

        private async Task<bool> ProcessRoleAsync(ApplicationUser user, string roleName)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                bool createdRole = await CreateRoleAsync(roleName); // Create Role if it doesn't exist
                if (createdRole) role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) return false;
            }

            bool roleAdded = await AssignUserRoleAsync(user, roleName); // Assign Role to User
            return roleAdded;
        }

        private List<string> StandarizeRoles(List<string> roles)
        {
            List<string> newRoles = new List<string>();
            foreach(string role in roles)
            {
                if (_adRolesMapping.ContainsKey(role)) newRoles.Add(_adRolesMapping[role]); // Map roles to Api roles
                else if (!_excludeAdRoles.Contains(role)) newRoles.Add(role); // Skip if should be excluded from Roles
            }

            return newRoles;
        }

        private async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        private async Task<bool> AssignUserRoleAsync(ApplicationUser user, string roleName)
        {
            var response = await _userManager.AddToRoleAsync(user, roleName);
            if (response.Succeeded)
                return true;
            else
                return false;
        }

        private async Task<bool> CreateRoleAsync(string roleName)
        {
            var response = await _roleManager.FindByNameAsync(roleName);

            if (response != null)
                return true;

            ApplicationRole newrole = new ApplicationRole();
            newrole.Name = roleName;
            newrole.TenantKey = "root"; // TODO: add tenant filter;

            var createresponse = await _roleManager.CreateAsync(newrole);
            if (createresponse.Succeeded)
                return true;

            return false;
        }

        private async Task<bool> DeleteUserRoleAsync(ApplicationUser user, string roleName)
        {
            var response = await _userManager.RemoveFromRoleAsync(user, roleName);

            return response.Succeeded;
        }

        private async Task<bool> RoleExistsAsync(ApplicationUser user, string roleName)
        {
            var response = await GetUserRolesAsync(user);

            if (response != null)
                return response.Contains(roleName);
            else
                return false;
        }
        #endregion

        #region AD SUPPORT METHODs
        private List<AdUser> GetAllUsers()
        {
            List<AdUser> adUsers = new List<AdUser>();

            SearchResultCollection users;
            DirectorySearcher directorySearcher = null;
            DirectoryEntry entry = new DirectoryEntry(_domain.Connection);

            directorySearcher = new DirectorySearcher(entry);
            directorySearcher.PropertiesToLoad.Add("userPrincipalName"); // email
            directorySearcher.PropertiesToLoad.Add("name"); // full name
            directorySearcher.PropertiesToLoad.Add("givenname"); // first name
            directorySearcher.PropertiesToLoad.Add("sn"); // last name
            directorySearcher.PropertiesToLoad.Add("samAccountName"); // username
            directorySearcher.PropertiesToLoad.Add("department"); // deparment
            directorySearcher.Filter = "(&(objectCategory=User)(objectClass=person))";

            users = directorySearcher.FindAll();

            foreach(SearchResult user in users)
            {
                // Add User info
                AdUser newUser = new AdUser();
                if (user.Properties["samaccountname"].Count > 0) newUser.UserName = user.Properties["samaccountname"][0].ToString();
                if (user.Properties["name"].Count > 0) newUser.FullName = user.Properties["name"][0].ToString();
                if (user.Properties["givenname"].Count > 0) newUser.FirstName = user.Properties["givenname"][0].ToString();
                if (user.Properties["sn"].Count > 0) newUser.LastName = user.Properties["sn"][0].ToString();
                if (user.Properties["userprincipalname"].Count > 0) newUser.Email = user.Properties["userprincipalname"][0].ToString();
                if (user.Properties["department"].Count > 0) newUser.Department = user.Properties["department"][0].ToString();

                // Get Groups
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain, _domain.DomainName), IdentityType.SamAccountName, newUser.UserName);
                foreach(GroupPrincipal group in userPrincipal.GetGroups()) newUser.Groups.Add(group.Name);
                adUsers.Add(newUser);
            }

            return adUsers;
        }

        private Domain GetCurrentDomainPath()
        {
            DirectoryEntry directory = new DirectoryEntry("LDAP://RootDSE");

            Domain domain = new Domain();
            domain.Connection = "LDAP://" + directory.Properties["defaultNamingContext"][0].ToString();
            domain.DcNames = new List<string>();
            string[] dcs = directory.Properties["defaultNamingContext"][0].ToString().Split(",");
            foreach(string s in dcs)
            {
                domain.DcNames.Add(s.Replace("DC=", string.Empty));
            }

            domain.DomainName = domain.DcNames[0];
            return domain;
        }
        #endregion
    }

    public class AdUser
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public List<string> Groups { get; set; } = new List<string>();
    }

    public class Domain
    {
        public string Connection { get; set; }
        public string DomainName { get; set; }
        public List<string> DcNames { get; set; }
    }
}
