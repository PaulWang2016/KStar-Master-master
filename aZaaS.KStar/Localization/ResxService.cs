using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Globalization;
using System.Resources;
using System.Collections;
using System.Text;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.Localization
{
    public static class ResxService
    {
        private static readonly string _defaultCultureName = "en-US";


        public static String AddResoucesItem(string resxFilePath, string name, string value)
        {

            return "";

        }

        public static string LangAdaptor(string lang)
        {

            if (lang.StartsWith("zh"))
            {
                if (lang.EndsWith("TW"))
                {
                    lang = "zh-TW";
                }
                else
                {
                    lang = "zh-CN";
                }
            }
            else if (lang.StartsWith("en"))
            {
                lang = "en-US";
            }
            else
            {

                lang = "en-US";
            }
            return lang;

        }

        public static string GetAvailableCulture()
        {

            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (String.IsNullOrEmpty(ci.Name))
            {
                return ci.Parent.Name;
            }
            else
            {
                return ci.Name;

            }
            //  return "zh-CN"; 
        }


        /// <summary>
        ///  没有相关的参数，该做什么？
        /// </summary>
        /// <returns></returns>
        public static string GetJSResxFilePath(String jsFilePath)
        {
            jsFilePath = jsFilePath.Replace("_", "/");

            var mapPath = AppDomain.CurrentDomain.BaseDirectory;
            var cultureName = GetAvailableCulture();
            string jsResxRoot = Path.Combine(mapPath, "Resx\\JS");
            string jsResxFilePath = string.Empty;

            jsResxFilePath = Path.Combine(jsResxRoot, jsFilePath + "_js." + cultureName + ".resx");
            FileInfo fi_parentCulture = new FileInfo(jsResxFilePath);
            if (!fi_parentCulture.Exists)
            {
                jsResxFilePath = Path.Combine(jsResxRoot, jsFilePath + "_js." + _defaultCultureName + ".resx");
            }

            return jsResxFilePath;

        }


        /// <summary>
        /// 获取指定路径下的资源文件里指定Key的值
        /// </summary>
        /// <param name="resxKey">指定值</param>
        /// <param name="resxFilePath">指定路径</param>
        /// <returns></returns>
        public static String GetResouces(String resxKey, String resxFilePath)
        {
            Dictionary<string,string> resxdic=LoadResxFile(resxFilePath);
            if(resxdic.ContainsKey(resxKey))
            {
                return resxdic[resxKey];
            }
            return resxKey;
        }

        public static string GetResxFilePathByDbInfo(String dbInfo)
        {

            var mapPath = AppDomain.CurrentDomain.BaseDirectory;
            string csResxRoot = Path.Combine(mapPath, "Resx\\Database");

            string csResxFilePath = string.Empty;
            var cultureName = GetAvailableCulture();
            csResxFilePath = Path.Combine(csResxRoot, dbInfo + "." + cultureName + ".resx");

            //FileInfo fi = new FileInfo(csResxFilePath);

            FileInfo fi_parentCulture = new FileInfo(csResxFilePath);
            if (!fi_parentCulture.Exists)
            {
                csResxFilePath = Path.Combine(csResxRoot, dbInfo + "." + _defaultCultureName + ".resx");
            }
            return csResxFilePath;

        }


        /// <summary>
        /// 调用时只需要传入 HtmlHelper 获取当前页面所需要的语言资源文件
        /// </summary>
        /// <param name="htmlHelper">通过HtmlHelper 来获取到对应的资源文件的路径</param>
        /// <returns></returns>
        public static string GetResxFilePathByHtmlhelper(HtmlHelper htmlHelper)
        {
            WebViewPage webViewPage = (htmlHelper.ViewDataContainer as WebViewPage);



            var virtualPath = webViewPage.VirtualPath;
            var mapPath = webViewPage.Server.MapPath("~");
            string cshtmlResxRoot = Path.Combine(mapPath, "Resx");

            string cshtmlResxFilePath = string.Empty;

            virtualPath = virtualPath.TrimStart(@"~/".ToArray());
            virtualPath = virtualPath.Replace(".", "_");

            var cultureName = GetAvailableCulture();

            cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, virtualPath + "." + cultureName + ".resx");

            FileInfo fi = new FileInfo(cshtmlResxFilePath);


            FileInfo fi_parentCulture = new FileInfo(cshtmlResxFilePath);
            if (!fi_parentCulture.Exists)
            {
                cshtmlResxFilePath = Path.Combine(cshtmlResxRoot, virtualPath + "." + _defaultCultureName + ".resx");
            }

            return cshtmlResxFilePath;
        }

        public static string GetCommonResx(String fileName)
        {

            var mapPath = AppDomain.CurrentDomain.BaseDirectory;
            string commonResxPath = Path.Combine(mapPath, "Resx\\Common");
            string commonResxFilePath = string.Empty;
            var cultureName = GetAvailableCulture();
            commonResxFilePath = Path.Combine(commonResxPath, fileName + "." + cultureName + ".resx");

            FileInfo fi_parentCulture = new FileInfo(commonResxFilePath);
            if (!fi_parentCulture.Exists)
            {
                commonResxFilePath = Path.Combine(commonResxPath, fileName + "." + _defaultCultureName + ".resx");
            }
            return commonResxFilePath;

        }

        public static string GetResxFilePathByNamespace(String nameSpace)
        {
            var mapPath = AppDomain.CurrentDomain.BaseDirectory;
            string csResxRoot = Path.Combine(mapPath, "Resx\\Namespace");

            string csResxFilePath = string.Empty;


            var cultureName = GetAvailableCulture();

            csResxFilePath = Path.Combine(csResxRoot, nameSpace + "." + cultureName + ".resx");

            FileInfo fi_parentCulture = new FileInfo(csResxFilePath);
            if (!fi_parentCulture.Exists)
            {
                csResxFilePath = Path.Combine(csResxFilePath, nameSpace + "." + _defaultCultureName + ".resx");
            }

            return csResxFilePath;

        }


        /// <summary>
        /// 读取缓存中指定key的缓存值，如果不存在的话，则从读取资源文件读取，然后将读取结果插入到缓存中，提高资源文件的加载效率
        /// 返回 Dictionary<string, string>
        /// </summary>
        /// <param name="resxFilePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadResxFile(string resxFilePath)
        {
            Dictionary<string, string> dict = CacheResx.Get(resxFilePath);
            if (dict == null)
            {
                dict = new Dictionary<string, string>();

                using (ResXResourceReader reader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry item in reader)
                    {
                        if (item.Value == null || item.Value.ToString().Trim().Length == 0)
                        {
                            dict.Add(item.Key.ToString(), item.Key.ToString());
                        }
                        else
                        {
                            dict.Add(item.Key.ToString(), item.Value.ToString());
                        }
                    }
                }
                CacheResx.Add(resxFilePath, dict);
            }
            else
            {
                return dict;
            }
            return dict;
        }


        /// <summary>
        /// 从数据库中提取指定数据库 指定表 指定列的 对应的资源文件
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> LoadResxFromDB(string dbName, string tableName, string columnName)
        {
            var culturn = GetAvailableCulture();
            var localizationResourceTable = "LocalizationResource" + "_" + culturn.ToUpper().Replace("-", "");
            var sql = String.Format(@"select  ID, DataBaseName,TableName ,ColumnName , ResxKey,ResxValue 
                                        from {0}
                                        where DataBaseName='{1}'
                                         and   TableName='{2}'
                                         and  ColumnName='{3}'    ",
                localizationResourceTable, dbName, tableName, columnName);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            KStarDbContext dbContext = new KStarDbContext();

            var resxList = dbContext.Database.SqlQuery<LocalizationResourceEntity>(sql);
            foreach (var resx in resxList)
            {
                dic.Add(resx.ResxKey, resx.ResxValue);
            }
            return dic;

        }

        public static string GetGenerateJavaScript(String jsFilePath)
        {
            var jsObjName = "jsResx" + jsFilePath;

            jsFilePath = GetJSResxFilePath(jsFilePath);
            Dictionary<string, string> dict = ResxService.LoadResxFile(jsFilePath);
            StringBuilder builder = new StringBuilder();
            builder.Append("var   " + jsObjName + " = {");
            foreach (var item in dict)
            {
                builder.AppendFormat("{0}:'{1}',", item.Key, item.Value.Replace("'", "\\'"));
            }
            var s = builder.ToString();
            builder.Remove(builder.Length - 1, 1);
            s = builder.ToString();
            builder.Append("}");
            return builder.ToString();


        }

        /// <summary>
        /// 更新数据库中提取指定数据库 指定表 指定列的 对应的资源文件
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="resxKey"></param>
        /// <param name="resxValues"></param>
        public static void UpdateResxFromDB(string dbName, string tableName, string columnName, string resxKey, Dictionary<string, string> resxValues)
        {
            var culturns = LocalizationResxExtend.GetPortalLanguage().Keys;
            foreach (var culturn in culturns)
            {
                var localizationResourceTable = "LocalizationResource" + "_" + culturn.ToUpper().Replace("-", "");
                var sql = String.Format(@"select  ID, DataBaseName,TableName ,ColumnName , ResxKey,ResxValue 
                                        from {0}
                                        where DataBaseName=N'{1}'
                                           and   TableName=N'{2}'
                                           and  ColumnName=N'{3}'
                                           and     ResxKey=N'{4}'
                                        ",
                    localizationResourceTable, dbName, tableName, columnName, resxKey);

                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var resx = dbContext.Database.SqlQuery<LocalizationResourceEntity>(sql).FirstOrDefault();
                    if (resx != null)
                    {
                        resx.ResxValue = resxValues[culturn.ToLowerInvariant().Replace("-", "")];

                        sql = String.Format(@"update {0} set ResxValue = N'{5}'
                                        where DataBaseName=N'{1}'
                                           and   TableName=N'{2}'
                                           and  ColumnName=N'{3}'
                                           and     ResxKey=N'{4}'
                                        ",
                    localizationResourceTable, dbName, tableName, columnName, resxKey, resx.ResxValue);
                        dbContext.Database.ExecuteSqlCommand(sql);
                    }
                    else
                    {
                        var resxValue = resxValues[culturn.ToLowerInvariant().Replace("-", "")];
                        //插入
                        sql = string.Format(@"INSERT INTO {0}
                                                   ([ID]
                                                   ,[DataBaseName]
                                                   ,[TableName]
                                                   ,[ColumnName]
                                                   ,[ResxKey]
                                                   ,[ResxValue])
                                             VALUES
                                                   ('{1}'
                                                   ,N'{2}'
                                                   ,N'{3}'
                                                   ,N'{4}'
                                                   ,N'{5}'
                                                   ,N'{6}')",
                             localizationResourceTable, Guid.NewGuid(), dbName, tableName, columnName, resxKey, resxValue);
                        dbContext.Database.ExecuteSqlCommand(sql);
                    }
                }
            }
        }

        /// <summary>
        /// 获取数据库中提取指定数据库 指定表 指定列的 对应的ResxValue
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> GetAllResxFromDB(string dbName, string tableName, string columnName, string resxKey)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            var key = new Dictionary<string, string>();
            key.Add(resxKey, null);
            result.Add("key", key);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var culturns = LocalizationResxExtend.GetPortalLanguage().Keys;//new string[] { "en-us", "zh-cn", "zh-tw" };
            foreach (var culturn in culturns)
            {
                var localizationResourceTable = "LocalizationResource" + "_" + culturn.ToUpper().Replace("-", "");
                var sql = String.Format(@"select  ID, DataBaseName,TableName ,ColumnName , ResxKey,ResxValue 
                                        from {0}
                                        where DataBaseName=N'{1}'
                                           and   TableName=N'{2}'
                                           and  ColumnName=N'{3}'
                                           and     ResxKey=N'{4}'
                                        ",
                    localizationResourceTable, dbName, tableName, columnName, resxKey);

                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var resx = dbContext.Database.SqlQuery<LocalizationResourceEntity>(sql).FirstOrDefault();
                    if (resx != null)
                    {
                        dic.Add(culturn.Replace("-", ""), resx.ResxValue);
                    }
                }

            }
            result.Add("value", dic);
            return result;
        }

        public static PortalEnvironmentEntity GetPortalEnvironment(string key)
        {
            var sql = String.Format(@"SELECT * FROM PortalEnvironment where [Key]=N'{0}'", key);
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.Database.SqlQuery<PortalEnvironmentEntity>(sql).FirstOrDefault();
                return item;
            }
        }
    }
}