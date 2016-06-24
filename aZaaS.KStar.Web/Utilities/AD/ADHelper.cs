using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Text;

namespace aZaaS.KStar.Utilities
{
    public class ADHelper
    {
        /// 
        /// 域名,比如：Denallix
        /// 
        private static string Domain = ConfigurationManager.AppSettings["Domain"];

        /// 
        /// 域名,比如：Denallix
        /// 
        private static string ADDomain= ConfigurationManager.AppSettings["DefaultDomain"];

        /// 
        /// LDAP绑定路径,比如：LDAP://DENALLIX.COM
        /// 
        private static string ADPath = ConfigurationManager.AppSettings["ADConnString"];
        /// 
        /// 登录帐号,比如：TestUser
        /// 
        private static string ADAdmin = ConfigurationManager.AppSettings["DefaultAdminUser"];
        /// 
        /// 登录密码,比如：123456
        /// 
        private static string ADPassword = ConfigurationManager.AppSettings["AdminPassword"];

        private static string GetDomainReference(string domain)
        {
            var domainReference = new StringBuilder();
            var dcs = domain.Split('.');

            foreach (var d in dcs)
            {
                domainReference.AppendFormat("DC={0},", d);
            }

            domainReference.Remove(domainReference.Length - 1, 1);

            return domainReference.ToString();
        }
        private static string GetPath(string adServer, string domain)
        {
            return string.Format("LDAP://{0}/{1}", adServer, GetDomainReference(domain));
        }

        public static IList<DirectoryEntry> GetDirectoryEntries()
        {
            var des = new List<DirectoryEntry>();
            var de = GetDirectoryObject();
            var filter = "(&(objectCategory=user)(!cn=Administrator))";
            var deSearch = new DirectorySearcher(de, filter);
            
            var results = deSearch.FindAll();

            foreach (SearchResult r in results)
            {
                des.Add(r.GetDirectoryEntry());
            }

            return des;
        }

        public static DirectoryEntry GetDirectoryEntry(string username)
        {
            var de = GetDirectoryObject();
            var filter = string.Format("(&(objectCategory=user)(|(cn={0})(userPrincipalName={1})))", username, username + "@" + Domain);
            var deSearch = new DirectorySearcher(de) { SearchRoot = de, Filter = filter };
            var results = deSearch.FindOne();

            return results != null ? results.GetDirectoryEntry() : null;
        }

        public static DirectoryEntry GetDirectoryEntry(string commonname, string category)
        {
            var filter = string.Format("(&(objectCategory={0})(cn={1}))", category, commonname);
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de, filter);

            try
            {
                SearchResult result = deSearch.FindOne();

                return new DirectoryEntry(result.Path,ADAdmin,ADPassword);
            }
            catch
            {
                return null;
            }
        }

        public static DirectoryEntry GetDirectoryObject()
        {
            return GetDirectoryObject(ADAdmin, ADPassword);
        }

        public static DirectoryEntry GetDirectoryObject(string username, string password)
        {
            return new DirectoryEntry(ADPath, username, password);
        }

        public static DirectoryEntry CreateUserAccount(string username, string password)
        {
            var de = GetDirectoryObject();
            var subDe = de.Children.Find("CN=Users");
            var newUser = subDe.Children.Add("CN=" + username, "user");
            try
            {
                newUser.Properties[ADUserProperty.Account.UserLogonNamePreWin2000].Value = username;
                newUser.Properties[ADUserProperty.Account.UserLogonName].Add(username + "@" + Domain);
                newUser.Properties[ADUserProperty.Account.LockOutTime].Value = 0;
                newUser.CommitChanges();

                newUser.Invoke("SetPassword", new object[] { password });
                newUser.CommitChanges();

                de.Close();
                newUser.Close();
            }
            catch (DirectoryServicesCOMException ex)
            {
                throw ex;
            }

            return newUser;
        }

        public static void ResetPassword(string username, string password)
        {
            var de = GetDirectoryEntry(username);
            if (de != null)
            {
                try
                {
                    de.Invoke("SetPassword", new object[] { password });
                    de.Close();
                }
                catch (DirectoryServicesCOMException ex)
                {
                    throw ex;
                }
            }
        }

        public static bool IsDirectoryObjectExists(string username)
        {
            return GetDirectoryEntry(username) != null;
        }

        public static void EnableUserAccount(DirectoryEntry de)
        {
            try
            {
                int val = (int)de.Properties[ADUserProperty.Account.AccountIsLockedOut].Value;
                de.Properties[ADUserProperty.Account.AccountIsLockedOut].Value = val & ~0x2;
                de.CommitChanges();
                de.Close();
            }
            catch (DirectoryServicesCOMException ex)
            {
                throw ex;
            }
        }

        public static void EnableUserAccount(string username)
        {
            EnableUserAccount(GetDirectoryEntry(username));
        }

        public static void DisableUserAccount(string username)
        {
            try
            {
                var user = GetDirectoryEntry(username);
                int val = (int)user.Properties[ADUserProperty.Account.AccountIsLockedOut].Value;
                user.Properties[ADUserProperty.Account.AccountIsLockedOut].Value = val | 0x2;
                user.CommitChanges();
                user.Close();
            }
            catch (DirectoryServicesCOMException ex)
            {
                throw ex;
            }
        }

        public static void RemoveUserFromGroup(string username, string groupname)
        {
            DirectoryEntry group = GetDirectoryEntry(groupname, "group");
            DirectoryEntry user = GetDirectoryEntry(username);

            if (group.Properties["member"].Contains(user.Properties[ADUserProperty.Account.DistinguishedName].Value))
            {
                group.Properties["member"]
                    .Remove(user.Properties[ADUserProperty.Account.DistinguishedName].Value);
            }

            group.CommitChanges();
            group.Close();
            user.Close();
        }

        public static void AddUserToGroup(string username, string groupname)
        {
            DirectoryEntry group = GetDirectoryEntry(groupname, "group");
            DirectoryEntry user = GetDirectoryEntry(username);

            if (!group.Properties["member"].Contains(user.Properties[ADUserProperty.Account.DistinguishedName].Value))
            {
                group.Properties["member"]
                    .Add(user.Properties[ADUserProperty.Account.DistinguishedName].Value);
            }
            group.CommitChanges();
            group.Close();
            user.Close();
        }

    }
}