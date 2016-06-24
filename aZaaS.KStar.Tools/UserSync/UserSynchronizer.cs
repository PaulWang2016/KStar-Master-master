using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using aZaaS.KStar;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;

using UserSync.Models;
using System.Configuration;


namespace UserSync
{
    public static class UserSynchronizer
    {
        //Everyone - Role Id
        static readonly Guid eRoleId = Guid.Parse("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8");
       
        public static void Sync(HashSet<User> adUsers, HashSet<string> excludeDNs = null)
        {
            var userBO = new UserBO();
            var userService = new UserService();
            Logging log = new Logging();

            //Excluding the specified domain users
            if (excludeDNs != null)
            {
                log.ProcessLog(string.Format("Excluding the specified domains:{0} users ...", string.Join(",", excludeDNs.ToArray())));
                ConsoleMsg.Highlight("Excluding the specified domains:{0} users ...", string.Join(",", excludeDNs.ToArray()));
                adUsers.RemoveWhere(u => excludeDNs.Contains(u.Domain));
            }

            foreach (var adUser in adUsers)
            {
                log.ProcessLog(string.Format("Checking whether user:{0} exists ...", adUser.ToString()));

                var user = userService.ReadUserBase(adUser.UserFQDN);
                if (user == null)
                {
                    user = new UserBaseDto
                    {
                        SysID = Guid.NewGuid(),
                        Address = string.Empty,
                        UserName = adUser.UserFQDN,
                        Email = adUser.Email,
                        Phone = string.Empty,
                        Status = "True",
                        LastName = adUser.LastName,
                        FirstName = adUser.FirstName,
                        CreateDate = DateTime.Now,
                        Remark = adUser.Description
                    };

                    log.ProcessLog(string.Format("Creating user:{0} into system ...", adUser.ToString()));

                    var userID = userBO.CreateUser(user);

                    log.ProcessLog(string.Format("Assigning Everyone role to user:{0} ...", adUser.ToString()));
                    userBO.AppendRole(userID, eRoleId);            
                }
                else
                {
                    user.FirstName = adUser.FirstName;
                    user.LastName = adUser.LastName;
                    user.Email = adUser.Email;
                    user.Remark = adUser.Description;

                    log.ProcessLog(string.Format("Updating user:{0} information ...", adUser.ToString()));

                    userBO.UpdateUser(user);
                }
            }
        }
    }
}
