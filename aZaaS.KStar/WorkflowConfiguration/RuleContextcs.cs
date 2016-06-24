using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    ///   @contex
    /// </summary>
    public class RuleContext
    { 
        private ProcessFormContent getFormContentDate(int procInstID)
        {
            using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
            {
                var linq = from h in dbContext.ProcessFormHeader
                           join c in dbContext.ProcessFormContent on h.FormID equals c.FormID
                           into XmlData
                           from pro in XmlData.DefaultIfEmpty()
                           where h.ProcInstID == procInstID
                           select pro;

                return  linq.FirstOrDefault();
            }
        }
         
        /// <summary>
        /// 获取表单head内容的xml 数据
        /// </summary>
        /// <param name="procInstID"></param>
        /// <returns></returns>
        private ProcessFormHeader getFormHeadDate(int procInstID)
        {
            ProcessFormHeader processFormHeader = null;
            using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
            {
                processFormHeader = dbContext.ProcessFormHeader.Where(x => x.ProcInstID == procInstID).FirstOrDefault();

            }
            return processFormHeader;
        }

        #region  public  function attribute

        public JObject GetDictionaryById(string sysID)
        {
            try
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var dict = dbContext.DataDictionary.Where(x => x.Id == Guid.Parse(sysID)).FirstOrDefault();
                    if (dict != null)
                    {
                        return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public JObject GetDictionaryByCode(string code)
        {
            try
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var dict = dbContext.DataDictionary.Where(x => x.Code == code).FirstOrDefault();
                    if (dict != null)
                    {
                        return JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dict));
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public string GetDictionaryValueById(string sysID)
        {
            try
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var dict = dbContext.DataDictionary.Where(x => x.Id == Guid.Parse(sysID)).FirstOrDefault();
                    if (dict != null)
                    {
                        return dict.Value;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public string GetDictionaryValueByCode(string code)
        {
            try
            {
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    var dict = dbContext.DataDictionary.Where(x => x.Code == code).FirstOrDefault();
                    if (dict != null)
                    {
                        return dict.Value;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }


        public ProcessFormHeader FormHeader { set; get; }


        public JObject FormContentJson { set; get; }

        /// <summary>
        /// FormContent
        /// </summary>
        public ProcessFormContent _FormContent { set; get; }
         /// <summary>
         /// 流程全路径
         /// </summary>
        public string ProcessFullName { set; get; }

        public string ActName { set; get; }

        /// <summary>
        /// 实例ID
        /// </summary>
        public int ProcInstID { set; get; }
         
        /// <summary>
        /// 用户选择结果
        /// </summary>
        public string Outcome { set; get; }
         
        #endregion

        public RuleContext(int procInstID)
        {
            this.ProcInstID = procInstID;

            this.FormHeader = getFormHeadDate(procInstID);

            this._FormContent = getFormContentDate(procInstID);
            this.FormContentJson = JObject.Parse(this._FormContent.JsonData);
        }
    }
}
