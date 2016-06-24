using aZaaS.KStar.Form;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class CategoryTemplateController : Controller
    {
        //
        // GET: /Settings/ControlTemplateCategory/        
        private readonly ControlTmplProvider _controlTmplProvider;

        public CategoryTemplateController()
        {
            _controlTmplProvider = new ControlTmplProvider();
        }

        public ActionResult Index()
        {
            return PartialView("~/Areas/Maintenance/Views/Parts/_ControlTemplateView.cshtml");
        }

        [HttpPost]
        public JsonResult GetControlTemplate(int current, int rowCount, string categoryId = "", string categorytype = "", string searchPhrase = "")
        {
            int total;
            IList<ControlTemplateModel> controltemplate = new List<ControlTemplateModel>();
            if (string.IsNullOrEmpty(categoryId))
            {
                if (string.IsNullOrEmpty(searchPhrase))
                {
                    controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => true), out total);
                }
                else
                {
                    controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => x.DisplayName.Contains(searchPhrase) || x.HtmlTemplate.Contains(searchPhrase)), out total);
                }
            }
            else
            {
                //当前分类的Guid
                Guid tempid = Guid.Parse(categoryId);
                //存放子分类的Guid集合
                List<Guid> childs = new List<Guid>();
                if (categorytype == "Category")
                {
                    //获取所有分类
                    IList<TemplateCagetoryModel> categorys = _controlTmplProvider.GetAllCategories();
                    //获取当前分类下所有的子分类的Guid集合
                    _controlTmplProvider.GetCategoryChilds(categorys, tempid, childs);
                }

                if (string.IsNullOrEmpty(searchPhrase))
                {
                    if (categorytype == "Category")
                    {

                        controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => childs.Contains(x.CategoryId)), out total);
                    }
                    else
                    {
                        controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => x.SysId == tempid), out total);
                    }

                }
                else
                {
                    if (categorytype == "Category")
                    {
                        controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => childs.Contains(x.CategoryId) && (x.DisplayName.Contains(searchPhrase) || x.HtmlTemplate.Contains(searchPhrase))), out total);
                    }
                    else
                    {
                        controltemplate = _controlTmplProvider.GetTemplate(rowCount, current, (x => x.SysId == tempid && (x.DisplayName.Contains(searchPhrase) || x.HtmlTemplate.Contains(searchPhrase))), out total);
                    }
                }
            }
            IList<TemplateCagetoryModel> categorytemplates = _controlTmplProvider.GetAllCategories();

            foreach (var item in controltemplate)
            {
                if (item.CategoryId != Guid.Empty)
                {
                    TemplateCagetoryModel categorytemplate = categorytemplates.Where(x => x.SysId == item.CategoryId).FirstOrDefault();
                    item.CategoryName = categorytemplate.CategoryName;
                }
            }
            return Json(new { current = current, rowCount = rowCount, total = total, rows = controltemplate }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddControlTemplate(string jsonData)
        {
            ControlTemplateModel controltemplate = ControlTemplateModel.ByJSON(jsonData);
            _controlTmplProvider.AddTemplate(controltemplate);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetControlTemplateById(Guid sysid)
        {
            ControlTemplateModel controltemplate = _controlTmplProvider.GetTemplate(sysid);
            if (controltemplate.CategoryId != Guid.Empty)
            {
                TemplateCagetoryModel categorytemplate = _controlTmplProvider.GetCategory(controltemplate.CategoryId);
                controltemplate.CategoryName = categorytemplate.CategoryName;
            }
            return Json(controltemplate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult EditControlTemplate(string jsonData)
        {
            ControlTemplateModel formModel = ControlTemplateModel.ByJSON(jsonData);
            _controlTmplProvider.EditTemplate(formModel);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult DeleteControlTemplate(string jsonData)
        {
            TemplateCagetoryModel cagetorytemplate = TemplateCagetoryModel.ByJSON(jsonData);
            _controlTmplProvider.DeleteTemplate(cagetorytemplate.SysId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCategoryTypeTree(string id)
        {
            List<TreeModel> categorytree = new List<TreeModel>();
            IList<TemplateCagetoryModel> categorymodels = new List<TemplateCagetoryModel>();
            if (string.IsNullOrEmpty(id))
            {
                categorymodels = _controlTmplProvider.GetRootCategories();
            }
            else
            {
                categorymodels = _controlTmplProvider.GetChildCategories(Guid.Parse(id));
            }
            foreach (var item in categorymodels)
            {
                categorytree.Add(new TreeModel()
                {
                    ID = item.SysId.ToString(),
                    SysId = item.SysId,
                    ParentID = item.ParentId,
                    NodeName = item.CategoryName,
                    isParent = true
                });
            }
            return Json(categorytree, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddCategoryTemplate(string jsonData)
        {
            TemplateCagetoryModel cagetorytemplate = TemplateCagetoryModel.ByJSON(jsonData);
            Guid sysid = _controlTmplProvider.AddCategory(cagetorytemplate);
            return Json(new { flag = true, ID = sysid.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryTemplateById(Guid sysid)
        {
            TemplateCagetoryModel cagetorytemplate = _controlTmplProvider.GetCategory(sysid);
            return Json(cagetorytemplate, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditCategoryTemplate(string jsonData)
        {
            TemplateCagetoryModel cagetorytemplate = TemplateCagetoryModel.ByJSON(jsonData);
            _controlTmplProvider.EditCategory(cagetorytemplate);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCategoryTemplate(string jsonData)
        {
            TemplateCagetoryModel cagetorytemplate = TemplateCagetoryModel.ByJSON(jsonData);
            _controlTmplProvider.DeleteCategory(cagetorytemplate.SysId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCategoryTree(string id, string type)
        {
            List<TreeModel> categorytree = new List<TreeModel>();
            IList<TemplateCagetoryModel> categorymodels = new List<TemplateCagetoryModel>();
            IList<ControlTemplateModel> templatemodels = new List<ControlTemplateModel>();
            if (string.IsNullOrEmpty(id))
            {
                categorymodels = _controlTmplProvider.GetRootCategories();
            }
            else
            {
                categorymodels = _controlTmplProvider.GetChildCategories(Guid.Parse(id));
                templatemodels = _controlTmplProvider.GetCategoryTemplates(Guid.Parse(id));
            }
            if (categorymodels != null && categorymodels.Count > 0)
            {
                foreach (var item in categorymodels)
                {
                    categorytree.Add(new TreeModel()
                    {
                        ID = item.SysId.ToString(),
                        SysId = item.SysId,
                        ParentID = item.ParentId,
                        NodeName = item.CategoryName,
                        Type = "Category",
                        isParent = true
                    });
                }
            }
            if (templatemodels != null && templatemodels.Count > 0)
            {
                foreach (var item in templatemodels)
                {
                    categorytree.Add(new TreeModel()
                    {
                        ID = item.SysId.ToString(),
                        SysId = item.SysId,
                        ParentID = item.CategoryId,
                        NodeName = item.DisplayName,
                        Type = "Template",
                        isParent = false
                    });
                }
            }
            return Json(categorytree, JsonRequestBehavior.AllowGet);
        } 
    }
}
