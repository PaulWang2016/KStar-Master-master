using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class FlowThemeController : Controller
    {
        //
        // GET: /Maintenance/FlowTheme/

        public JsonResult GetFlowThemeList()
        {
            FlowThemeRepository flowThemeRepository = new FlowThemeRepository();
            List<ProcessFormFlowTheme> processFormFlowThemeList = flowThemeRepository.GetAll();
            return Json(processFormFlowThemeList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PostAddFlowTheme(string jsonData)
        { 
            bool isOK = false;
            string message = string.Empty;

            try
            {
                ProcessFormFlowTheme entity = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessFormFlowTheme>(jsonData);
                if (entity != null)
                {
                    FlowThemeRepository flowThemeRepository = new FlowThemeRepository();
                    if (string.IsNullOrWhiteSpace(entity.ModelFullName) || string.IsNullOrWhiteSpace(entity.Name) || string.IsNullOrWhiteSpace(entity.ProcessFullName) || string.IsNullOrWhiteSpace(entity.RuleString))
                    {
                        throw new Exception("Null Reference");
                    }
                    //验证规则是否写对
                    if (!string.IsNullOrWhiteSpace(entity.ModelFullName))
                    {
                        Type modelType = System.Type.GetType(entity.ModelFullName, false);
                        string[] rules = RegexUtilities.RegexAngle(entity.RuleString);


                        foreach (string str in rules)
                        {
                            string[] keyArray = str.Split('.');
                            PropertyInfo propertyInfo = null;
                            foreach (string item in keyArray)
                            {
                                if (keyArray.Length == 1 || propertyInfo == null)
                                {
                                    propertyInfo = modelType.GetProperty(item);
                                }
                                else
                                {
                                    propertyInfo = propertyInfo.GetType().GetProperty(item);
                                }
                                if (propertyInfo == null)
                                {
                                    throw new Exception("找不到【" + str + "】字段。");
                                }
                            }
                        }
                    }
                    //添加
                    isOK = flowThemeRepository.Add(entity);
                }
            }
            catch (Exception ex)
            {
                isOK = false;
                message = ex.Message;
            }
            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("succeed", isOK);
            ht.Add("message", message);

            return Json(ht, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostDeleteFlowTheme(int id)
        {
            bool isOK = false;
            string message = string.Empty;
            try
            {
                FlowThemeRepository flowThemeRepository = new FlowThemeRepository();
             
                isOK = flowThemeRepository.Delete(id);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                isOK = false;
            }

            System.Collections.Hashtable ht = new System.Collections.Hashtable();
            ht.Add("succeed", isOK);
            ht.Add("message", message);
            return Json(ht, JsonRequestBehavior.AllowGet);
        }
    }
}
