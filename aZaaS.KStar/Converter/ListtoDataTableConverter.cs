using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace aZaaS.KStar
{
    public class ListtoDataTableConverter
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                bool res = isContinue(prop);
                if (res)
                {
                    continue;
                }
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[dataTable.Columns.Count];
                int n = 0;
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    bool res = isContinue(Props[i]);
                    if (res)
                    {
                        continue;
                    }
                    values[n] = Props[i].GetValue(item, null);
                    n++;
                }

                dataTable.Rows.Add(values);

            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private static bool isContinue(PropertyInfo prop)
        {
            var isContinue = false;
            if (prop.PropertyType == typeof(string))
            {

            }
            else if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
            {

            }
            else if (prop.PropertyType == typeof(bool))
            {

            }
            else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
            {

            }
            else if (prop.PropertyType == typeof(DateTime?) || prop.PropertyType == typeof(DateTime))
            {

            }
            else if (prop.PropertyType == typeof(float))
            {

            }
            else if (prop.PropertyType == typeof(double))
            {

            }
            else
            {
                isContinue = true;
            }
            return isContinue;
        }
    }
}
