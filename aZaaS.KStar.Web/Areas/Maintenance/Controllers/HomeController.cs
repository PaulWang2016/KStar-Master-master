using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Models;
using aZaaS.Framework.Extend;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Localization;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class HomeController : BaseMvcController
    {
        public static List<ScheduleConfigView> ScheduleConfigList;
        public static List<TargetSys> TargetList;
        public static List<SourceSys> SourceList;
        public static List<Setting> SettingList;
        public static List<TreeViews> SettingsList;
        public static List<TreeViews> MenuList;

        private readonly UserService userService;
        private readonly PositionService positionService;
        private readonly OrgChartService orgchartService;

        public HomeController()
        {
            if (null == SettingList)
            {
                SettingList = new List<Setting>();
                SettingList.Add(new Setting() { SettingsID = "3", Category = "EmailSetting", SettingName = "SettingName1", SettingValue = "SettingValue8", Description = "Description" });
                SettingList.Add(new Setting() { SettingsID = "3", Category = "EmailSetting", SettingName = "SettingName2", SettingValue = "SettingValue7", Description = "Description1" });
                SettingList.Add(new Setting() { SettingsID = "2", Category = "EmailSetting", SettingName = "SettingName3", SettingValue = "SettingValue6", Description = "Description2" });
                SettingList.Add(new Setting() { SettingsID = "2", Category = "EmailSetting", SettingName = "SettingName4", SettingValue = "SettingValue5", Description = "Description3" });
                SettingList.Add(new Setting() { SettingsID = "4", Category = "EmailSetting", SettingName = "SettingName5", SettingValue = "SettingValue4", Description = "Description4" });
                SettingList.Add(new Setting() { SettingsID = "2", Category = "EmailSetting", SettingName = "SettingName6", SettingValue = "SettingValue3", Description = "Description5" });
                SettingList.Add(new Setting() { SettingsID = "4", Category = "EmailSetting", SettingName = "SettingName7", SettingValue = "SettingValue2", Description = "Description6" });
                SettingList.Add(new Setting() { SettingsID = "4", Category = "EmailSetting", SettingName = "SettingName8", SettingValue = "SettingValue1", Description = "Description7" });
            }

            if (null == SettingsList)
            {
                SettingsList = new List<TreeViews>();
               // SettingsList.Add(new TreeViews() { ID = "1", Name = "SettingName1", HasChildren = true, ParentID = null });
                SettingsList.Add(new TreeViews() { ID = "Master", Name = "Master", HasChildren = true, ParentID = null });
                SettingsList.Add(new TreeViews() { ID = "TradeMix", Name = "Trade Mix", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "Nature", Name = "Nature", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "Reason", Name = "Reason", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "Electiondistrict", Name = "Election district", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "CUSTOMERS", Name = "Customer", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "PROPERTIES", Name = "Property", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "UNITS", Name = "Unit", HasChildren = false, ParentID = "Master" });
                SettingsList.Add(new TreeViews() { ID = "Leanes", Name = "Leanes", HasChildren = false, ParentID = "Master" });
            }

            if (null == MenuList)
            {
            //    MenuList = new List<TreeViews>();
            //    MenuList.Add(new TreeViews() { ID = 1, Name = "SettingName1", Link = "Link 1", HasChildren = true, ParentID = 0, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 2, Name = "eForm", Link = "Link 2", HasChildren = true, ParentID = 1, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 3, Name = "Incident Registry", Link = "Link 3", HasChildren = false, ParentID = 1, Status = true });
            //    MenuList.Add(new TreeViews() { ID = 4, Name = "Preventive Maintenance", Link = "Link 4", HasChildren = false, ParentID = 1, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 5, Name = "Inspection", Link = "Link 5", HasChildren = false, ParentID = 1, Status = true });
            //    MenuList.Add(new TreeViews() { ID = 6, Name = "Asset Management", Link = "Link 6", HasChildren = false, ParentID = 1, Status = true });

            //    MenuList.Add(new TreeViews() { ID = 7, Name = "Tenant Database", HasChildren = true, ParentID = 2, Status = false });



            //    MenuList.Add(new TreeViews() { ID = 8, Name = "Workflow Management", HasChildren = true, ParentID = 7, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 14, Name = "My Dashboard", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 15, Name = "My Pending Tasks", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 16, Name = "My Request Tasks", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 17, Name = "On-Going Tasks", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 18, Name = "Completed Tasks", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 19, Name = "My Delegation", HasChildren = false, ParentID = 8, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 20, Name = "My Draft", HasChildren = false, ParentID = 8, Status = false });

            //    MenuList.Add(new TreeViews() { ID = 9, Name = "Maintenance(Admin Only)", HasChildren = true, ParentID = 7, Status = true });
            //    MenuList.Add(new TreeViews() { ID = 21, Name = "Organization Management", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 22, Name = "Position Management", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 23, Name = "User Management", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 24, Name = "Master Management", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 25, Name = "Menu Configuration", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 26, Name = "Schedule Configuration", HasChildren = false, ParentID = 9, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 27, Name = "Email Template", HasChildren = false, ParentID = 9, Status = false });

            //    MenuList.Add(new TreeViews() { ID = 10, Name = "Report", HasChildren = true, ParentID = 7, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 28, Name = "Tenant Report", HasChildren = false, ParentID = 10, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 29, Name = "Summary Report", HasChildren = false, ParentID = 10, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 30, Name = "Detail Report", HasChildren = false, ParentID = 10, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 31, Name = "KPI Report", HasChildren = false, ParentID = 10, Status = false });

            //    MenuList.Add(new TreeViews() { ID = 11, Name = "Links", HasChildren = true, ParentID = 7, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 29, Name = "Submit Tenant Database Request", HasChildren = false, ParentID = 11, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 30, Name = "Cancel Tenant Database Request", HasChildren = false, ParentID = 11, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 31, Name = "Tenant Database Form Guide", HasChildren = false, ParentID = 11, Status = false });

            //    MenuList.Add(new TreeViews() { ID = 12, Name = "Support Documents", HasChildren = true, ParentID = 7, Status = true });
            //    MenuList.Add(new TreeViews() { ID = 29, Name = "Tenant Database Quick Guide", HasChildren = false, ParentID = 12, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 30, Name = "Tenant Database Workflow Application Guide", HasChildren = false, ParentID = 12, Status = false });

            //    MenuList.Add(new TreeViews() { ID = 13, Name = "Support Videos", HasChildren = true, ParentID = 7, Status = true });
            //    MenuList.Add(new TreeViews() { ID = 29, Name = "Tenant Database Overview", HasChildren = false, ParentID = 13, Status = false });
            //    MenuList.Add(new TreeViews() { ID = 30, Name = "Tenant Administration", HasChildren = false, ParentID = 13, Status = false });
            }

            #region Init Schedule Config Data
            if (null == ScheduleConfigList)
            {
                ScheduleConfigList = new List<ScheduleConfigView>();
                //ScheduleConfigList.Add(new ScheduleConfigView() { ScheduleID = 1, TargetName = "Sql Server", SourceName = "Oracle", CreateTime = DateTime.Now, NextRunTime = DateTime.Now.AddHours(2), Status = true, TerminationTime = DateTime.MaxValue, DisplayName = "Automatic approval process", IntervalPeriod = "5 minutes" });
            }

            if (null == TargetList)
            {
                TargetList = new List<TargetSys>();
                TargetList.Add(new TargetSys() { TargetID = 1, DisplayName = "FTP" });
                TargetList.Add(new TargetSys() { TargetID = 2, DisplayName = "Sql Server" });
                TargetList.Add(new TargetSys() { TargetID = 3, DisplayName = "Shared directory" });
                TargetList.Add(new TargetSys() { TargetID = 4, DisplayName = "Oracle" });
                //TargetList.Add(new Models.TargetSys() { TargetID = 5, DisplayName = "" });
            }

            if (null == SourceList)
            {
                SourceList = new List<SourceSys>();
                SourceList.Add(new SourceSys() { SourceID = 1, DisplayName = "FTP" });
                SourceList.Add(new SourceSys() { SourceID = 2, DisplayName = "Sql Server" });
                SourceList.Add(new SourceSys() { SourceID = 3, DisplayName = "Shared directory" });
                SourceList.Add(new SourceSys() { SourceID = 4, DisplayName = "Oracle" });
                //SourceList.Add(new Models.SourceSys() { SourceID = 5, DisplayName = "" });
            }

             userService=new UserService();
             positionService = new PositionService();
             orgchartService = new OrgChartService();
            #endregion
        }

        public ActionResult Organization(bool isAjax = false)
        {
            if (isAjax)
            {
                FieldBase[] fields = orgchartService.GetNodeExtendFieldsDefition();
                ViewData["fields"] = (fields??new FieldBase[0]);
                return PartialView("~/Areas/Maintenance/Views/Parts/_OrganizationView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Position(bool isAjax = false)
        {
            if (isAjax)
            {
                FieldBase[] fields = positionService.GetPositionExtendFieldsDefition();
                ViewData["fields"] = (fields ?? new FieldBase[0]);
                return PartialView("~/Areas/Maintenance/Views/Parts/_PositionView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult CustomRole(bool isAjax = false)
        {
            if (isAjax)
            {
                //FieldBase[] fields = positionService.GetPositionExtendFieldsDefition();
                //ViewData["fields"] = (fields ?? new FieldBase[0]);
                return PartialView("~/Areas/Maintenance/Views/Parts/_CustomRoleView.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult ProcessSuperviseManage(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/ProcessSuperviseManage/Index.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult ProcessSupervise(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/ProcessSupervise/Index.cshtml");
            }
            return View("_SingPage");
            //return PartialView("~/Areas/Maintenance/Views/ProcessSuperviseManage/Index.cshtml");
        }
        public ActionResult DimissionManagement(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/DimissionRedirect/Index.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult MyProcessInstance(bool isAjax = false)
        {

            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/MyProcessInstance/Index.cshtml");
            }

            return View("_SingPage");
        }

        public ActionResult SIMList(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/SIMList/Index.cshtml");
            }
            return View("_SingPage");
            //return PartialView("~/Areas/Maintenance/Views/ProcessSuperviseManage/Index.cshtml");
        }

        public ActionResult Staff(bool isAjax = false)
        {
            if (isAjax)
            {                
                ViewBag.AuthType = this.AuthType;
                ViewBag.CultureName = ResxService.GetAvailableCulture();
                FieldBase[] fields = userService.GetUserExtendFieldsDefition();
                ViewData["fields"] = (fields ?? new FieldBase[0]);
                return PartialView("~/Areas/Maintenance/Views/Parts/_StaffView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Tenant(bool isAjax = false)
        {
            if (isAjax)
            {                            
                return PartialView("~/Areas/Maintenance/Views/Parts/_TenantView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Master(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_MasterView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Schedule(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_ScheduleView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Menu(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_MenuView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Role(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_RoleView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult EmailTmpl(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_EmailTemplate.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult App(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_AppBar.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult MenuBar(string key, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.key = key;
                return PartialView("~/Areas/Maintenance/Views/Parts/_MenuBar.cshtml");
            }
            return View("_SingPage");
        }
      
       public ActionResult Applications(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_ApplicationManagement.cshtml");
            }
            return View("_SingPage");
        }

       public ActionResult FieldExtend(bool isAjax = false)
       {
           if (isAjax)
           {
               return PartialView("~/Areas/Maintenance/Views/Parts/_FieldExtendManagement.cshtml");
           }
           return View("_SingPage");
       }

        public ActionResult ApplicationDetail(string key, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.key = key;
                return PartialView("~/Areas/Maintenance/Views/Parts/_ApplicationDetail.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Library(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_Library.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Document(string ID, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.DocumentLibraryID = ID;
                return PartialView("~/Areas/Maintenance/Views/Parts/_Document.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Portal(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_Portal.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult TDConfig(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_TDConfig.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult DataDictionary(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_DataDictionaryView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult ControlTemplate(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_ControlTemplateView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Delegation(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_DelegationView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult MyDrafts(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_MyDraftsView.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult MyFormCC(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_MyFormCCView.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult HaveRead(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_HaveReadView.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult WaitRead(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_WaitReadView.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult InsteadRequest(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_InsteadRequestView.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult AdminDelegation(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_AdminDelegationView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult Worklist(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_Worklist.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult WorklistData(Guid key, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.key = key;
                return PartialView("~/Areas/Maintenance/Views/Parts/_WorklistData.cshtml");
            }
            return View("_SingPage");
        }



        public ActionResult Authorization(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_Authorization.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult Permissions(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_Permissions.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult ResourcePermissions(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_ResourcePermissions.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult EditResourcePermission(string ID, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = ID;
                ViewBag.Title = "Edit ResourcePermission";
                return PartialView("~/Areas/Maintenance/Views/Parts/_EditResourcePermission.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult CreateResourcePermission(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "Create ResourcePermission";
                return PartialView("~/Areas/Maintenance/Views/Parts/_CreateResourcePermission.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult ProcessPermission(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "ProcessPermission";
                return PartialView("~/Areas/Maintenance/Views/Parts/_ProcessPermission.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult CommonReportPermission(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "CommonReportPermission";
                return PartialView("~/Areas/Maintenance/Views/Parts/_CommonReportPermission.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult EditAuthorization(string ID, bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = ID;
                ViewBag.Title = "Edit Authorization";
                return PartialView("~/Areas/Maintenance/Views/Parts/_EditAuthorization.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult CreateAuthorization(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "Create Authorization";
                return PartialView("~/Areas/Maintenance/Views/Parts/_CreateAuthorization.cshtml");
            }
            return View("_SingPage");
        }

        public JsonResult GetCurrentDatetime()
        {
            DateTime dt = Helper.UtilityHelper.GetCurrentDate();
            return Json(dt.ToString("yyyy/MM/dd HH:mm:ss"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogRequest(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/_LogRequestView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult FeaturePermission(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "FeaturePermission";
                return PartialView("~/Areas/Maintenance/Views/Parts/_FeaturePermission.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult FlowTheme(bool isAjax = false)
        {
            if (isAjax)
            {
                ViewBag.ID = "";
                ViewBag.Title = "FlowTheme";
                return PartialView("~/Areas/Maintenance/Views/Parts/_FlowThemeManage.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult CommonReportMaitance(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/CommonReportMait/Index.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult CommonReportSearchConfig(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/Parts/CommonReportSearchConfig.cshtml");
            }
            return View("_SingPage");
        }
        public ActionResult ProcessInstanceSign(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Maintenance/Views/ProcessInstanceManageAddSigner/_ManageInstance.cshtml");
            }
            return View("_SingPage");
        }
    }
}
