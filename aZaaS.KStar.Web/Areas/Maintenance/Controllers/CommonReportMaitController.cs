using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.BasisEntity;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class CommonReportMaitController : Controller
    {
        //
        // GET: /Maintenance/CommonReportMait/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Find(int ProcessSetID)
        {
            int pageIndex = 0;
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int.TryParse(page, out pageIndex);
            int.TryParse(Size, out pageSize);
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                var result = bc.CommonReportConfig_DisplayArea.Where(p => p.ProcSetID == ProcessSetID).OrderBy(x => x.ID).Skip((pageIndex-1) * pageSize).Take(pageSize).ToList();
                var count = bc.CommonReportConfig_DisplayArea.Where(p => p.ProcSetID == ProcessSetID).ToList().Count;
                var output = new { data = result, total = count };
                return Json(output, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FindSearch(int ProcessSetID)
        {
            int pageIndex = 0;
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int.TryParse(page, out pageIndex);
            int.TryParse(Size, out pageSize);
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                var result = bc.CommonReportConfig_SearchArea.Where(p => p.ProcSetID == ProcessSetID).OrderBy(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var count = bc.CommonReportConfig_SearchArea.Where(p => p.ProcSetID == ProcessSetID).ToList().Count;
                var output = new { data = result, total = count };
                return Json(output, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EditReportDisplay(CommonReportConfig_DisplayArea model)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                var result = bc.CommonReportConfig_DisplayArea.SingleOrDefault(f => f.ID == model.ID);
                if (result != null)
                {
                    result.FieldID = model.FieldID;
                    result.FieldName = model.FieldName;
                    result.FieldType = model.FieldType;
                    result.DataResource = model.DataResource;
                    result.XPATH = model.XPATH;
                    result.ProcSetID = model.ProcSetID;
                    result.Memo = model.Memo;
                }
                bc.Entry(result).State = System.Data.Entity.EntityState.Modified;
                bc.SaveChanges();
                return Json("修改成功！", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EditReportSearch(CommonReportConfig_SearchArea model)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                var result = bc.CommonReportConfig_SearchArea.SingleOrDefault(f => f.ID == model.ID);
                if (result != null)
                {
                    result.FieldID = model.FieldID;
                    result.FieldName = model.FieldName;
                    result.FieldType = model.FieldType;
                    result.DataResource = model.DataResource;
                    result.XPATH = model.XPATH;
                    result.ProcSetID = model.ProcSetID;
                    result.Memo = model.Memo;
                }
                bc.Entry(result).State = System.Data.Entity.EntityState.Modified;
                bc.SaveChanges();
                return Json("修改成功！", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetReportDisplayAreaByID(string id)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                var result = bc.CommonReportConfig_DisplayArea.SingleOrDefault(f=>f.ID==Convert.ToInt32(id));
                return Json(result,JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult AddDisplay(CommonReportConfig_DisplayArea model)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                bc.CommonReportConfig_DisplayArea.Add(model);
                bc.SaveChanges();
                return Json("添加成功", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AddSearch(CommonReportConfig_SearchArea model)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                bc.CommonReportConfig_SearchArea.Add(model);
                bc.SaveChanges();
                return Json("添加成功", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteDisplay(List<string> idList)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                foreach (string s in idList)
                {
                    int ID = 0;
                    int.TryParse(s, out ID);
                    var result = bc.CommonReportConfig_DisplayArea.FirstOrDefault(f => f.ID == ID);
                    bc.CommonReportConfig_DisplayArea.Remove(result);
                    bc.SaveChanges();
                }
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteSearch(List<string> idList)
        {
            using (aZaaS.KStar.Web.Models.BasisEntity.BasisEntityContainer bc = new BasisEntityContainer())
            {
                foreach (string s in idList)
                {
                    int ID = 0;
                    int.TryParse(s, out ID);
                    var result = bc.CommonReportConfig_SearchArea.FirstOrDefault(f => f.ID == ID);
                    bc.CommonReportConfig_SearchArea.Remove(result);
                    bc.SaveChanges();
                }
                return Json("删除成功", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
