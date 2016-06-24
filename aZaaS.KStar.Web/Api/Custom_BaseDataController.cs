using aZaaS.Kstar.DAL;
using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace aZaaS.KStar.Web.Api
{
    public class Custom_BaseDataController : ApiController
    {
      
        [HttpGet]
        [ActionName("GetlabelTemplate")]
        public HttpResponseMessage GetlabelTemplates(int pageIndex, string searchString = null)
        {
            string jsonData = string.Empty;
            try
            {
                jsonData = Custom_BaseData_Dal.GetlabelTemplates(pageIndex, searchString);

            }
            catch (Exception ex)
            {
                jsonData = "[]";
            }
            return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
        }
        [HttpGet]
        [ActionName("GetlabelTemplatePicture")]
        public HttpResponseMessage GetlabelTemplatesPicture(string Number)
        {
            BasisEntityContainer basis = new BasisEntityContainer();
            byte[] bytes = new byte[0];
            try
            {
                bytes = basis.Custom_BaseData_labelTemplate.Where(m => m.Number == Number).Select(s => s.Picture).FirstOrDefault();
                bytes = bytes == null ? new byte[0] : bytes;
            }
            catch (Exception ex)
            {
                var data = ex;

            }
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytes.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return result;

        }
  
        [HttpGet]
        [ActionName("GetBaseData")]
        public HttpResponseMessage GetBaseData(string tableName, int pageIndex, string searchString = null, string filterStr = null, string displayField = null)
        {
            string jsonData = string.Empty;
            try
            {
                string[] displayFields = null;
                if (!string.IsNullOrWhiteSpace(displayField))
                {
                    displayFields = displayField.Split(',');
                }
                FiltrateEntity[] filterStrs = null;
                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filterStrs = FiltrateOperate.JsonToEntity(filterStr);
                }
                jsonData = Custom_BaseData_Dal.GetBaseData(tableName, pageIndex, searchString, filterStrs, displayFields);

            }
            catch (Exception ex)
            {
                jsonData = "[]";
            }
            return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
        }
 
    }
}