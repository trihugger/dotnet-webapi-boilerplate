using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace PEXHub
{
    public class ADconn
    {
        // ip:10.61.254.171
        // port:389
        // root:dc=fcfcu,dc=firstcitizens,dc=org

        public enum authgroup
        {
            Admin,
            Loans,
            Deposit,
            MAG,
            Consumer,
            Commercial,
            Residential,
            Accounting,
            Retail,
            Contact,
            Guest
        }

        private UserPrincipal User;

        public string DisplayName { get; }
        public string Email { get; set; }
        public List<string> Groups { get; set; }
        public string Department { get; set; }
        public string AuthGroup { get; set; }

        private readonly string _domain = "FCFCU";

        public ADconn(string username)
        {
            List<string> rgroups = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                User = UserPrincipal.FindByIdentity(context, username);

                if (User != null)
                {
                    //Get Name
                    DisplayName = User.DisplayName;

                    //Get Groups
                    PrincipalSearchResult<Principal> groups = User.GetAuthorizationGroups();

                    foreach (Principal group in groups)
                    {
                        if (group is GroupPrincipal)
                        {
                            rgroups.Add(group.ToString());
                        }
                    }
                    Groups = rgroups;

                    //Get Email
                    Email = User.EmailAddress;

                    //Get Department
                    DirectoryEntry directory = User.GetUnderlyingObject() as DirectoryEntry;
                    Department = directory.Properties["department"].Value.ToString();

                    //Get Auth Group
                    AuthGroup = GetAuthGroup().ToString();
                }
            }

        }

        public ADconn(string username, string password)
        {
            List<string> rgroups = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, _domain, username, password))
            {
                User = UserPrincipal.FindByIdentity(context, username);

                if (User != null)
                {
                    // Get Name
                    DisplayName = User.DisplayName;

                    // Get Groups
                    PrincipalSearchResult<Principal> groups = User.GetAuthorizationGroups();

                    foreach (Principal group in groups)
                    {
                        if (group is GroupPrincipal)
                        {
                            rgroups.Add(group.ToString());
                        }
                    }

                    Groups = rgroups;

                    // Get Email
                    Email = User.EmailAddress;

                    // Get Department
                    DirectoryEntry directory = User.GetUnderlyingObject() as DirectoryEntry;
                    Department = directory.Properties["department"].Value.ToString();

                    // Get Auth Group
                    AuthGroup = GetAuthGroup().ToString();
                }
            }

        }

        public bool Authenticate(string auth)
        {
            foreach (string group in Groups)
            {
                if (group.ToLower().Contains(auth.ToLower())) { return true; }
            }

            if (Department.ToLower().Contains(auth.ToLower())) { return true; } else { return false; }
        }

        public authgroup GetAuthGroup()
        {
            if (Authenticate("Process Excellence")) { return authgroup.Admin; }

            if (Authenticate("Loan")) { return authgroup.Loans; }

            if (Authenticate("Consumer Lending")) { return authgroup.Loans; }

            if (Authenticate("Retail")) { return authgroup.Retail; }

            if (Authenticate("Contact")) { return authgroup.Contact; }

            return authgroup.Guest;
        }

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                var u = new UserPrincipal(context);

                using (var search = new PrincipalSearcher(u))
                {
                    PrincipalSearchResult<Principal> results = search.FindAll();

                    foreach (Principal user in results)
                    {
                        if (user.DisplayName != null)
                        {
                            string userName = user.DisplayName;

                            bool userExists = users.Contains(userName);

                            if (!userExists && isUser(userName))
                            {
                                users.Add(userName);
                            }
                        }
                    }
                }
            }
            users.Sort();

            return users;
        }

        public List<string> GetDepartments()
        {
            List<string> departments = new List<string>();

            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                var u = new UserPrincipal(context);
                using (var search = new PrincipalSearcher(u))
                {
                    PrincipalSearchResult<Principal> results = search.FindAll();
                    foreach (Principal user in results)
                    {
                        DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;
                        if (user.DisplayName != null)
                        {
                            if (directoryEntry.Properties["department"].Value != null)
                            {
                                string dept = directoryEntry.Properties["department"].Value.ToString();

                                bool deptExists = departments.Contains(dept);

                                if (!deptExists && !dept.Contains("%"))
                                {
                                    departments.Add(dept);
                                }
                            }
                        }
                    }
                }
            }

            departments.Sort();

            return departments;
        }

        private bool isUser(string username)
        {
            List<string> excludeUsers = new List<string>();
            bool isUser = true;

            excludeUsers.Add("FCFCU");
            excludeUsers.Add("fcfcu");
            excludeUsers.Add(".adm");
            excludeUsers.Add("Okta");
            excludeUsers.Add("papersrv");
            excludeUsers.Add("orchestrataor");
            excludeUsers.Add("Robot");
            excludeUsers.Add("Histroy");
            excludeUsers.Add("Identifi");
            excludeUsers.Add("%");
            excludeUsers.Add("LDAP");
            excludeUsers.Add("GRA");
            excludeUsers.Add("__");
            excludeUsers.Add("Teller");
            excludeUsers.Add("Platform");
            excludeUsers.Add("Conway");
            excludeUsers.Add("Audit");
            excludeUsers.Add("Horizon");

            foreach (string excludeName in excludeUsers)
            {
                isUser = isUser && !username.Contains(excludeName);
            }

            return isUser;
        }
    }
}
