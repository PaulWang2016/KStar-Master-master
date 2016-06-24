using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace aZaaS.KStar
{
    public class DataTabletoListConverter
    {
        public List<T> ToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合  
            List<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite) continue;//该属性不可写，直接跳出  
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value == DBNull.Value)
                            continue;

                        if (pi.PropertyType == typeof(string))
                        {
                            value = value.ToString();
                        }
                        else if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Guid?))
                        {
                            Guid guid;
                            if (Guid.TryParse(value.ToString(), out guid))
                                value = guid;
                            else
                                value = null;
                        }
                        else if (pi.PropertyType == typeof(bool))
                        {
                            value = bool.Parse(value.ToString());
                        }
                        else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(int?))
                        {
                            value = int.Parse(value.ToString());
                        }
                        else if (pi.PropertyType == typeof(DateTime?) || pi.PropertyType == typeof(DateTime))
                        {
                            value = DateTime.Parse(value.ToString());
                        }
                        else if (pi.PropertyType == typeof(float))
                        {
                            value = float.Parse(value.ToString());
                        }
                        else if (pi.PropertyType == typeof(double))
                        {
                            value = double.Parse(value.ToString());
                        }
                        else
                        {
                            value = null;
                        }
                        if (value != null)
                            pi.SetValue(t, value, null);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }

            return ts;
        }
    }
}
