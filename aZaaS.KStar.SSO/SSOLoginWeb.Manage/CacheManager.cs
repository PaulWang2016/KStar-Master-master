
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SSOLoginWeb.Manage
{
    /// <summary>
    /// 缓存管理
    /// 将用户凭证、令牌的关系数据存放于Cache中
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// 获取缓存中的DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCacheTable()
        {
            cacheInit();

            DataTable dt = null;
            if (HttpContext.Current.Cache["CERT"] != null)
            {
                dt = (DataTable)HttpContext.Current.Cache["CERT"];
            }
            return dt;
        }

        public static DataTable GetCacheTable1()
        {
            cacheInit();

            DataTable dt = null;
            if (HttpContext.Current.Cache["LOGIN"] != null)
            {
                dt = (DataTable)HttpContext.Current.Cache["LOGIN"];
            }
            return dt;
        }

        /// <summary>
        /// 初始化数据结构
        /// </summary>
        /// <remarks>
        /// ----------------------------------------------------------------------------------------------
        /// | ID | OA账号 | 对应系统编码 | 对应系统账号 |
        /// |---------------------------------------------------------------------------------------------
        /// </remarks>
        private static void cacheInit()
        {
            if (HttpContext.Current.Cache["CERT"] == null)
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("rowid", Type.GetType("System.String"));
                dt.Columns["rowid"].Unique = true;
                  
                dt.Columns.Add("userAccount", Type.GetType("System.String"));
                dt.Columns["userAccount"].DefaultValue = null;

                dt.Columns.Add("systemCode", Type.GetType("System.String"));
                dt.Columns["systemCode"].DefaultValue = null;

                dt.Columns.Add("otherAccount", Type.GetType("System.String"));
                dt.Columns["otherAccount"].DefaultValue = null;

                DataColumn[] keys = new DataColumn[1];
                keys[0] = dt.Columns["rowid"];
                dt.PrimaryKey = keys;

                //dt.Rows.Add(new object[] { "1", "smtwangl", "OA", "smtwangl" });
                //dt.Rows.Add(new object[] { "2", "smtwangl", "MY", "smtwangl" });
                //dt.Rows.Add(new object[] { "3", "laih", "OA", "laih" });
                //dt.Rows.Add(new object[] { "4", "laih", "MY", "laih" });
                //dt.Rows.Add(new object[] { "5", "fuyicheng", "OA", "fuyicheng" });
                //dt.Rows.Add(new object[] { "6", "fuyicheng", "MY", "fuyicheng" });
                //dt.Rows.Add(new object[] { "7", "liuy1", "OA", "liuy1" });
                //dt.Rows.Add(new object[] { "8", "liuy1", "MY", "liuy1" });
                //dt.Rows.Add(new object[] { "9", "zhangdk", "OA", "zhangdk" });
                //dt.Rows.Add(new object[] { "10", "zhangdk", "MY", "zhangdk" });
                //dt.Rows.Add(new object[] { "11", "zhaoxixi", "OA", "zhaoxixi" });
                //dt.Rows.Add(new object[] { "12", "zhaoxixi", "MY", "zhaoxixi" });
                //dt.Rows.Add(new object[] { "13", "zhoujp", "OA", "zhoujp" });
                //dt.Rows.Add(new object[] { "14", "zhoujp", "MY", "zhoujp" });

                //dt.Rows.Add(new object[] { "15", "admin", "OA", "admin" });

                //Cache永久有效
                HttpContext.Current.Cache.Insert("CERT", dt);
            }

            if (HttpContext.Current.Cache["LOGIN"] == null)
            {
                DataTable dt = new DataTable();
                  
                dt.Columns.Add("userAccount", Type.GetType("System.String"));
                dt.Columns["userAccount"].DefaultValue = null;


                dt.Columns.Add("tokenID", Type.GetType("System.String"));
                dt.Columns["tokenID"].DefaultValue = null;

                dt.Columns.Add("systemCode", Type.GetType("System.String"));
                dt.Columns["systemCode"].DefaultValue = null;

                dt.Columns.Add("LoginTime", Type.GetType("System.DateTime"));
                dt.Columns["LoginTime"].DefaultValue = null;

                DataColumn[] keys = new DataColumn[1];
                keys[0] = dt.Columns["userAccount"];
                dt.PrimaryKey = keys;

                //Cache
                HttpContext.Current.Cache.Insert("LOGIN", dt);
            }
        }
        
        /// <summary>
        /// 判断用户账号是否已经登录,并获取Tokenid
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        public static string UserAccountIsExist(string userAccount)
        {
            cacheInit();

            DataTable dt = (DataTable)HttpContext.Current.Cache["LOGIN"];
            DataRow[] dr = dt.Select("userAccount = '" + userAccount + "'");

            if (dr.Length == 0)
                return string.Empty;
            else
                return dr[0]["tokenID"].ToString();
        }

        /// <summary>
        /// 使用OA账号统一登录
        /// </summary>
        /// <param name="userAccount"></param>
        public static void CommonLogin(string systemCode, string userAccount, string tokenID)
        {
            cacheInit();

            if (string.IsNullOrEmpty(UserAccountIsExist(userAccount)))
            {
                DataTable dt = (DataTable)HttpContext.Current.Cache["LOGIN"];
                dt.Rows.Add(new object[] { userAccount, tokenID, systemCode, DateTime.Now });
            }
        }

        /// <summary>
        /// 使用OA账号统一注销
        /// </summary>
        /// <param name="userAccount"></param>
        public static bool CommonLoginOut(string userAccount)
        {
            cacheInit();
            if (!string.IsNullOrEmpty(userAccount))
            {
                DataTable dt = (DataTable)HttpContext.Current.Cache["LOGIN"];
                DataRow[] dr = dt.Select("userAccount = '" + userAccount + "'");
                if (dr.Length > 0)
                {
                    foreach (DataRow dri in dr)
                    {
                        dt.Rows.Remove(dri);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 其它系统登录
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userAccount"></param>
        public static string SystemLogin(string systemCode, string userAccount,string tokenID)
        {

            string _returnAccount = string.Empty;

            try
            {
                cacheInit();


                //DataTable dt = (DataTable)HttpContext.Current.Cache["CERT"];

                //DataRow[] dr = dt.Select("systemCode = '" + systemCode + "' and otherAccount='" + userAccount + "'");

                //if (dr.Length > 0)
                //{
                //    _returnAccount = dr[0]["userAccount"].ToString();
                    //CommonLogin(systemCode, dr[0]["userAccount"].ToString(), tokenID);

                    //直接添加到已登录用户表中
                    CommonLogin(systemCode, userAccount, tokenID);
                    WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 登录成功", userAccount, DateTime.Now.ToString(), systemCode));
                //}
                //else
                //{

                //    WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 登录,但获取数据异常！", userAccount, DateTime.Now.ToString(), systemCode));
                //}


            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 登录,但登录异常！{3}，{4}", userAccount, DateTime.Now.ToString(), systemCode, ex.Message, ex.StackTrace));
            }

            return _returnAccount;
        }


        /// <summary>
        /// 统一注销系统注销
        /// </summary>
        /// <param name="userAccount"></param>
        public static void SystemLoginOut(string userAccount)
        {
            try
            {
                 cacheInit(); 
                 CommonLoginOut(userAccount);
                 WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 注销成功", userAccount, DateTime.Now.ToString()));
               
            }

            catch (Exception ex)
            {
                WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 注销异常{2}，{3}", userAccount, DateTime.Now.ToString(), ex.Message, ex.StackTrace));
            }
        }


        /// <summary>
        /// 从其它系统上注销
        /// </summary>
        /// <param name="systemCode"></param>
        /// <param name="userAccount"></param>
        public static void OtherSystemLoginOut(string systemCode, string userAccount)
        {
            try
            {
                cacheInit();

                DataTable dt = (DataTable)HttpContext.Current.Cache["CERT"];

                DataRow[] dr = dt.Select("systemCode = '" + systemCode + "' and otherAccount='" + userAccount + "'");

                if (dr.Length > 0)
                {
                    CommonLoginOut(dr[0]["userAccount"].ToString());

                    WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 注销成功", userAccount, DateTime.Now.ToString(), systemCode));
                }

                WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 注销,但获取数据异常！", userAccount, DateTime.Now.ToString(), systemCode));
            }
            catch (Exception ex)
            {
                WriteLogHelper.WriteLogger(string.Format("{0} 于 {1} 从 {2} 注销,但注销异常！{3}，{4}", userAccount, DateTime.Now.ToString(), systemCode, ex.Message, ex.StackTrace));
            }
        }
    }
}