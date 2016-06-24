using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaas.NeowayOnly.DAL
{
 public class NeoWay_DataTable_Dal
    {
        public static List<string> GetTables()
        {
            string sql = @"SELECT Name FROM  SysObjects Where XType='U' ORDER BY Name";
            DataTable dt = SqlHelper.ExecuteTable(sql);
            List<string> DataList = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                DataList.Add((dr[0] + string.Empty));
            }
            return DataList;
        }

        public static Dictionary<string, string> GetColumns(string tableName)
        {
            string sql = @"select syscolumns.Name,systypes.name as TypeName    from(
                            SELECT  Name,xtype FROM SysColumns  
                                               WHERE id=Object_Id('{0}') and  
						                             columnproperty(SysColumns.id,SysColumns.name,'IsIdentity')!=1
				                    ) as syscolumns left join  systypes on systypes.xtype=SysColumns.xtype  and status =0";
            sql = string.Format(sql, tableName);
        
            DataTable dt = SqlHelper.ExecuteTable(sql);
            Dictionary<string, string> DataList = new Dictionary<string, string>(); 
            foreach (DataRow dr in dt.Rows)
            {
                DataList.Add((dr[0] + string.Empty),dr[1]+string.Empty);
            }
            return DataList;
        }
    }
}
