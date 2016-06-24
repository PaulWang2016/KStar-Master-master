using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities.OuterNetwork
{
    /// <summary>
    ///  OuterNetworkUtilites
    /// </summary>
    public class OuterNetworkUtilites
    {
        /// <summary>
        /// 判断是否是外网ip
        /// 外网ip 段
        /// A类 10.0.0.0到10.255.255.255 
        /// B类 172.16.0.0到172.31.255.255 
        /// C类 192.168.0.0到192.168.255.255
        /// </summary>
        /// <param name="hostAddress">ip</param>
        /// <returns>
        /// true 外网ip
        /// false 内网ip
        /// </returns>
        public static bool IsOuterNetwork(string hostAddress)
        {
            if (hostAddress == "127.0.0.1" || hostAddress=="::1")
            {
                return false;
            }
            if ((hostAddress+string.Empty).Trim() == "192.168.15.2")//15.2 为外网
            {
                return true;
            }
            var splitAddress = hostAddress.Split('.');

            if (splitAddress[0].Trim() == "192" && splitAddress[1].Trim() == "168")
            {
                return false;
            }
            else if (splitAddress[0].Trim() == "172" && splitAddress[1].Trim() == "16")
            {
                return false;
            }
            else if (splitAddress[0].Trim() == "10")
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 该用户是否具有外网访问权限
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkRole(string account)
        {
            var code = aZaaS.KStar.Web.Models.BasisEntity.EnumCollection.FeaturePermissionCode.InternetRole.ToString();
            return CheckRole(account, code);
        }

        public static bool IsNetworkDownloadRole(string account)
        {
            var code = aZaaS.KStar.Web.Models.BasisEntity.EnumCollection.FeaturePermissionCode.InternetDownloadRole.ToString();
            return CheckRole(account, code);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool CheckRole(string account,string code)
        {
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var linq = from role in basisEntity.NetworkRoles
                           join roleUser in basisEntity.RoleUsers
                           on role.RoleID equals roleUser.Role_SysId
                           into RoleInfoTemp
                           from RoleInfo in RoleInfoTemp.DefaultIfEmpty()
                           join user in basisEntity.Users
                           on RoleInfo.User_SysId equals user.SysId
                           where user.UserName == account && role.Code == code
                           select new
                           {
                               SysId = user.SysId
                           };
                var count = linq.Count();
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}