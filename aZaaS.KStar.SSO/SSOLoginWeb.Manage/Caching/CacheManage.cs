
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSOLoginWeb.Manage.DbContext.Caching
{
    public class CacheManage
    {
        private static string _LoginCheckCode = "_LoginCheckCode";

        #region LoginCheckCode
        public static void SetLoginCheckCode(LoginCheckCodetEntity entity,Action<bool> CheckErrorEvent=null)
        {
            List<LoginCheckCodetEntity> LoginCheckCodeList =null;
            System.Web.Caching.Cache cache = GetCache();

            object LoginCheckCodeObject = cache[_LoginCheckCode];
            if (LoginCheckCodeObject == null)
            {
                //新添加
                LoginCheckCodeList = new List<LoginCheckCodetEntity>();
                entity.createTime = DateTime.Now;
                LoginCheckCodeList.Add(entity);
                Set(_LoginCheckCode, LoginCheckCodeList, entity.createTime);

            }
            else
            {
                LoginCheckCodeList = (List<LoginCheckCodetEntity>)LoginCheckCodeObject;
                if (LoginCheckCodeList.Count == 0)
                {//新添加
                    LoginCheckCodeList = new List<LoginCheckCodetEntity>();
                    entity.createTime = DateTime.Now;
                    LoginCheckCodeList.Add(entity);
                    IsSet(_LoginCheckCode);
                    Set(_LoginCheckCode, LoginCheckCodeList, entity.createTime);
                }
                else
                {
                    bool isCheck = false;
                    //修改
                    foreach (LoginCheckCodetEntity item in LoginCheckCodeList)
                    {
                        if (item.address == entity.address && item.userID == entity.userID)
                        {
                            item.errorCount = item.errorCount + 1;
                            if (CheckErrorEvent != null)
                            {
                                if (item.errorCount >= 3)
                                {
                                    CheckErrorEvent.Invoke(true);
                                }
                            }
                            isCheck = true;
                        }
                    }
                    if (!isCheck)
                    {
                        entity.createTime = DateTime.Now;
                        LoginCheckCodeList.Add(entity);
                    }
                    cache[_LoginCheckCode] = LoginCheckCodeList;
                }
            }
        }

        public static int QueryLoginCheckCode(LoginCheckCodetEntity entity)
        {
            System.Web.Caching.Cache cache = GetCache();
            object LoginCheckCodeObject = cache[_LoginCheckCode];
            if (LoginCheckCodeObject == null)
            {
                return 0;
            }
            else
            {
                List<LoginCheckCodetEntity> LoginCheckCodeList = (List<LoginCheckCodetEntity>)LoginCheckCodeObject;

                foreach (LoginCheckCodetEntity item in LoginCheckCodeList)
                {
                    if (item.address == entity.address && item.userID == entity.userID)
                    {
                        return item.errorCount;
                    }
                }
            }
            return 0;
        }

        public static void ClearLoginCheckCode(LoginCheckCodetEntity entity)
        {
            System.Web.Caching.Cache cache = GetCache();
            object LoginCheckCodeObject = cache[_LoginCheckCode];
            if (LoginCheckCodeObject != null)
            {
                List<LoginCheckCodetEntity> LoginCheckCodeList = (List<LoginCheckCodetEntity>)LoginCheckCodeObject;

                LoginCheckCodetEntity tempEntity = null;
                foreach (LoginCheckCodetEntity item in LoginCheckCodeList)
                {
                    if (item.address == entity.address && item.userID == entity.userID)
                    {
                        tempEntity = item;
                        break;
                    }
                }
                if (tempEntity != null)
                {
                    LoginCheckCodeList.Remove(tempEntity);
                    cache[_LoginCheckCode] = LoginCheckCodeList;
                }
            }

        }
        #endregion

        public static System.Web.Caching.Cache GetCache()
        {
            System.Web.Caching.Cache cache = HttpContext.Current.Cache ?? new System.Web.Caching.Cache();
            return cache;
        }
        public static void Set(string key, object data, DateTime? createTime = null)
        {
            System.Web.Caching.Cache cache = GetCache();
            if (createTime == null)
            {
                cache.Add(key, data, null, DateTime.Now.AddMinutes(30), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
            }
            else
            {
                cache.Add(key, data, null, createTime.Value.AddMinutes(30), TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Default, null);
            }

        }

        public object Get(string Key)
        {
            return GetCache()[Key];
        }
        public T Get<T>(string key)
        {
            return (T)GetCache()[key];
        }
        public void Remove(string Key)
        {
            System.Web.Caching.Cache cache = GetCache();

            if (cache[Key] != null)
            {
                cache.Remove(Key);
            }
        }
        public static bool IsSet(string key)
        {
            System.Web.Caching.Cache cache = GetCache();

            return cache[key] != null;
        }
        public void Clear()
        {
            System.Web.Caching.Cache cache = GetCache();
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                cache.Remove(enumerator.Key.ToString());
            }
        }
    }
}