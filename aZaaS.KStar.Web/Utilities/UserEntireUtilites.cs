using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace aZaaS.KStar.Web.Utilities
{
    public class UserEntireUtilites
    {
        /// <summary>
        /// 获取用户信息（部门、职位)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserEntire GetUserEntire(string userName)
        {
            List<UserEntire> userList = null;
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var linq = from u in basisEntity.Users
                           join un in basisEntity.UserOrgNodes
                                   on u.SysId equals un.User_SysId
                                    into pro
                           from uOrg in pro.DefaultIfEmpty()
                           join o in basisEntity.OrgNodes
                                   on uOrg.OrgNode_SysId equals o.SysId
                                   into orgNode
                           from org in orgNode.DefaultIfEmpty()
                           join PositionUsers in basisEntity.PositionUsers
                                    on u.SysId equals PositionUsers.User_SysId
                                    into pus
                           from pu in pus.DefaultIfEmpty()
                           join p in basisEntity.Positions
                                  on pu.Position_SysId equals p.SysId
                          // where (u.UserName == userName || p.Name==null) && p.Name.StartsWith("v_") == false
                           into Positions
                           from po in Positions.DefaultIfEmpty()

                           where u.UserName == userName
                           select new UserEntire
                           {
                               SysId = u.SysId,
                               UserName = u.UserName,
                               FirstName = u.FirstName,
                               UserId = u.UserId,
                               Type = org.Type,
                               Parent_SysId = org.Parent_SysId == null ? Guid.Empty : org.Parent_SysId,
                               Cluster_Type = org.Type,
                               Cluster_SysId = org.SysId == null ? Guid.Empty : org.SysId,
                               Cluster_Name = org.Name,
                               Position_Name = po.Name,
                               Position_SysId = po.SysId == null ? Guid.Empty : po.SysId,
                               
                           };
                linq = from r in linq
                       where r.Position_Name.StartsWith("v_") == false || r.Position_Name==null
                       select new UserEntire{
                    SysId = r.SysId,
                    UserName = r.UserName,
                    FirstName =r.FirstName,
                    UserId = r.UserId,
                    Type = r.Type,
                    Parent_SysId = r.Parent_SysId,
                    Cluster_Type = r.Cluster_Name,
                    Cluster_SysId = r.Cluster_SysId,
                    Cluster_Name = r.Cluster_Name,
                    Position_Name = r.Position_Name,
                    Position_SysId = r.Position_SysId
                
                };

                userList = linq.ToList();
                if (userList.Count > 0)
                {
                    //查询是科室
                    if (userList[0].Type == EnumCollection.OrgNodeType.Property.ToString() && userList[0].Parent_SysId != Guid.Empty)
                    {
                        var sysId = userList[0].Parent_SysId;
                        var OrgNodes = basisEntity.OrgNodes.Where(x => x.SysId == sysId).ToList();
                        var entity = OrgNodes.First();

                        if (OrgNodes.Count > 0 && entity.Type == EnumCollection.OrgNodeType.Cluster.ToString())
                        {  //科室
                            userList[0].Property_SysId = userList[0].Cluster_SysId;
                            userList[0].Property_Name = userList[0].Cluster_Name;
                            userList[0].Property_Type = userList[0].Cluster_Type;
                            //部门
                            userList[0].Cluster_SysId = entity.SysId;
                            userList[0].Cluster_Name = entity.Name;
                            userList[0].Cluster_Type = entity.Type;
                        }

                    }//查询是子公司
                    else if (userList[0].Type == EnumCollection.OrgNodeType.Division.ToString() && userList[0].Parent_SysId != Guid.Empty)
                    {
                        userList[0].Cluster_SysId = Guid.Empty;
                        userList[0].Cluster_Name = null;
                        userList[0].Cluster_Type = null;
                    }//查询是集团
                    else if (userList[0].Type == EnumCollection.OrgNodeType.Company.ToString())
                    {
                        userList[0].Cluster_SysId = Guid.Empty;
                        userList[0].Cluster_Name = null;
                        userList[0].Cluster_Type = null;
                    }

                }
            }
            if (userList == null || userList.Count <= 0)
            {
                return null;
            }
            return userList[0];
        }

        public string GetUserName(string sysID)
        {
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                System.Guid guid = new Guid(sysID);

                User us = basisEntity.Users.Where(x => x.SysId == guid).FirstOrDefault();
                if (us != null)
                {
                    return us.UserName;
                }
            }
            return null;
        }

        /// <summary>
        ///  获取职位对应职员的Username
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>
        /// Username 之间用“,” 分割的字符串
        /// </returns>
        public string GetPositionsUsername(string guid)
        {
            string userName = "";
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var linq = from pu in basisEntity.PositionUsers
                           join u in basisEntity.Users
                               on pu.User_SysId equals u.SysId
                               into pro
                           from po in pro.DefaultIfEmpty()
                           where pu.Position_SysId == new Guid(guid)
                           select new
                           {
                               UserName = po.UserName
                           };

                var userList = linq.ToList();

                if (userList.Count > 0)
                {
                    foreach (var item in userList)
                    {
                        if (string.IsNullOrEmpty(userName))
                        {
                            userName = item.UserName;
                        }
                        else
                        {
                            userName += "," + item.UserName;
                        }
                    } 
                }

            }
            return userName;
        }

        /// <summary>
        ///  获取部门对应职员的Username
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>
        /// Username 之间用“,” 分割的字符串
        /// </returns>
        public string GetOrgNodeUsername(string guid)
        {
            string userName = "";
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                //根据部门Guid 获取对应的userName
                var linq = from pu in basisEntity.UserOrgNodes
                           join u in basisEntity.Users
                               on pu.User_SysId equals u.SysId
                               into pro
                           from po in pro.DefaultIfEmpty()
                           where pu.OrgNode_SysId == new Guid(guid)
                           select new
                           {
                               UserName = po.UserName
                           };

                var userList = linq.ToList();

                if (userList.Count > 0)
                {
                    foreach (var item in userList)
                    {
                        if (string.IsNullOrEmpty(userName))
                        {
                            userName = item.UserName;
                        }
                        else
                        {
                            userName += "," + item.UserName;
                        }
                    }
                }

            }
            return userName;
        }

         /// <summary>
         /// 获取职位对应的简单用户信息 
         /// </summary>
         /// <param name="positionName"></param>
         /// <returns></returns>
        public static SimpleUser GetPositionsSimpleUser(string positionName)
        {
            SimpleUser simpleUser = new SimpleUser();
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                
                var linq = basisEntity.Positions
                    .Where(c => c.Name == positionName)
                    .Join(basisEntity.PositionUsers,
                          p => p.SysId,
                          pu => pu.Position_SysId,
                          (p, pu) =>
                          new { User_SysId = pu.User_SysId })
                    .Join(basisEntity.Users,
                          pu => pu.User_SysId,
                          u => u.SysId,
                          (pu, u) =>
                          new { Value = u.SysId, UserName = u.UserName, Name = u.FirstName, Types = EnumCollection.UserpickType.Users.ToString() });


                //var linq = from p in basisEntity.Positions
                //           where p.Name == positionName
                //           join pus in basisEntity.PositionUsers
                //                on p.SysId equals pus.Position_SysId
                //                  into Positions
                //           from pos in Positions.DefaultIfEmpty()
                //           join u in basisEntity.Users
                //                on pos.User_SysId equals u.SysId
                //                into pro
                //           from po in pro.DefaultIfEmpty()

                //           select new
                //           {
                //               UserName = po.UserName,
                //               Name = po.FirstName,
                //               Value = po.SysId,
                //               Types = EnumCollection.UserpickType.Users.ToString()
                //           };

                var userList = linq.ToList();

                if (userList.Count > 0)
                {
                    foreach (var item in userList)
                    {
                        if (string.IsNullOrEmpty(simpleUser.UserName))
                        {
                            simpleUser.UserName = item.UserName;
                            simpleUser.Types = item.Types;
                            simpleUser.Value = item.Value.ToString();
                            simpleUser.Name = item.Name;

                        }
                        else
                        {
                            simpleUser.UserName += "," + item.UserName;
                            simpleUser.Types += "," + item.Types;
                            simpleUser.Value += "," + item.Value.ToString();
                            simpleUser.Name += "," + item.Name;
                        }
                    }
                }
            }
            return simpleUser;
        } 

        /// <summary>
        /// 获取SimpleUser 中的userName
        /// </summary>
        /// <returns></returns>
        public static string GetSimpleUserUserName(SimpleUser simpleUser)
        {
            string userName = "";
            if (simpleUser == null) return userName;
            //fraser 2015-5-12新增 防止SimpleUser初始化后没有赋值
            if (simpleUser.Types == null || simpleUser.Value == null) return userName;
            var types = simpleUser.Types.Split(',');
            var sysIDs = simpleUser.Value.Split(',');
            simpleUser.UserName = (simpleUser.UserName == null ? "" : simpleUser.UserName);
            var userNames = simpleUser.UserName.Split(',');
             
            UserEntireUtilites entire = new UserEntireUtilites();

            for (int i = 0; i < types.Length; i++)
            { 
                if (types[i] == EnumCollection.UserpickType.Users.ToString())//用户
                {
                    userName += "," + userNames[i];
                }
                else if (types[i] == EnumCollection.UserpickType.Positions.ToString())//职位
                {
                    var positions = sysIDs[i];
                    var positionsUserName = entire.GetPositionsUsername(positions);
                    userName += string.IsNullOrWhiteSpace(positionsUserName) ? "" : ("," + positionsUserName);
                }
                else if (types[i] == EnumCollection.UserpickType.Depts.ToString())//部门
                {
                    var depts = sysIDs[i];
                    var deptsUserName = entire.GetOrgNodeUsername(depts);
                    userName += string.IsNullOrWhiteSpace(deptsUserName) ? "" : ("," + deptsUserName);
                }
            }
            if (userName.Length > 0)
            {
                userName = userName.Remove(0, 1); 
            }
            return userName;
        }
    }
}