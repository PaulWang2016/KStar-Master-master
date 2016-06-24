
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace aZaaS.KStar.Utilities
{
    /// <summary>
    /// Active Directory操作集合类
    /// </summary>
    public static class ADUtility
    {
        /// <summary>
        /// 禁用账户
        /// </summary>
        /// <param name="username">账户名称</param>
        public static void DisableUser(string username)
        {
            ADHelper.DisableUserAccount(username);
        }

        /// <summary>
        /// 启用账户
        /// </summary>
        /// <param name="username">账户名称</param>
        public static void EnableUser(string username)
        {
            ADHelper.EnableUserAccount(username);
        }

        /// <summary>
        /// 检测账户是否存在，若存在返回true，否则返回false
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <returns>若账户存在返回true，否则返回false</returns>
        public static bool IsUserExists(string username)
        {
            return ADHelper.IsDirectoryObjectExists(username);
        }

        /// <summary>
        /// 管理员重置密码
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="password">新秘密</param>
        public static void ResetPassword(string username, string password)
        {
            ADHelper.ResetPassword(username, password);
        }

        /// <summary>
        /// 创建账户
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="password">密码</param>
        public static void CreateUser(string username, string password)
        {
            ADHelper.CreateUserAccount(username, password);
        }

        /// <summary>
        /// 添加账户到组
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="groupname">组名称</param>
        public static void AddUserToGroup(string username, string groupname)
        {
            ADHelper.AddUserToGroup(username, groupname);
        }

        /// <summary>
        /// 从组中移除账户
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="groupname">组名</param>
        public static void RemoveUserFromGroup(string username, string groupname)
        {
            ADHelper.RemoveUserFromGroup(username, groupname);
        }

        /// <summary>
        /// 更新账户信息，返回账户属性更新字典
        /// key存放属性名
        /// value为存放属性新旧值的字典：key存放:Name、OldValue和NewValue；value存放账户名称、属性新值和属性旧值
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="properties">账户属性字典</param>
        /// <returns>账户属性更新字典</returns>
        public static IDictionary<string, IDictionary<string, string>> UpdateUser(string username, IDictionary<string, string> properties)
        {
            return UpdateUser(ADHelper.GetDirectoryEntry(username), properties);
        }

        /// <summary>
        /// 获取大部分常用的AD属性名
        /// </summary>
        /// <returns>包含大部分常用AD属性名的集合</returns>
        public static IList<string> GetADPropertyNames()
        {
            IList<string> propertyNames = new List<string>();
            var types = typeof(ADUserProperty).GetNestedTypes();

            foreach (var t in types)
            {
                var fieldInfo = t.GetFields();
                foreach (var fi in fieldInfo)
                {
                    propertyNames.Add(fi.GetValue(null).ToString());
                }
            }

            propertyNames = propertyNames.Distinct().ToList();

            return propertyNames;
        }

        /// <summary>
        /// 获取指定账户的常用AD属性
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <returns>包含AD属性名称和属性值的字典</returns>
        public static IDictionary<string, string> GetUser(string username)
        {
            var propertyNames = GetADPropertyNames();
            return GetUser(username, propertyNames.ToArray());
        }

        /// <summary>
        /// 获取指定账户的指定AD属性
        /// </summary>
        /// <param name="username">账户名称</param>
        /// <param name="propertyNames">指定的属性名称数组</param>
        /// <returns>包含属性名称和值的字典</returns>
        public static IDictionary<string, string> GetUser(string username, string[] propertyNames)
        {
            var d = new Dictionary<string, string>(propertyNames.Length);
            var de = ADHelper.GetDirectoryEntry(username);

            foreach (string pn in propertyNames)
            {
                d.Add(pn, GetDEPropreryValue(de, pn));
            }

            return d;
        }

        /// <summary>
        /// 获取AD中所有账户信息，仅包含指定的AD属性,Administrator除外。
        /// 包含账户名称，属性名称和属性值的字典.
        /// key存放账户名称，
        /// value存放属性名称和属性值的字典，key:属性名称，value:属性值       
        /// </summary>
        /// <param name="propertyNames">指定要返回的AD属性名称</param>
        /// <returns>
        /// 包含帐号名称、属性名称和属性值的字典
        /// </returns>
        public static IDictionary<string, IDictionary<string, string>> GetAllUsers(string[] propertyNames)
        {
            var d = new Dictionary<string, IDictionary<string, string>>();
            var des = ADHelper.GetDirectoryEntries();
            foreach (var de in des)
            {
                var d2 = new Dictionary<string, string>(propertyNames.Length);
                foreach (var pn in propertyNames)
                {
                    d2.Add(pn, GetDEPropreryValue(de, pn));
                }

                var username = GetDEPropreryValue(de, ADUserProperty.Account.UserLogonName);
                if (!string.IsNullOrEmpty(username))
                {
                    d.Add(username, d2);
                }
            }

            return d;
        }

        /// <summary>
        /// 获取AD中所有账户信息，包常用的AD属性,Administrator除外。
        /// 包含账户名称，属性名称和属性值的字典.
        /// key存放账户名称，
        /// value存放属性名称和属性值的字典，key:属性名称，value:属性值
        /// </summary>
        /// <returns>
        /// 包含帐号名称、属性名称和属性值的字典
        /// </returns>
        public static IDictionary<string, IDictionary<string, string>> GetAllUsers()
        {
            var propertyNames = GetADPropertyNames();
            return GetAllUsers(propertyNames.ToArray());
        }


        private static IDictionary<string,IDictionary<string,string>> UpdateUser(DirectoryEntry de, IDictionary<string, string> properties)
        {
            var oldPropertyValue = string.Empty;
            var propertyUpdateLog = new Dictionary<string, IDictionary<string, string>>();

            foreach (var p in properties)
            {
                if (de.Properties.Contains(p.Key))
                {
                    oldPropertyValue = GetDEPropreryValue(de, p.Key);
                    if (oldPropertyValue != p.Value && !string.IsNullOrEmpty(p.Value))
                    {
                        de.Properties[p.Key][0] = p.Value;
                        var oldAndNewPropertyValues = new Dictionary<string, string>();
                        oldAndNewPropertyValues.Add("Name", de.Name);
                        oldAndNewPropertyValues.Add("OldValue", oldPropertyValue.ToString());
                        oldAndNewPropertyValues.Add("NewValue", p.Value);
                        propertyUpdateLog.Add(p.Key, oldAndNewPropertyValues);
                    }
                }
                else if (!string.IsNullOrEmpty(p.Value))
                {
                    de.Properties[p.Key].Add(p.Value);
                    var oldAndNewPropertyValues = new Dictionary<string, string>();
                    oldAndNewPropertyValues.Add("Name", de.Name);
                    oldAndNewPropertyValues.Add("OldValue", oldPropertyValue.ToString());
                    oldAndNewPropertyValues.Add("NewValue", p.Value);
                    propertyUpdateLog.Add(p.Key, oldAndNewPropertyValues);
                }

                de.CommitChanges();
            }

            de.CommitChanges();
            de.Close();

            return propertyUpdateLog;
        }

        private static string GetDEPropreryValue(DirectoryEntry de, string propertyName)
        {
            return de.Properties[propertyName].Value != null ?
                de.Properties[propertyName].Value.ToString() :
                string.Empty;
        }

        private static void EnableUser(DirectoryEntry de)
        {
            ADHelper.EnableUserAccount(de);
        }
    }
}