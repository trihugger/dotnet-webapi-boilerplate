using DN.WebApi.Application.Abstractions.Services.Identity;
using DN.WebApi.Application.Wrapper;
using DN.WebApi.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace DN.WebApi.Infrastructure.Identity.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ActiveDirectoryService()
        {

        }

        public ActiveDirectoryService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public bool AuthenticateAsync(string domain, string userName, string password)
        {
            UserPrincipal user;
            List<string> rgroups = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, domain, userName, password))
            {
                user = UserPrincipal.FindByIdentity(context, userName);

                if (user != null)
                {
                    return true;
                }
            }

            return false;
        }

        public Task<IResult> ImportAdUsersAsync()
        {
            try
            {
                UpdateUsers();
                return Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Result.FailAsync();
            }
        }

        public async Task<IResult> UpdateUserAsync(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                var response = await _userManager.AddPasswordAsync(user, password);
                if (response.Succeeded)
                    return Result.Success();
                else
                    return Result.Fail();
            }

            return Result.Fail();
        }

        #region USER IDENTITY METHODS
        private async Task<bool> UpdateUserAsync(ApplicationUser user, AdUser updatedUser)
        {
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            var response = await _userManager.UpdateAsync(user);
            if (response.Succeeded)
            {
                user = await _userManager.FindByIdAsync(updatedUser.UserName);
                if (user != null)
                    return await UpdateUserRoleAsync(user, updatedUser.Department);

                return false;
            }
            else
            {
                Console.WriteLine("ERROR: Could not update user " + user.UserName);
                return false;
            }
        }

        private async Task<bool> AddUserAsync(AdUser newUser)
        {
            ApplicationUser user = new ApplicationUser();
            user.UserName = newUser.UserName;
            user.Email = newUser.Email;
            user.FirstName = newUser.FirstName;
            user.LastName = newUser.LastName;
            var response = await _userManager.CreateAsync(user);
            if (response.Succeeded)
            {
                user = await _userManager.FindByIdAsync(newUser.UserName);

                // Add Department Role
                bool addedRole = await CreateRoleAsync(newUser.Department);

                // Attach Department Role to User
                if (addedRole)
                {
                    bool attachedRole = await AssignUserRoleAsync(user, newUser.Department);
                    return attachedRole;
                }

                return false;
            }
            else
            {
                Console.WriteLine("ERROR: Could not create user " + newUser.UserName);
                return false;
            }
        }

        private async Task<bool> AssignUserRoleAsync(ApplicationUser user, string roleName)
        {
            var response = await _userManager.AddToRoleAsync(user, roleName);
            if (response.Succeeded)
                return true;
            else
                return false;
        }

        private async Task<bool> UpdateUserRoleAsync(ApplicationUser user, string roleName)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                bool createdRole = await CreateRoleAsync(roleName);
                if (createdRole)
                    role = await _roleManager.FindByNameAsync(roleName);

                if (role == null) return false;
            }

            bool userHasRole = await RoleExistsAsync(user, roleName);

            if (!userHasRole)
            {
                bool roleDeleted = await DeleteUserRoleAsync(user, roleName);

                if (roleDeleted)
                {
                    bool roleAdded = await AssignUserRoleAsync(user, roleName);
                    return roleAdded;
                }
            }

            return true;
        }

        private async Task<bool> DeleteUserRoleAsync(ApplicationUser user, string roleName)
        {
            var response = await _userManager.RemoveFromRoleAsync(user, roleName);

            return response.Succeeded;
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

        private async Task<bool> RoleExistsAsync(ApplicationUser user, string roleName)
        {
            var response = await _userManager.GetRolesAsync(user);

            if (response != null)
                return response.Contains(roleName);
            else
                return false;
        }
        #endregion

        #region AD SUPPORT METHODs
        private async void UpdateUsers()
        {
            List<AdUser> users = GetAllUsers();
            foreach (AdUser user in users)
            {
                ApplicationUser currentUser = await _userManager.FindByIdAsync(user.UserName);
                bool userExists = currentUser != null;

                // TODO: add logs for create/update success and faillure;
                if (userExists)
                {
                    bool updatedUser = await UpdateUserAsync(currentUser, user);
                }
                else
                {
                    bool createdUser = await AddUserAsync(user);
                }
            }
        }

        private List<AdUser> GetAllUsers()
        {
            List<AdUser> adUsers = new List<AdUser>();

            SearchResultCollection users;
            DirectorySearcher directorySearcher = null;
            DirectoryEntry entry = new DirectoryEntry(GetCurrentDomainPath());

            directorySearcher = new DirectorySearcher(entry);
            directorySearcher.PropertiesToLoad.Add("userPrincipalName"); // username
            directorySearcher.PropertiesToLoad.Add("name"); // full name
            directorySearcher.PropertiesToLoad.Add("givenname"); // first name
            directorySearcher.PropertiesToLoad.Add("sn"); // last name
            directorySearcher.PropertiesToLoad.Add("mail"); // email
            directorySearcher.PropertiesToLoad.Add("department"); // deparment
            directorySearcher.Filter = "(&(objectCategory=User)(objectClass=person))";

            users = directorySearcher.FindAll();

            foreach(SearchResult user in users)
            {
                AdUser newUser = new AdUser();
                if (user.Properties["name"].Count > 0) newUser.FullName = user.Properties["name"][0].ToString();
                if (user.Properties["givenname"].Count > 0) newUser.FirstName = user.Properties["givenname"][0].ToString();
                if (user.Properties["sn"].Count > 0) newUser.LastName = user.Properties["sn"][0].ToString();
                if (user.Properties["mail"].Count > 0) newUser.Email = user.Properties["mail"][0].ToString();
                if (user.Properties["deparment"].Count > 0) newUser.Department = user.Properties["department"][0].ToString();
                adUsers.Add(newUser);
            }

            return adUsers;
        }

        private string GetCurrentDomainPath()
        {
            DirectoryEntry directory = new DirectoryEntry("LDAP://RootDSE");

            return "LDAP://" + directory.Properties["defaultNamingContext"][0].ToString();
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
    }
}
