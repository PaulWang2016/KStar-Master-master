using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using aZaaS.KStar.Form;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Mvc.Controllers;
using CsQuery;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.MgmtDtos;

using aZaaS.KStar.Form.Mvc;
using aZaaS.Kstar.DAL;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public static class HtmlHelpers
    {
        #region Context Properties

        public static WorkMode GetWorkMode(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.Controller.GetWorkMode();
        }

        public static string GetActivityName(this HtmlHelper htmlHelper)
        {
            var activityName = string.Empty;

             if (htmlHelper.ViewContext.Controller is KStarFormController)
                 activityName = (htmlHelper.ViewContext.Controller as KStarFormController).ActivityName;

             return activityName;
        }

        public static string GetPositionUsers(this HtmlHelper htmlHelper,string Position)
        {
            var Users = new object();

            if (!string.IsNullOrWhiteSpace(Position))
                Users = CustomExtUtility.GetScalarData(string.Format("SP_NW_GetPositionUsers '{0}'", Position));

            return Users == null ? string.Empty : Users.ToString();
        }

        public static bool IsSIMManagement(this HtmlHelper htmlHelper)
        {
            var Users = CustomExtUtility.GetScalarData(string.Format("SP_NW_GetPositionUsers '{0}'", "SIM卡管理员"));
            var userName = HttpContext.Current.Session["CurrentUser"].ToString();

            return Users == null ? false : Users.ToString().IndexOf(userName) >= 0;
        }

        public static bool IsControlSetting(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.Controller.IsControlSetting();
        }

        public static WorkMode ControlSettingMode(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.Controller.ControlSettingMode();
        }

        //public static IHtmlString GetKStarFormUser(this HtmlHelper htmlHelper)
        //{
        //    UserDto LoginUser = null;

        //    if (htmlHelper.ViewContext.Controller is KStarFormController)
        //        LoginUser = (htmlHelper.ViewContext.Controller as KStarFormController).GetLoginUser();
  

        //    return htmlHelper.Raw(string.Format(@"KStar.User={0};", JsonHelper.SerializeObject(LoginUser)));
        //}

        #endregion

        #region PartialView Rendering

        public static HtmlString RenderKStarFormNavigation(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_KStarForm_Navigation_Part");
        }

        public static HtmlString RenderKStarFormHeader(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_KStarForm_Header_Part");
        }

        public static HtmlString RenderKStarFormToolbar(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_KStarForm_Toolbar_Part");
        }

        public static HtmlString RenderFormToolbar(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_Form_Toolbar_Part");
        }

        public static HtmlString RenderKStarFormFooter(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_KStarForm_Footer_Part");
        }

        public static HtmlString RenderKStarFormAttachment(this HtmlHelper htmlHelper)
        {
            return PartialExtensions.Partial(htmlHelper, "_KStarForm_Attachment_Part");
        }

        public static HtmlString RenderKStarFormProcessLog(this HtmlHelper htmlHelper)
        {
            if ((!htmlHelper.IsControlSetting() && htmlHelper.GetWorkMode() == WorkMode.Startup)
                || (htmlHelper.IsControlSetting() && htmlHelper.ControlSettingMode() == WorkMode.Startup))
                return new HtmlString("");

            return PartialExtensions.Partial(htmlHelper, "_KStarForm_ProcessLog_Part");
        }

        public static HtmlString RenderKStarFormControlSettings(this HtmlHelper htmlHelper)
        {
            string _html=string.Empty;
            if (htmlHelper.ViewContext.Controller is KStarFormController)
            {
                var controller = htmlHelper.ViewContext.Controller as KStarFormController;
                var _formSettingProvider = controller.FormSettingProvider;
                var _controlTmplProvider = controller.ControlTmplProvider;
                IList<CotnrolSettingModel> _settings = _formSettingProvider.GetAllControlSettings();
                IList<ControlTemplateModel> _template = _controlTmplProvider.GetTemplate();

                CQ _doc = htmlHelper.ViewContext.Controller.GetTemplateContent();
                StringBuilder componenttemplate = new StringBuilder();
                #region  -----模板html内容
                componenttemplate.Append("");
                componenttemplate.Append("<div class=\"panel panel-default kstar-component\">");
                componenttemplate.Append("        <div class=\"panel-heading\">");
                componenttemplate.Append("            <h4 class=\"panel-title\">");
                componenttemplate.Append("                <a data-toggle=\"collapse\" href=\"#ApplicationInformation\">申请信息-(点击折叠)");
                componenttemplate.Append("                </a>");
                componenttemplate.Append("            </h4>");
                componenttemplate.Append("        </div>");
                componenttemplate.Append("        <div id=\"ApplicationInformation\" class=\"panel-collapse collapse in\">");
                componenttemplate.Append("            <div class=\"panel-body\">");
                componenttemplate.Append("                <div class=\"row\">");
                componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                componenttemplate.Append("                        <input id=\"showappinfo\" class='chkcontrol chkshow' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsHide\"  style=\"vertical-align: top;\" /><label for=\"showappinfo\" style=\"cursor: pointer\">是否隐藏</label>");
                componenttemplate.Append("                    </div>");
                componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                componenttemplate.Append("                        <input id=\"disabledappinfo\" class='chkcontrol chkdisabled' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsDisable\"  style=\"vertical-align: top;\" /><label for=\"disabledappinfo\" style=\"cursor: pointer\">禁用子控件</label>");
                componenttemplate.Append("                    </div>");
                componenttemplate.Append("                    <div class=\"col-md-2\" style=\"\">");
                componenttemplate.Append("                        <input id=\"customappinfo\" class='chkcontrol chkcustom chkcustomtree' type=\"checkbox\" data-bind=\"checked: ComponentModel.IsCustom\" style=\"vertical-align: top;\" /><label for=\"templateappinfo\" style=\"cursor: pointer\">使用模板</label>");
                componenttemplate.Append("                    </div>");
                componenttemplate.Append("                    <div class=\"col-md-6\" style=\"\">");
                componenttemplate.Append("                        <div class=\"row\">");
                componenttemplate.Append("                            <div class=\"col-sm-4\">");
                componenttemplate.Append("                                <label class=\"control-label\">组件使用该模板显示:</label>");
                componenttemplate.Append("                            </div>");
                componenttemplate.Append("                            <div class=\"col-sm-8\">");
                componenttemplate.Append("                                <input data-bind=\"value: ComponentModel.RenderTemplateName\" type=\"text\" readonly data-rule-required=\"true\" data-msg-required=\"请选择模板！\" class=\"form-control dropdowntree\" placeholder=\"请选择模板\" />");
                componenttemplate.Append("                                <input data-bind=\"value: ComponentModel.RenderTemplateId\"  type=\"hidden\" />");
                componenttemplate.Append("                            </div>");
                componenttemplate.Append("                        </div>");
                componenttemplate.Append("                    </div>");
                componenttemplate.Append("                </div>");
                componenttemplate.Append("                <div>");
                componenttemplate.Append("                    <table class=\"table table-striped table-hover\">");
                componenttemplate.Append("                        <thead>");
                componenttemplate.Append("                            <tr>");
                componenttemplate.Append("                                <th>控件#id</th>");
                componenttemplate.Append("                                <th>控件名称</th>");
                componenttemplate.Append("                                <th>操作选项</th>");
                componenttemplate.Append("                            </tr>");
                componenttemplate.Append("                        </thead>");
                componenttemplate.Append("                        <tbody data-bind=\"foreach: $root.ControlModel\">");
                componenttemplate.Append("                            <tr>");
                componenttemplate.Append("                                <td><span data-bind=\"text: ControlRenderId\"></span></td>");
                componenttemplate.Append("                                <td><span data-bind=\"text: ControlName\"></span></td>");
                componenttemplate.Append("                                <td>");
                componenttemplate.Append("                                    <div class=\"row\">");
                componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                componenttemplate.Append("                                            <input  class='chkcontrol chkshow' data-bind=\"checked: IsHide\"  type=\"checkbox\" value=\"\" style=\"vertical-align: top;\" /><label  style=\"cursor: pointer\">是否隐藏</label>");
                componenttemplate.Append("                                        </div>");
                componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                componenttemplate.Append("                                            <input class='chkcontrol chkdisabled'   data-bind=\"checked: IsDisable\"   type=\"checkbox\" value=\"\" style=\"vertical-align: top;\" /><label  style=\"cursor: pointer\">禁用控件</label>");
                componenttemplate.Append("                                        </div>");
                componenttemplate.Append("                                        <div class=\"col-md-4\" style=\"\">");
                componenttemplate.Append("                                            <input class='chkcontrol chkcustom chkcustomcontroltree' data-bind=\"checked: IsCustom\"   type=\"checkbox\" value=\"\" style=\"vertical-align: top;\" /><label  style=\"cursor: pointer\">使用模板</label>");
                componenttemplate.Append("                                        </div>");
                componenttemplate.Append("                                    </div>");
                componenttemplate.Append("                                    <div class=\"row\">");
                componenttemplate.Append("                                        <div class=\"col-md-12\" style=\"\">");
                componenttemplate.Append("                                            <div class=\"row\">");
                componenttemplate.Append("                                                <div class=\"col-sm-8\">");
                componenttemplate.Append("                                                    <label class=\"control-label\"></label>");
                componenttemplate.Append("                                                </div>");
                componenttemplate.Append("                                                <div class=\"col-sm-4\">");
                componenttemplate.Append("                                                    <input data-bind=\"value: RenderTemplateName,attr:{'data': RenderTemplateId}\"  type=\"text\" readonly data-rule-required=\"true\" data-msg-required=\"请选择模板！\" class=\"form-control dropdowntree\" placeholder=\"请选择模板\" />");
                componenttemplate.Append("                                                    <input data-bind=\"value: RenderTemplateId\"  type=\"hidden\" />");
                componenttemplate.Append("                                                </div>");
                componenttemplate.Append("                                            </div>");
                componenttemplate.Append("                                        </div>");
                componenttemplate.Append("                                    </div>");
                componenttemplate.Append("                                </td>");
                componenttemplate.Append("                            </tr>");
                componenttemplate.Append("                        </tbody>");
                componenttemplate.Append("                    </table>");
                componenttemplate.Append("                </div>");
                componenttemplate.Append("            </div>");
                componenttemplate.Append("        </div>");
                componenttemplate.Append("    </div>");
                #endregion

                List<IDomObject> _componentlist = new List<IDomObject>();
                //component
                IList<IDomObject> _kstar_componentlist = _doc[".kstar-component"].ToList();
                //循环 获取组件
                foreach (var component in _kstar_componentlist)
                {
                    string componentId=component.GetAttribute("role");
                    string componentName = component.GetAttribute("title");
                    CQ _doctemplate = CQ.CreateDocument(componenttemplate.ToString());
                    CQ _component_temp = CQ.CreateDocument(component.InnerHTML);                    
                    //获取已保存的组件信息
                    CotnrolSettingModel _componentModel = _settings.Where(x => x.ControlRenderId == componentId && x.ControlType == ControlType.Component).FirstOrDefault();
                    //更新组件信息                    
                    if (_componentModel == null)
                    {
                        _componentModel = new CotnrolSettingModel();
                        _componentModel.ControlName = componentName;
                        _componentModel.ControlRenderId = componentId;
                        _componentModel.ControlType = ControlType.Component;
                    }
                    else
                    { 
                        if(!string.IsNullOrEmpty(_componentModel.RenderTemplateId))
                        {
                            Guid tempid=Guid.Parse(_componentModel.RenderTemplateId);
                            ControlTemplateModel temp =_template.Where(x=>x.SysId==tempid).FirstOrDefault();
                            _componentModel.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                        }
                    }

                    List<CotnrolSettingModel> _controlModel = new List<CotnrolSettingModel>();
                    //script
                    StringBuilder _script = new StringBuilder();
                    //替换标题
                    _doctemplate["div.panel-heading a"].Html(componentName);
                    //替换id
                    _doctemplate["div.panel-default"].Attr("id", componentId).Attr("model", "_" + componentId + "kocomponentModel");
                    _doctemplate["div.panel-collapse"].Attr("id", componentId + "_control");
                    _doctemplate["div.panel-heading a"].Attr("href", "#" + componentId + "_control");

                    //组件
                    var chkshow = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(0);
                    var chkdisabled = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(1);
                    var chkcustom = _doctemplate["div.panel-collapse div.row"].Eq(0).Find("input").Eq(2);

                    chkshow.Attr("id", "show" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'show',$(this).prop('checked'));");
                    chkdisabled.Attr("id", "disabled" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'disabled',$(this).prop('checked'));");
                    chkcustom.Attr("id", "custom" + componentId).Attr("onclick", "ControlSettingWindow.updateKoModelForCheck(_" + componentId + "kocomponentModel,'custom',$(this).prop('checked'));");


                    _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(0).Attr("for", "show" + componentId);
                    _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(1).Attr("for", "disabled" + componentId);
                    _doctemplate["div.panel-collapse div.row"].Eq(0).Find("label").Eq(2).Attr("for", "custom" + componentId);


                    _doctemplate["tbody"].Attr("data-bind", "foreach:ControlModel");
                    //循环获取控件
                    IList<IDomObject> _kstar_controllist = _component_temp[".kstar-control"].ToList();
                    foreach (var control in _kstar_controllist)
                    {
                        //获取已存的控件信息
                        CotnrolSettingModel _controlModel_res = _settings.Where(x => x.ControlRenderId == control.Id && x.ControlType == ControlType.Control).FirstOrDefault();
                        if (_controlModel_res == null)
                        {
                            _controlModel_res = new CotnrolSettingModel()
                            {
                                ControlRenderId = control.Id,
                                ControlName = control.GetAttribute("title"),
                                ControlType = ControlType.Control,
                                ComponentId = componentId
                            };
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_controlModel_res.RenderTemplateId))
                            {
                                Guid tempid = Guid.Parse(_controlModel_res.RenderTemplateId);
                                ControlTemplateModel temp = _template.Where(x => x.SysId == tempid).FirstOrDefault();
                                _controlModel_res.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                            }
                            _controlModel_res.ComponentId = componentId;
                        }
                        _controlModel.Add(_controlModel_res);
                    }
                    //循环获取按钮
                    IList<IDomObject> _kstar_buttonlist = _component_temp[".kstar-button"].ToList();
                    foreach (var control in _kstar_buttonlist)
                    {
                        //获取已存的控件信息
                        CotnrolSettingModel _controlModel_res = _settings.Where(x => x.ControlRenderId == control.Id && x.ControlType == ControlType.Button).FirstOrDefault();
                        if (_controlModel_res == null)
                        {
                            _controlModel_res = new CotnrolSettingModel()
                            {
                                ControlRenderId = control.Id,
                                ControlName = control.GetAttribute("title"),
                                ControlType = ControlType.Button,
                                ComponentId = componentId
                            };
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(_controlModel_res.RenderTemplateId))
                            {
                                Guid tempid = Guid.Parse(_controlModel_res.RenderTemplateId);
                                ControlTemplateModel temp = _template.Where(x => x.SysId == tempid).FirstOrDefault();
                                _controlModel_res.RenderTemplateName = (temp == null ? "" : temp.DisplayName);
                            }
                            _controlModel_res.ComponentId = componentId;
                        }
                        _controlModel.Add(_controlModel_res);
                    }
                    var _kocomponentModel = new { ComponentModel = _componentModel, ControlModel = _controlModel };
                    _script.Append("<script type=\"text/javascript\">");
                    _script.Append("var _" + componentId + "kocomponentModel = KStarForm.toKoModel(" + JsonHelper.SerializeObject(_kocomponentModel) + ");");
                    _script.Append("KStarForm.applyBindings(_" + componentId + "kocomponentModel, $('#" + componentId + "')[0]);");
                    _script.Append("</script>");
                    _componentlist.Add(_doctemplate[0]);
                    _componentlist.Add(CQ.CreateDocument(_script.ToString())[0]);
                }

                CQ _resultdoc = CQ.Create(_componentlist);
                _html= _resultdoc.Render();                
            }
            return new HtmlString(_html);
        }

        #endregion

        #region ViewModel Registrations

        public static IHtmlString ApplyFormData(this HtmlHelper htmlHelper)
        {
            var model = new KStarFormModel();
            var jsonData = "{}";
            if (htmlHelper.ViewContext.Controller is FormController)
            {
                var controller = htmlHelper.ViewContext.Controller as FormController;
                var formId = GetFormId(controller);
                var Proc = GetProc(controller);
                if (formId > 0)
                    jsonData = CustomExtUtility.GetjsonData("SP_NW_GetData",
                        new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter("@FormId", formId) });
            }

            var formJSModel = model.Json();
            var contentJSModel = jsonData;

            formJSModel = string.IsNullOrEmpty(formJSModel) ? "{}" : formJSModel;
            contentJSModel = string.IsNullOrWhiteSpace(contentJSModel) ? "{}" : contentJSModel;
            return htmlHelper.Raw(string.Format(@"var _data={0};_data.ContentData={1};KStarForm.applyData(_data);", formJSModel, contentJSModel));
        }

        public static IHtmlString ApplyData(this HtmlHelper htmlHelper)
        {
            var jsonData = "{}";
            if (htmlHelper.ViewContext.Controller is KStarFormController)
            {
                var controller = htmlHelper.ViewContext.Controller as KStarFormController;
                var formId = GetFormId(controller);
                //var Proc = GetProc(controller);
                if (formId > 0)
                    jsonData = CustomExtUtility.GetjsonData("SP_NW_GetData",
                        new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter("@FormId", formId) });
            }
            var contentJSModel = jsonData;
            contentJSModel = string.IsNullOrWhiteSpace(contentJSModel) ? "{}" : contentJSModel;
            return htmlHelper.Raw(string.Format(@"var _data={0};", contentJSModel));
        }

        public static int GetFormId(this ControllerBase controller)
        {
            int formId = 0;
            var sFormId = controller.ControllerContext.HttpContext.Request["FormId"];
            int.TryParse(sFormId, out formId);

            return formId;
        }

        public static string GetProc(this ControllerBase controller)
        {
            var Proc = controller.ControllerContext.HttpContext.Request["Proc"];

            return Proc;
        }

        public static IHtmlString RegisterContentModel(this HtmlHelper htmlHelper, object model)
        {
            return htmlHelper.Raw(string.Format("KStarForm.registerModel({0});",JsonHelper.SerializeObject(model)));
        }

        private static KStarFormModel InitFormModelData(string userName)
        {
            IOrganizationService svc = new KStarFormOrganizationService();
            var userInfo = svc.GetUser(userName);

            if (userInfo == null)
            {
                return new KStarFormModel();
            }

            var formModel = new KStarFormModel()
            {
                FormSubject = string.Empty,
                ProcessFolio = string.Empty,
                SubmitterAccount = userInfo.UserName,
                SubmitterDisplayName = string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName),
                ApplicantAccount = userInfo.UserName,
                ApplicantDisplayName = string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName),
                ApplicantTelNo = userInfo.Phone,
                ApplicantEmail = userInfo.Email,
                SubmitDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            formModel.Attachments = new List<AttachmentModel>();
            formModel.ProcessLogs = new List<ProcessLogModel>();
            formModel.Toolbar = new ToolbarActionModel();

            return formModel;
        }

        private static ToolbarActionModel GetToolbarModel(WorkflowTaskModel taskModel)
        {
            return new ToolbarActionModel()
            {
                IsEnableSign = taskModel.IsEnableSign,
                SignActionName = taskModel.SignActionName,
                TaskActions = taskModel.Actions,
                ReworkActivitys = GetReworkActivityList(taskModel.ActivityId)
            };
        }

        private static IList<Activity> GetReworkActivityList(int activityId)
        {
            var configManager = new ConfigManager(AuthenticationType.Windows);
            var list = configManager.GetReworkActivityListByActId(activityId);

            return list;
        }

        #endregion
    }
}
