using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace aZaaS.Kstar.DAL
{
    public class FiltrateEntity
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { set; get; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 左边
        /// </summary>
        public bool Left { set; get; }
        /// <summary>
        /// 右边
        /// </summary>
        public bool Right { set; get; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Operate { set; get; }

        public bool AndJoin { set; get; }

    }

    public class FiltrateOperate
    {
        //转换成对应where sql
        public static string ToWhereString(FiltrateEntity[] entitys, out  List<SqlParameter> parameters)
        {
            string str = "";
            parameters = new List<SqlParameter>();
            if (entitys == null) return "";
            int i = 1;
            foreach (var entity in entitys)
            {
                if (string.IsNullOrEmpty(entity.Operate) || string.IsNullOrEmpty(entity.Field))
                    throw new Exception("参数错误！");
                str += " " + (entity.AndJoin ? "and" : "or") + " ";
                if (entity.Left == true)
                {
                    str += "( ";
                }
                str += " " + entity.Field;
                entity.Operate = entity.Operate.Trim().ToLower();
                switch (entity.Operate)
                {
                    case "=":
                        str += " =";
                        break;
                    case ">=":
                        str += " >=";
                        break;
                    case "<=":
                        str += " <=";
                        break;
                    case "<":
                        str += " <";
                        break;
                    case ">":
                        str += " >";
                        break;
                    case "<>":
                        str += " <>";
                        break;
                    case "like":
                        str += " like ";
                        break;
                    default:
                        throw new Exception("操作参数错误！");
                }

                str += "@" + entity.Field+i.ToString();

                if (entity.Right == true)
                {
                    str += ")";
                }

                SqlParameter p = new SqlParameter();
                p.SqlDbType = System.Data.SqlDbType.NVarChar;
                p.ParameterName = "@" + entity.Field + i.ToString();
                p.Value = entity.Value;
                parameters.Add(p);
                i++;
            }
            return str;
        }

        /// <summary>
        /// 转换成json 实体
        /// </summary>
        /// <returns></returns>
        public static FiltrateEntity[] JsonToEntity(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FiltrateEntity[]>(json);
        }
    }


}