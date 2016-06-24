using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaas.NeowayOnly.DAL
{
    public class NW_ProjectDal
    {
        static object unique = new object();

        private static int PageSize = 10;

        /// <summary>
        /// 获取型号信息（排序方式暂时默认id,默认第一条为列标题条目） 
        /// </summary>
        /// <param name="pageIndex">获取地N页数据</param>
        /// <returns>返回查询到的第N页的数据的json 数组对象</returns>
        public static string GetProjectData(int pageIndex, string searchString = null)
        {
            #region sql
            string sql = @"select 0 as id,[ProjectName],[Model],[ProjectManager] from
                            (
                             SELECT
                                   c.name, 
                                   ex.value  
                             FROM  
                                 sys.columns c  
                             LEFT OUTER JOIN  
                                 sys.extended_properties ex  
                             ON  
                                ex.major_id = c.object_id 
                                AND ex.minor_id = c.column_id  
                                AND ex.name = 'MS_Description'  
                            WHERE  
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  
                                AND OBJECT_NAME(c.object_id) = 'NW_Project'--表名
                             ) as  s pivot
                             ( max(s.value)
                                  for s.name in(ProjectName,Model,ProjectManager)
                             ) as columnsName
                             
                             select top {0} * from (select  ROW_NUMBER() over(order by [id] desc) as [id],[ProjectName],[Model],[ProjectManager] from NW_Project "
                             + (searchString == null ? "" : string.Format(" where [ProjectName] LIKE  N'%{0}%' or [Model] LIKE  N'%{0}%' or [ProjectManager] LIKE  N'%{0}%' ", searchString))
                             + @" ) as NW_Project  
                             where id>{0}*{1} and id<={0}*({1}+1)";
            #endregion
            sql = string.Format(sql, PageSize, pageIndex);

            DataTableCollection dts = SqlHelper.ExecuteTables(sql);

            string jsonDataBody = Newtonsoft.Json.JsonConvert.SerializeObject(dts[1], Newtonsoft.Json.Formatting.None);
            string jsonDatahead = Newtonsoft.Json.JsonConvert.SerializeObject(dts[0], Newtonsoft.Json.Formatting.None);
            string jsonData = jsonDatahead.Substring(0, jsonDatahead.Length - 1) + "," + jsonDataBody.Substring(1);
            return jsonData;
        }


      
        public static string GetUserData(int pageIndex, string searchString = null)
        {
            #region sql
            string sql = @"select 0 as id,[UserId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark],[CreateDate]   from
                            (
                             SELECT
                                   c.name, 
                                   ex.value  
                             FROM  
                                 sys.columns c  
                             LEFT OUTER JOIN  
                                 sys.extended_properties ex  
                             ON  
                                ex.major_id = c.object_id 
                                AND ex.minor_id = c.column_id  
                                AND ex.name = 'MS_Description'  
                            WHERE  
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  
                                AND OBJECT_NAME(c.object_id) = 'User'--表名
                             ) as  s pivot
                             ( max(s.value)
                                  for s.name in([UserId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark],[CreateDate])
                             ) as columnsName
                              
                            select top 10 * from (select  ROW_NUMBER() over(order by CreateDate desc) as [id],[UserId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark],[CreateDate] from [User] "
                             + (searchString == null ? "" : string.Format(" where [UserId] LIKE N'%{0}%' or [UserName] LIKE  N'%{0}%' or [FirstName] LIKE  N'%{0}%' or [LastName] LIKE  N'%{0}%' or [CreateDate] LIKE  N'%{0}%' or [Phone] LIKE  N'%{0}%' or [Email] LIKE  N'%{0}%' or [Address] LIKE  N'%{0}%' ", searchString))
                             + @" ) as NW_Project  
                             where id>{0}*{1} and id<={0}*({1}+1)";
            #endregion
        
            sql = string.Format(sql, PageSize, pageIndex);

            DataTableCollection dts = SqlHelper.ExecuteTables(sql);

            string jsonDataBody = Newtonsoft.Json.JsonConvert.SerializeObject(dts[1], Newtonsoft.Json.Formatting.None);
            string jsonDatahead = Newtonsoft.Json.JsonConvert.SerializeObject(dts[0], Newtonsoft.Json.Formatting.None);
            string jsonData = jsonDatahead.Substring(0, jsonDatahead.Length - 1) + "," + jsonDataBody.Substring(1);
            return jsonData;
        }

    }
}
