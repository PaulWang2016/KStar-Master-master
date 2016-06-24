using aZaaS.KStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Helper;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Extend;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class FieldExtendController : Controller
    {
        private static readonly FxExtendService _fxextendService = new FxExtendService();

        public JsonResult GetFxExtend()
        {
            List<FieldExtendEntity> list = new List<FieldExtendEntity>();
            IEnumerable<Fx_ExtendDto> fields = _fxextendService.GetAllFxExtends();
            foreach (var item in fields)
            {
                FieldExtendBaseDto[] fielditems = new FieldExtendBaseDto[item.fields.Length];
                int i = 0;
                foreach (var fitem in item.fields)
                {
                    fielditems[i] = new FieldExtendBaseDto();
                    if (fitem.GetType().Name == "ChooseField")
                    {
                        fielditems[i].Source = ((ChooseField)fitem).Source;
                    }
                    fielditems[i].DefalutValue = fitem.DefalutValue;
                    fielditems[i].Description = fitem.Description;
                    fielditems[i].DisplayName = fitem.DisplayName;
                    fielditems[i].Name = fitem.Name;
                    fielditems[i].Validators = fitem.Validators;
                    fielditems[i].Value = fitem.Value;
                    fielditems[i].FieldType = fitem.GetType().Name;
                    i++;
                }
                list.Add(new FieldExtendEntity() { SysId = item.SysID, Name = item.Name, fields = fielditems });
            }


            return Json(list.OrderBy(o => o.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateFieldExtend(string Name)
        {
            _fxextendService.CreateFxExtend(Name, new FieldBase[0]);
            Fx_ExtendDto fxd = _fxextendService.GetFxExtends(Name);

            FieldExtendBaseDto[] fielditems = new FieldExtendBaseDto[fxd.fields.Length];
            int i = 0;
            foreach (var fitem in fxd.fields)
            {
                fielditems[i] = new FieldExtendBaseDto();
                if (fitem.GetType().Name == "ChooseField")
                {
                    fielditems[i].Source = ((ChooseField)fitem).Source;
                }
                fielditems[i].DefalutValue = fitem.DefalutValue;
                fielditems[i].Description = fitem.Description;
                fielditems[i].DisplayName = fitem.DisplayName;
                fielditems[i].Name = fitem.Name;
                fielditems[i].Validators = fitem.Validators;
                fielditems[i].Value = fitem.Value;
                fielditems[i].FieldType = fitem.GetType().Name;
                i++;
            }
            FieldExtendEntity item = new FieldExtendEntity() { SysId = fxd.SysID, Name = fxd.Name, fields = fielditems };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFieldExtend(string Name)
        {
            Fx_ExtendDto fxd = _fxextendService.GetFxExtends(Name);
            _fxextendService.DeleteFxExtend(Name);
            return Json(fxd.SysID, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateFieldExtend(Guid SysId, string Name, [ModelBinder(typeof(JsonListBinder<FieldExtendBaseDto>))]IList<FieldExtendBaseDto> fieldlist)
        {
            FieldBase[] fields = new FieldBase[fieldlist.Count];
            //获取原来扩展定义
            Fx_ExtendDto fx_extend = _fxextendService.GetFxExtends(Name); 
            int i = 0;
            foreach (var item in fieldlist)
            {
                switch (item.FieldType)
                {
                    case "TextField":
                        TextField tf = new TextField() { DefalutValue = item.DefalutValue, Description = item.Description, DisplayName = item.DisplayName, Name = item.Name, Value = item.Value };
                        fields[i] = tf;
                        i++;
                        break;
                    case "DateField":
                        DateField df = new DateField() { DefalutValue = item.DefalutValue, Description = item.Description, DisplayName = item.DisplayName, Name = item.Name, Value = item.Value };
                        fields[i] = df;
                        i++;
                        break;
                    case "YesNoField":
                        YesNoField ynf = new YesNoField() { DefalutValue = item.DefalutValue, Description = item.Description, DisplayName = item.DisplayName, Name = item.Name, Value = item.Value };
                        fields[i] = ynf;
                        i++;
                        break;
                    case "ChooseField":
                        ChooseField cf = new ChooseField() { DefalutValue = item.DefalutValue, Description = item.Description, DisplayName = item.DisplayName, Name = item.Name, Value = item.Value, Source = item.Source };
                        fields[i] = cf;
                        i++;
                        break;
                    case "NumericField":
                        NumericField nf = new NumericField() { DefalutValue = item.DefalutValue, Description = item.Description, DisplayName = item.DisplayName, Name = item.Name, Value = item.Value };
                        fields[i] = nf;
                        i++;
                        break;
                }
            }
            _fxextendService.UpdateFxExtend(Name, fields);
            //如果有删除，则更新各个扩展表中的数据                
            foreach (var item in fx_extend.fields)
            {
                var filed = fieldlist.Where(x => x.Name == item.Name).FirstOrDefault();
                if (filed==null)
                {
                    _fxextendService.DeleteExtendField(Name, item.Name);
                }
            }
         
            FieldExtendEntity fitem = new FieldExtendEntity() { SysId = SysId, Name = Name, fields = fieldlist.ToArray() };
            return Json(fitem, JsonRequestBehavior.AllowGet);
        }
    }

    public class FieldExtendEntity
    {
        public Guid SysId { get; set; }
        public string Name { get; set; }
        public FieldExtendBaseDto[] fields { get; set; }
    }
}
