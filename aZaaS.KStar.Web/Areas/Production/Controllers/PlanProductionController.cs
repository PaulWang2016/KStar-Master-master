using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.ViewResults;
using System.Net.Http;
using System.Text;
using aZaaS.KStar.Web.Areas.Production.Models;
using aZaaS.KStar.Form;
using aZaaS.Kstar.DAL;
using aZaaS.KStar.Web.Models.BasisEntity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.tool.xml;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using aZaaS.KStar.Web.Controllers;


namespace aZaaS.KStar.Web.Areas.Production.Controllers
{
    public class PlanProductionController : FormController
    {
        //
        // GET: /Production/PlanProduction/

        public ActionResult Index()
        {
            AdaptivePlanModel adaptiveModel = new AdaptivePlanModel();


            var _OrganizationService = new KStarFormOrganizationService();
            var userDisplayName = _OrganizationService.GetDisplayName(this.UserName);
            adaptiveModel.PlanProduction.PlannerMan = userDisplayName;

            adaptiveModel.PlanProduction.QualityTesting = true;
            adaptiveModel.PlanProduction.ManagerReview = new ManagerReviewPlan();
            adaptiveModel.PlanProduction.PlanFinishDate = DateTime.Now;
            adaptiveModel.PlanProduction.PlanType = false;
            adaptiveModel.PlanProduction.StartDate = DateTime.Now;

            adaptiveModel.MadeChanges.StartDate = DateTime.Now;
            adaptiveModel.MadeChanges.PlanFinishDate = DateTime.Now;
            adaptiveModel.MadeChanges.ManagerReview = new ManagerReviewMade();
            adaptiveModel.MadeChanges.PlannerMan = userDisplayName;


            adaptiveModel.ConfigFiles.Add(new ConfigFile() {});
            //配置环节
            adaptiveModel.AdaptiveSupplyModels = new AdaptiveSupplyModel();
            adaptiveModel.AdaptiveSupplyModels.SMTPasterInfos.Add(new SMTPasterInfo());
            adaptiveModel.AdaptiveSupplyModels.ToolSoftInfos.Add(new Custom_BaseData_ToolSoft());
            //默认软件信息二行
            adaptiveModel.AdaptiveSupplyModels.SoftInfos.Add(new SoftInfo() { Category = "烧录软件" });
            adaptiveModel.AdaptiveSupplyModels.SoftInfos.Add(new SoftInfo() { Category = "下载软件"});

            return KStarFormView(adaptiveModel);
        }

        public override void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        {
            //流程发起参数
            var model = context.DataModel<AdaptivePlanModel>();
            context.AddDataField("PlanType", model.PlanProduction.PlanType);
            context.ProcessName = this.ProcessName;

            var contentData = context.FormModel.ContentData;
            try
            {
                Newtonsoft.Json.Linq.JToken json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(contentData);
                json["PlanProduction"]["PlanType"] = model.PlanProduction.PlanType ? "正常生产" : "改造";
                context.FormModel.ContentData = json.ToString(Newtonsoft.Json.Formatting.None);
                //执行基类的new Task
                base.OnWorkflowNewTaskStarting(context);
                //还原替换的ContentData
                context.FormModel.ContentData = contentData;
            }
            catch
            {
            }
        }

        public override void OnWorkflowNewTaskStarted(WorkflowTaskContext context)
        {
            base.OnWorkflowNewTaskStarted(context);
            //流程发起参数
            var model = context.DataModel<AdaptivePlanModel>();
            //发起流程 发送抄送
            //发放人为必录的字段
            if (model.PlanProduction.PlanType)
            {
                this.WorkflowTocc(model.PlanProduction.IssueMan.UserName, context.FormId);
            }
            else
            {
                this.WorkflowTocc(model.MadeChanges.IssueMan.UserName, context.FormId);
            }
        }

        public override void OnWorkflowTaskExecuting(WorkflowTaskContext context)
        {
            var model = context.DataModel<AdaptivePlanModel>();
            if (EnumCollection.ActionName.Consent == context.ActionName)
            {
                if (context.ActivityName.Equals("015_重写申请"))
                {
                    context.AddDataField("PlanType", model.PlanProduction.PlanType);
                    //发放人为必录的字段
                    if (model.PlanProduction.PlanType)
                    {
                        this.WorkflowTocc(model.PlanProduction.IssueMan.UserName, context.FormId);
                    }
                    else
                    {
                        this.WorkflowTocc(model.MadeChanges.IssueMan.UserName, context.FormId);
                    }
                }
                else if (context.ActivityName.Equals("030_提供配置文件"))
                {
                    if (model.PlanProduction.IssueMan != null)
                        this.WorkflowTocc(model.PlanProduction.IssueMan.UserName, context.FormId);
                }
                else if (context.ActivityName.Equals("050_项目经理审批"))
                {
                    if (model.PlanProduction.PlanType)
                    {
                        if (model.PlanProduction.IssueMan != null)
                            this.WorkflowTocc(model.PlanProduction.IssueMan.UserName, context.FormId);
                        //根线工程师
                        if (model.PlanProduction.ManagerReview.EngineerMan != null)
                            this.WorkflowTocc(model.PlanProduction.ManagerReview.EngineerMan.UserName, context.FormId);
                    }
                    else
                    {
                        if (model.MadeChanges.IssueMan != null)
                            this.WorkflowTocc(model.MadeChanges.IssueMan.UserName, context.FormId);
                    }
                }
                else if (context.ActivityName.Equals("060_改造人员审核"))
                {
                    if (model.MadeChanges.IssueMan != null)
                        this.WorkflowTocc(model.MadeChanges.IssueMan.UserName, context.FormId);
                }
            }
            context.ProcessName = this.ProcessName;

            var contentData = context.FormModel.ContentData;
            try
            {
                Newtonsoft.Json.Linq.JToken json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(contentData);
                json["PlanProduction"]["PlanType"] = model.PlanProduction.PlanType ? "正常生产" : "改造";
                context.FormModel.ContentData = json.ToString(Newtonsoft.Json.Formatting.None);
                //执行基类的new Task 
                base.OnWorkflowTaskExecuting(context);
                //还原替换的ContentData
                context.FormModel.ContentData = contentData;
            }
            catch
            {
            }
        }

       //public ActionResult ExportPdf(int _FormId)
        //{
        //    AdaptivePlanModel adaptiveModel = null;
        //    using (BasisEntityContainer container = new BasisEntityContainer())
        //    {
        //        ProcessFormContent context = container.ProcessFormContents.Where(x => x.FormID == _FormId).FirstOrDefault();
        //        if (context == null) return null;
        //        adaptiveModel = Newtonsoft.Json.JsonConvert.DeserializeObject<AdaptivePlanModel>(context.JsonData);
        //    }
        //    string htmlText = RenderPartialViewToString(this, "PDFTemplate", adaptiveModel);

        //    byte[] pdfFile = this.ConvertHtmlTextToPDF(htmlText);

        //    return File(pdfFile, "application/pdf", "有方科技生产计划单审批流程.pdf");
            
        //} 
       /// <summary>
        /// 将PartialView输出为字符串
        /// </summary>
        /// <param name="controller">Controller实例</param>
        /// <param name="viewName">如果PartialView文件在当前Controller目录下，则直接输入文件名(例:Toolbar)；否则,从根路径开始指定(例：~/Views/User/Toolbar.cshtml)</param>
        /// <param name="model">构造页面所需的的实体参数</param>
         /// <returns>字符串</returns>
       //[NonAction]
        //public static string RenderPartialViewToString(Controller controller, string viewName, object model)
        //{
        //    IView view = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName).View;
        //    controller.ViewData.Model = model;
        //    using (StringWriter writer = new StringWriter())
        //    {
        //        ViewContext viewContext = new ViewContext(controller.ControllerContext, view, controller.ViewData, controller.TempData, writer);
        //        viewContext.View.Render(viewContext, writer);
        //        return writer.ToString();
        //    }
        //}

      //  /// <summary>
      //  /// 将Html文字 输出到PDF档里
      //  /// </summary>
      //  /// <param name="htmlText"></param>
      //  /// <returns></returns>
      //  [NonAction]
      //  public byte[] ConvertHtmlTextToPDF(string htmlText)
      //  {
      //      if (string.IsNullOrEmpty(htmlText))
      //      {
      //          return null;
      //      }
      //      //避免当htmlText无任何html tag标签的纯文字时，转PDF时会挂掉，所以一律加上<p>标签
      //     // htmlText = "<p>" + htmlText + "</p>";

      //      MemoryStream outputStream = new MemoryStream();//要把PDF写到哪个串流
      //      byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串转成byte[]
      //      MemoryStream msInput = new MemoryStream(data);
      //      Document doc = new Document();//要写PDF的文件，建构子没填的话预设直式A4
      //      PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
      //      //指定文件预设开档时的缩放为100%

      //      PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
      //      //开启Document文件 
      //      doc.Open();

      //      //使用XMLWorkerHelper把Html parse到PDF档里
      //      XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
      //      //将pdfDest设定的资料写到PDF档
      //      PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
      //      writer.SetOpenAction(action);
      //      doc.Close();
      //      msInput.Close();
      //      outputStream.Close();
      //      //回传PDF档案 
      //      return outputStream.ToArray();

      //  }
      //  //设置字体类
      //  public class UnicodeFontFactory : FontFactoryImp
      //  {
      //      private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
      //"arialuni.ttf");//arial unicode MS是完整的unicode字型。
      //      private static readonly string 标楷体Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
      //      "KAIU.TTF");//标楷体


      //      public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
      //      {
      //          BaseFont bfChiness = BaseFont.CreateFont(@"C:\Windows\Fonts\simhei.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
      //          //可用Arial或标楷体，自己选一个
      //          BaseFont baseFont = BaseFont.CreateFont(标楷体Path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
      //          return new Font(baseFont, size, style, color);
      //      }
      //  }    

        public override string ProcessName
        {
            get { return @"Innos.KStar.Workflow\Production"; }
        }
    }
}