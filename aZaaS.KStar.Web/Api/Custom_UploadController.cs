using aZaaS.Framework.Execl;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Windows.Forms;

namespace aZaaS.KStar.Web.Api
{
    public class Custom_UploadController : ApiController
    {
        [HttpPost]
        [ActionName("ImportExcel")]
        public HttpResponseMessage PostImportExcel([FromBody] string value)
        {
            ExeclHelper help = new ExeclHelper();
            System.Data.DataTable dt = help.ImportExcelData(value);
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return new HttpResponseMessage() { Content = new StringContent(jsonData, Encoding.UTF8) };
        }

        [HttpPost]
        [ActionName("exportPdf")]
        public HttpResponseMessage PostExportPdf([FromBody] string imgData)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            Dictionary<string, object> dicList = new Dictionary<string, object>();
            bool isOK = false;
            string directory = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Cache\\Pdfs\\" ;
            string url = directory + Guid.NewGuid() + ".pdf";
            Document doc = null;
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
               
                string[] fileData = imgData.Split(new string[] { "base64," }, StringSplitOptions.RemoveEmptyEntries);
                string byte64Img = fileData[1]; 
                byte[] bytes = Convert.FromBase64String(byte64Img);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bytes);
                var fit = img.Width / PageSize.A4.Width;
                var height = img.Height / fit;
                var width = img.Width / fit;
                img.ScaleToFit(width, height);
                 
                doc = new Document(PageSize.A4); 
                PdfWriter.GetInstance(doc, new FileStream(url, FileMode.Create)); 
                doc.Open();

                img.SetAbsolutePosition(0, doc.PageSize.Height - height);   
                doc.Add(img);

                float A4height = height > PageSize.A4.Height ? height - PageSize.A4.Height : 0;
                while (A4height != 0)
                {
                    doc.NewPage();
                    img.SetAbsolutePosition(0, doc.PageSize.Height - A4height);
                    doc.Add(img);
                    A4height = A4height - PageSize.A4.Height;
                    A4height = A4height < 0 ? 0 : A4height;
                }
                doc.Close(); 
                isOK = true;
                dicList.Add("url", url.Replace(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "/").Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                isOK = false;
            }
            finally
            {
                doc.Close();
            } 
            dicList.Add("isOK", isOK);
           

            return new HttpResponseMessage() { Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(dicList), Encoding.UTF8) };
        }
  
           
    }
}