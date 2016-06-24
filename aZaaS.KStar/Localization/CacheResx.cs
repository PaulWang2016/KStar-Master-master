using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;

namespace aZaaS.KStar.Localization
{
    public class CacheResx
    {

        /// <summary>
        /// 将指定的文件写入缓存
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="value"></param>
        public static void Add(String filePath, object value)
        {
            CacheDependency cd = new CacheDependency(filePath);
           HostingEnvironment.Cache.Insert(filePath, value, cd);


        }


       /// <summary>
       /// 获取指定key的缓存值
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
        public static Dictionary<string, string> Get(String key)
        {

            return (Dictionary<string, string>)HostingEnvironment.Cache.Get(key);

        }

        public static int GetCount()
        {
            return HostingEnvironment.Cache.Count;
        
        }

    }
}
