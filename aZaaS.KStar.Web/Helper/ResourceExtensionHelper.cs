using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.WorkflowData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Web;

namespace System.Web.Mvc
{
    public static class ResourceExtensionHelper
    {
        public static string GetResourceMsgValue(this HtmlHelper htmlhelper, string key)
        {
            string filePath = htmlhelper.ViewContext.HttpContext.Server.MapPath("/") + "ResourceTips\\";
            string value = GetString(htmlhelper.ViewContext.HttpContext, key, filePath);
            return value;
        }

        /// <summary>
        /// 在 C# 中使用
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetInner(HttpContextBase httpContext, string key)
        {
            string filePath = httpContext.Server.MapPath("/") + "ResourceTips\\";
            return GetString(httpContext, key, filePath);
        }

        private static string GetString(HttpContextBase httpContext, string key, string filePath)
        {
            return LangResourceFileProvider.GetString(key, filePath);
        }

    }

    public static class LangResourceFileProvider
    {
        public static string GetString(string key, string filePath)
        {
            string filename = "ResourceTips.resx";
            ResXResourceReader reader = null;
            reader = new ResXResourceReader(filePath + filename);
            var entry = reader.Cast<DictionaryEntry>().FirstOrDefault<DictionaryEntry>(x => x.Key.ToString() == key);
            return entry.Value == null ? "" : (string)entry.Value;
        }
    }

    public static class TaskListCountProvider
    {
        [Obsolete("平台已经不使用该方式统计待办任务项个数！")]
        public static string GetPendingTasks(TastListType type)
        {
            //int total = 0;
            //AuthenticationType AuthType = AuthenticationType.Windows;
            //string currentUser = string.Empty;
            //if (HttpContext.Current.Session != null && HttpContext.Current.Session["__AuthType"] != null)
            //{
            //    AuthType = (AuthenticationType)HttpContext.Current.Session["__AuthType"];
            //}
            //if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
            //{
            //    currentUser = HttpContext.Current.Session["CurrentUser"].ToString();
            //}
            //WorkflowDataService tenantDatabaseService = new WorkflowDataService(AuthType);
            //switch (type)
            //{
            //    case TastListType.Total:
            //        try
            //        {
            //            var items = tenantDatabaseService.GetPendingTasks(currentUser, AuthType);
            //            if (items != null)
            //            {
            //                total = items.Count();
            //            }
            //        }
            //        catch (Exception ex) { }
            //        break;
            //}
            //return total.ToString();

            throw new NotImplementedException();
        }

        public static string GetDelegation(TastListType type, string pane)
        {
            int total = 0;
            aZaaS.KStar.DelegationService _delegationservice = new aZaaS.KStar.DelegationService();
            string currentUser = string.Empty;
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
            {
                currentUser = HttpContext.Current.Session["CurrentUser"].ToString();
            }
            switch (type)
            {
                case TastListType.Notexpired:                            
                    PageCriteria pageCriteria = new PageCriteria();
                    pageCriteria.PageSize = int.MaxValue;
                    pageCriteria.IsFirstQuery = true;
                    pageCriteria.PageIndex =0;
                    pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "FromUser", Value1 = currentUser, StartLogical = CriteriaLogical.And });
                    pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "TenantID", Value1 = pane, StartLogical = CriteriaLogical.And });
                    pageCriteria.AddRegularFilter(new RegularFilter() { Compare = CriteriaCompare.Equal, FieldName = "IsEnable", Value1 = true, StartLogical = CriteriaLogical.And });
                    try
                    {
                        var list = _delegationservice.GetAllDelegations(pageCriteria);
                        if (list != null)
                        {
                            list = list.Where(x => x.EndDate >= DateTime.Now).ToList();
                            total = list.Count();
                        }
                    }
                    catch (Exception ex) { }
                    break;
            }

            return total.ToString();
        }

        public static string GetMyDrafts(TastListType type)
        {
            int total = 0;
            IStorageProvider _storageProvider = new KStarFormStorageProvider();
            string currentUser = string.Empty;
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
            {
                currentUser = HttpContext.Current.Session["CurrentUser"].ToString();
            }            
            switch (type)
            { 
                case TastListType.Total:
                    try
                    {
                        var items = _storageProvider.GetDrafts(currentUser);
                        if (items != null)
                        {
                            total = items.Count();
                        }
                    }
                    catch (Exception ex) { }
                    break;
            }

            return total.ToString();
        }

        public static string GetWaitRead(TastListType type)
        {
            int total = 0;
            string currentUser = string.Empty;
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
            {
                currentUser = HttpContext.Current.Session["CurrentUser"].ToString();
            }   
            IFormCCProvider _formCCProvider = new KStarFormCCProvider();            
            switch (type)
            {
                case TastListType.Total:
                    try
                    {
                        var items = _formCCProvider.ReceiveFormCC(currentUser, false);
                        if (items != null)
                        {
                            total = items.Count();
                        }
                    }
                    catch (Exception ex) { }
                    break;
            }
            return total.ToString();
        }
    }

    public enum TastListType
    { 
         Total=0,//总条目
         Notexpired=1//未过期        
    }
}