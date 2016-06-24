using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;


namespace aZaaS.Kstar.DAL
{
    public class OrgUtility
    {
        /// <summary>
        /// 部门级别
        /// </summary>
        public static string strCluster = "Cluster";
        /// <summary>
        /// 科室级别
        /// </summary>
        public static string strProperty = "Property";
        /// <summary>
        /// 公司级别
        /// </summary>
        public static string strDivision = "Division";
        /// <summary>
        /// 集团级别
        /// </summary>
        public static string strCompany = "Company";
        //public static string GetUserLevel(string UserID)
        //{
 
        //}
        //public static string UserDepartmentLevel(string UserID)
        //{ }
        /// <summary>
        /// 根据用户和级别取相对应的部门级别信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public static DataTable GetUserDepartmentLevel(string UserID, string Level)
        {
            SqlParameter[] paras = { new SqlParameter("@UserID", UserID),
                                     new SqlParameter("@type",Level)
                                   };
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Organazation_GetUpUserAllInfo", paras).Tables[0];
            return table;
        }
        /// <summary>
        /// 取得用户的直接部门领导
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static DataTable GetUserHead(string UserID)
        {
            SqlParameter[] paras = { new SqlParameter("@UserID", UserID) };
            var dataTable = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Organization_GetUserHead", paras).Tables[0];
            return dataTable;
        }
        /// <summary>
        /// 取得用户的上级一部门
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static DataTable GetUserUpOrgNode(string UserID)
        {
            SqlParameter[] paras = { new SqlParameter("@UserID", UserID) };
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Organization_GetUpOrgNode", paras).Tables[0];
            return table;
        }
        /// <summary>
        /// 根据当前的用户ID,取得当前用户的基本信息,
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfo(string UserID)
        {
            UserInfo userInfo = null;
            SqlParameter[] paras = { new SqlParameter("@UserID", UserID) };
            var ds = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_sp_Organazation_GetUserAllInfo", paras);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
               userInfo=new UserInfo();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    userInfo.Email = r["Email"].ToString();
                    userInfo.Postion = r["posName"].ToString();
                    userInfo.RealName = r["FirstName"].ToString();
                    userInfo.Sex = r["Sex"].ToString();
                    userInfo.SysID = r["SysID"].ToString();
                    userInfo.UserID = r["UserID"].ToString();
                    userInfo.UserName = r["UserName"].ToString();
                    userInfo.UserLevel = r["Level"].ToString();
                    userInfo.UserTotalServiceYears = r["TotalServiceYear"].ToString();
                    var model = new Department();
                    model.OrgName = r["OrgName"].ToString();
                    model.OrgType = r["Type"].ToString();
                    model.SysId = r["OrgSysID"].ToString();
                    model.HeadUserID=r["HeadUserID"].ToString();
                    model.HeadRealName = r["HeadUserName"].ToString();
                    model.IsLeader = r["IsLeader"].ToString().ToLower() == "yes" ? true : false;
                    userInfo.UserDepart = model;
                    var upModel = new Department();
                    upModel.OrgName = r["UpName"].ToString();

                    upModel.OrgType = r["UpType"].ToString();
                    upModel.SysId = r["UpSysId"].ToString();
                    userInfo.UserUpDepart = upModel;
                }
            }
            return userInfo;
        }
        /// <summary>
        /// 根据部门ID取得当前部门的领导信息
        /// </summary>
        /// <param name="SysID">部门ID</param>
        /// <returns>用户基本信息集合</returns>
        public static DataTable GetDepartHeadByOrgID(string SysID)
        {
            SqlParameter[] paras = { new SqlParameter("@SysID", SysID) };
            var ds = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Organazation_GetDeptHeadByDeptID", paras).Tables[0];
            return ds;
        }
        /// <summary>
        /// 获取当前用户（如果是部门领导）下的所有用户
        /// 如果不是部门领导则返回当前用户的信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static DataTable GetAllChildUser(string UserID)
        {
            SqlParameter[] paras = { new SqlParameter("@UserID", UserID) };
            var ds = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Organization_GetAllChildUser_ByUserID", paras).Tables[0];
            return ds;
        }
        /// <summary>
        /// 根据部门ID获取部门信息
        /// </summary>
        /// <param name="SysID">部门ID</param>
        /// <returns></returns>
        public static DataTable GetDepartByOrgID(string SysID)
        {
            string strSql = "SELECT * FROM OrgNode where SysId=@SysID ";
            SqlParameter[] paras = { new SqlParameter("@SysID", SysID) };
            var ds = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, paras).Tables[0];
            return ds;
        }

        /// <summary>
        /// 根据UserID,Email获取User表信息
        /// </summary>
        /// <param name="SysID">UserID</param>
        /// <returns></returns>
        public static DataTable GetUser(string UserId, string Email)
        {
            string strSql = "SELECT * FROM [User] where ";
            DataTable ds = new DataTable();
            if(!string.IsNullOrWhiteSpace(UserId))
            {
                strSql += " UserId=@UserId ";
                SqlParameter[] paras = { new SqlParameter("@UserId", UserId) };
                ds = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, paras).Tables[0];
            }
            else if (!string.IsNullOrWhiteSpace(Email))
            {
                strSql += " Email=@Email ";
                SqlParameter[] paras = { new SqlParameter("@Email", Email) };
                ds = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, paras).Tables[0];
            }
            return ds;
        }


        /// <summary>
        /// 根据UserID,Email获取User表信息
        /// </summary>
        /// <param name="SysID">UserID</param>
        /// <returns></returns>
        public static DataTable GetEmployeeInfoByEmpNo(string EmpNo)
        {
            string strSql = "SELECT * FROM ehr_EmployeeInfo where  EmpNo=@EmpNo";
            SqlParameter[] paras = { new SqlParameter("@EmpNo", EmpNo) };
            DataTable ds = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, paras).Tables[0];

            return ds;
        }

        /// <summary>
        /// 入职流程结束--插入人员组织架构 User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertUserInfo(string Name, string Sex, string Email, string UserId)
        {
            //[SysId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark],[UserId],[CreateDate]
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [User] ");
            strSql.Append("(SysId,UserName,FirstName,LastName,Sex,Email,Address,Phone,Status,Remark,UserId,CreateDate");
            strSql.Append(") values (");
            strSql.Append("@SysId,@UserName,@FirstName,@LastName,@Sex,@Email,@Address,@Phone,@Status,@Remark,@UserId,@CreateDate");
            strSql.Append(") ");
            SqlParameter[] parameters = {
                        new SqlParameter("@SysId", SqlDbType.UniqueIdentifier) ,   
                        new SqlParameter("@UserName", SqlDbType.NVarChar, 100) ,   
                        new SqlParameter("@FirstName", SqlDbType.NVarChar,100) ,   
                        new SqlParameter("@LastName", SqlDbType.NVarChar,10) ,    
                        new SqlParameter("@Sex", SqlDbType.NVarChar,10) ,  
                        new SqlParameter("@Email", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@Address", SqlDbType.NVarChar,200) ,  
                        new SqlParameter("@Phone", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@Status", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@Remark", SqlDbType.NVarChar,1000) ,  
                        new SqlParameter("@UserId", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@CreateDate", SqlDbType.DateTime) 
            };
            parameters[0].Value = Guid.NewGuid();
            parameters[1].Value = @"neowaydc\" + UserId;
            parameters[2].Value = Name;
            parameters[3].Value = "";
            parameters[4].Value = Sex;
            parameters[5].Value = Email;
            parameters[6].Value = "";
            parameters[7].Value = "";
            parameters[8].Value = "True";
            parameters[9].Value = "";
            parameters[10].Value = UserId;
            parameters[11].Value = DateTime.Now;

            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
        }

        /// <summary>
        /// 入职流程结束--插入数据 部门与用户的关联表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertUserOrgNodes(string User_SysId, string OrgNode_SysId)
        {
            //[User_SysId],[OrgNode_SysId]
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [UserOrgNodes] ");
            strSql.Append("(User_SysId,OrgNode_SysId");
            strSql.Append(") values (");
            strSql.Append("@User_SysId,@OrgNode_SysId");
            strSql.Append(") ");
            SqlParameter[] parameters = {
                        new SqlParameter("@User_SysId", SqlDbType.UniqueIdentifier) ,
                        new SqlParameter("@OrgNode_SysId", SqlDbType.UniqueIdentifier) 
            };
            parameters[0].Value = new Guid(User_SysId);
            parameters[1].Value = new Guid(OrgNode_SysId);

            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
        }

        /// <summary>
        /// 入职流程结束--插入数据 职位与用户的关联表
        /// </summary>
        /// <param name="Position_SysId"></param>
        /// <param name="User_SysId"></param>
        /// <returns></returns>
        public static int InsertPositionUsers(string Position_SysId, string User_SysId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [PositionUsers] ");
            strSql.Append("(Position_SysId,User_SysId");
            strSql.Append(") values (");
            strSql.Append("@Position_SysId,@User_SysId");
            strSql.Append(") ");
            SqlParameter[] parameters = {
                        new SqlParameter("@Position_SysId", SqlDbType.UniqueIdentifier) ,
                        new SqlParameter("@User_SysId", SqlDbType.UniqueIdentifier) 
            };
            parameters[0].Value = new Guid(Position_SysId);
            parameters[1].Value = new Guid(User_SysId);

            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
        }

        /// <summary>
        /// 入职流程结束--插入数据到花名册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertEmpInfo(EmployeeInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ehr_EmployeeInfo ");
            strSql.Append("(EmpNo,EmpName,Sex,IsEntryAgain,");
            strSql.Append("Education,GraduateSchool,GraduateDate,CollegeLevel,Professional,");
            strSql.Append("BelongType,Department,Section,Jobs,");
            strSql.Append("IsRelatives,RelativeName,RelativeDept,RelativeSection,Relationship,Position,InDate");
            strSql.Append(") values (");
            strSql.Append("@EmpNo,@EmpName,@Sex,@IsEntryAgain,");
            strSql.Append("@Education,@GraduateSchool,@GraduateDate,@CollegeLevel,@Professional,");
            strSql.Append("@BelongType,@Department,@Section,@Jobs,");
            strSql.Append("@IsRelatives,@RelativeName,@RelativeDept,@RelativeSection,@Relationship,@Position,@InDate");
            strSql.Append(") ");
            SqlParameter[] parameters = {
                        new SqlParameter("@EmpNo", SqlDbType.NVarChar,50) ,   
                        new SqlParameter("@EmpName", SqlDbType.NVarChar,50) ,   
                        new SqlParameter("@Sex", SqlDbType.NVarChar,10) ,   
                        new SqlParameter("@IsEntryAgain", SqlDbType.NVarChar,10) ,    
                        new SqlParameter("@Education", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@GraduateSchool", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@GraduateDate", SqlDbType.DateTime) ,   
                        new SqlParameter("@CollegeLevel", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@Professional", SqlDbType.NVarChar,50) , 

                        new SqlParameter("@BelongType", SqlDbType.NVarChar,50) ,
                        new SqlParameter("@Department", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@Section", SqlDbType.NVarChar,100) ,  
                        new SqlParameter("@Jobs", SqlDbType.NVarChar,100) ,  

                        new SqlParameter("@IsRelatives", SqlDbType.NVarChar,10) ,  
                        new SqlParameter("@RelativeName", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@RelativeDept", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@RelativeSection", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@Relationship", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@Position", SqlDbType.NVarChar,50) ,  
                        new SqlParameter("@InDate", SqlDbType.DateTime) 
            };
            parameters[0].Value = model.EmpNo;
            parameters[1].Value = model.EmpName;
            parameters[2].Value = model.Sex;
            parameters[3].Value = model.IsEntryAgain;
            parameters[4].Value = GetFacValue(model.Education, "Education");
            parameters[5].Value = model.GraduateSchool;
            parameters[6].Value = model.GraduateDate;
            parameters[7].Value = GetFacValue(model.CollegeLevel, "CollegesProperty");
            parameters[8].Value = model.Professional;

            parameters[9].Value = GetFacValue(model.BelongType, "Category");
            parameters[10].Value = model.Department;
            parameters[11].Value = model.Section;
            parameters[12].Value = model.Jobs;

            parameters[13].Value = model.IsRelatives;
            parameters[14].Value = model.RelativeName;
            parameters[15].Value = model.RelativeDept;
            parameters[16].Value = model.RelativeSection;
            parameters[17].Value = model.Relationship;
            parameters[18].Value = GetFacValue(model.Position, "EmployeeLevel");
            parameters[19].Value = model.InDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
        }
        
        /// <summary>
        /// 入职流程结束，触发转正流程前的数据处理
        /// </summary>
        /// <param name="EmployeeInfo"></param>
        /// <param name="EntryNo"></param>
        /// <param name="Name"></param>
        /// <param name="Depart"></param>
        /// <param name="Email"></param>
        /// <param name="HeadName">上级领导</param>
        /// <param name="Sex"></param>
        /// <param name="PositionId">职位ID</param>
        /// <returns></returns>
        public static bool InsertEmployeeData(EmployeeInfo EmployeeInfo, string EntryNo, string Name, string DepartId, string Email, string Sex, string PositionId)
        {
            bool bol = false;
            if (InsertEmpInfo(EmployeeInfo) > 0 && InsertUserInfo(Name, Sex, Email, EntryNo) > 0)
            {
                DataTable Userdt = GetUser(EntryNo, "");
                if (Userdt.Rows.Count > 0)
                {
                    string User_SysId = Userdt.Rows[0]["SysId"].ToString();
                    if (InsertUserOrgNodes(User_SysId, DepartId) > 0 && InsertPositionUsers(PositionId, User_SysId) > 0)
                    {
                        DataTable Role = GetRoleUsersInfo("Everyone");
                        if (Role.Rows.Count > 0)
                        {
                            if (InsertRoleUsers(Role.Rows[0]["SysId"].ToString(), User_SysId) > 0)
                            {
                                bol = true;
                            }
                        }
                    }
                }
            }
            return bol;
        }
        /// <summary>
        /// 获取员工花名册信息
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public static DataTable GetEmpInfo(string empNo)
        {
            var sql = @"select * from ehr_EmployeeInfo where EmpNo=@EmpNo";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpNo",empNo)
            };

            var dt = SqlHelper.ExecuteTable(sql, parameters);

            return dt;
        }

        /// <summary>
        /// 入职流程结束--插入数据 是领导 且该单位没有领导 就插入数据
        /// </summary>
        /// <param name="Position_SysId"></param>
        /// <param name="User_SysId"></param>
        /// <returns></returns>
        public static void InsertOrgNodeExtends(string DepartId, string UserId)
        {
            if (!string.IsNullOrWhiteSpace(DepartId))
            {
                if (DepartIsHaveLeader(DepartId) == 0)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into [OrgNodeExtends] ");
                    strSql.Append("(SysId,Name,Value");
                    strSql.Append(") values (");
                    strSql.Append("@SysId,@Name,@Value");
                    strSql.Append(") ");
                    SqlParameter[] parameters = {
                        new SqlParameter("@SysId", SqlDbType.UniqueIdentifier) ,
                        new SqlParameter("@Name", SqlDbType.NVarChar,128) ,   
                        new SqlParameter("@Value", SqlDbType.NVarChar,100) 
                        };
                    parameters[0].Value = new Guid(DepartId);
                    parameters[1].Value = "DeptOwner";
                    parameters[2].Value = UserId;
                    SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
                }
            }

        }

        /// <summary>
        /// 判断 单位 是否有领导
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public static int DepartIsHaveLeader(string DeptsID)
        {
            var sql = @"select * from  [OrgNodeExtends]  where SysId=@SysId and Name='DeptOwner' ";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@SysId",DeptsID)
            };

            var dt = SqlHelper.ExecuteTable(sql, parameters);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 插入数据到角色表
        /// </summary>
        /// <param name="Role_SysId"></param>
        /// <param name="User_SysId"></param>
        /// <returns></returns>
        public static int InsertRoleUsers(string Role_SysId, string User_SysId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [RoleUsers] ");
            strSql.Append("(Role_SysId,User_SysId");
            strSql.Append(") values (");
            strSql.Append("@Role_SysId,@User_SysId");
            strSql.Append(") ");
            SqlParameter[] parameters = {
                        new SqlParameter("@Role_SysId", SqlDbType.UniqueIdentifier) ,
                        new SqlParameter("@User_SysId", SqlDbType.UniqueIdentifier) 
            };
            parameters[0].Value = new Guid(Role_SysId);
            parameters[1].Value = new Guid(User_SysId);

            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql.ToString(), parameters);
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static DataTable GetRoleUsersInfo(string Name)
        {
            var sql = @"select * from Role where Name=@Name";
            var parameters = new SqlParameter[]{
                    new SqlParameter("@Name",Name)
               };
            DataTable dt = SqlHelper.ExecuteTable(sql, parameters);
            return dt;
        }

        /// <summary>
        /// 通过字典code获取字典名字 获取字典数据
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="DictionaryCode"></param>
        /// <returns></returns>
        public static string GetFacValue(string Code, string DictionaryCode)
        {
            string ValueName = "";
            if (!string.IsNullOrWhiteSpace(Code))
            {
                aZaaS.KStar.Facades.DataDictionaryFacade facde = new aZaaS.KStar.Facades.DataDictionaryFacade();

                var item = facde.GetDataDictionaryByCode(DictionaryCode).ToList().FirstOrDefault(r => r.Code == Code);
                if (item != null)
                {
                    ValueName = item.Name;
                }
            }
            if (string.IsNullOrWhiteSpace(ValueName))
                ValueName = Code;
            return ValueName;
        }

        /// <summary>
        /// 获取数据库 数据-- 找事务表数据
        /// </summary>
        /// <returns></returns>
        public static System.Data.DataTable GetTableData(string tableName, string where)
        {
            string strSql = "SELECT * FROM " + tableName;
            if (where.Trim() != "")
            {
                strSql += " where " + where;
            }
            var table = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, null).Tables[0];
            return table;
        }
    }
    public class UserInfo 
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserID { get;set;}
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName{get;set;}
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string SysID { get; set; }
        /// <summary>
        /// 员用职位
        /// </summary>
        public string Postion { get; set; }
        /// <summary>
        /// 员工级别
        /// </summary>
        public string UserLevel { get; set; }
        /// <summary>
        /// 员工工龄
        /// </summary>
        public string UserTotalServiceYears { get; set; }
        /// <summary>
        /// 员工性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 员工所在部门
        /// </summary>
        public Department UserDepart { get; set; }
        /// <summary>
        /// 员工的上一级部门
        /// </summary>
        public Department UserUpDepart { get; set; }

    }
    public class Department
    {
        /// <summary>
        /// 部门编号
        /// </summary>
        public string SysId { get; set; }
        /// <summary>
        /// 部门姓名
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// 部门类别
        /// </summary>
        public string OrgType { get; set; }
        /// <summary>
        /// 部门领导ID
        /// </summary>
        public string HeadUserID { get; set; }
        /// <summary>
        /// 部门领导姓名
        /// </summary>
        public string HeadRealName { get; set; }
        /// <summary>
        /// 是否是部门领导
        /// </summary>
        public bool IsLeader { get; set; }
    }
    
}
