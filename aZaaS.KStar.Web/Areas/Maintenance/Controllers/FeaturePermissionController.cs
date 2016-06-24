using aZaaS.KStar.Web.Confing;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class FeaturePermissionController : BaseMvcController
    {
        //
        // GET: /Maintenance/FeaturePermission/

        public JsonResult GetFeatureTypeList()
        {
            Hashtable ht = ConfingUtilities.GetSingleConfing("_FeaturePermission.xml");
            List<Hashtable> listHt = new List<Hashtable>();
            foreach (var key in ht.Keys)
            {
                Hashtable tht = new Hashtable();
                tht.Add("key", key);
                tht.Add("value", ht[key]);
                listHt.Add(tht);
            }
            return Json(listHt, JsonRequestBehavior.AllowGet); ;
        }

        public JsonResult GetFeatureList(string type)
        {
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var code = IsCheck(type) ? type : "";


                var linq2 = from network in basisEntity.NetworkRoles
                            where  network.Code==code 
                            select new {
                                RoleID=network.RoleID,
                                Checked = "checked"
                            };
                            
                 var linq =from role in basisEntity.Roles 
                    join network in linq2
                      on role.SysId equals network.RoleID 
                           into networkRole 
                           from nRole in networkRole.DefaultIfEmpty()
                    select new
                    {
                        Name = role.Name,
                        Guid = role.SysId,
                        Checked = nRole.Checked == "checked" ? "checked" : string.Empty

                    };

                var dataList = linq.ToList();
                return Json(dataList, JsonRequestBehavior.AllowGet);
            } 
        }
        public JsonResult GetRoleList()
        { 
            using (BasisEntityContainer basisEntity = new BasisEntityContainer())
            {
                var linq = from role in basisEntity.Roles
                          // where role.Name.StartsWith("v_") == false
                           select new
                           {
                               Name = role.Name,
                               Guid = role.SysId
                           };
                var dataList = linq.ToList();
                return Json(dataList, JsonRequestBehavior.AllowGet);
            } 
        }
        [HttpPost]
        public JsonResult PostUpdateFeatureList(string featureList,string type)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(type))
                {
                    string[] guids = featureList.Split(',');
                    List<Guid> guidList = new List<Guid>();
                    if (!string.IsNullOrWhiteSpace(featureList))
                    {
                        foreach (string guid in guids)
                        {
                            guidList.Add(Guid.Parse(guid));
                        }
                    }
                  
                    var code = IsCheck(type) ? type : "";

                    using (BasisEntityContainer basisEntity = new BasisEntityContainer())
                    {
                        var deleteEntityList = basisEntity.NetworkRoles.Where(x => x.Code == code).ToList();
                        foreach (var guid in guidList)
                        {
                            NetworkRole entity = new NetworkRole();
                            entity.Code = code;
                            entity.RoleID = guid;
                            basisEntity.NetworkRoles.Add(entity);
                        }
                        basisEntity.NetworkRoles.RemoveRange(deleteEntityList);
                        basisEntity.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                return Json("{\"succeed\":false}", JsonRequestBehavior.AllowGet); 
            }

            return Json("{\"succeed\":true}", JsonRequestBehavior.AllowGet); 
        }


        public bool IsCheck(string type)
        {

            if (EnumCollection.FeaturePermissionCode.InternetDownloadRole.ToString() == type)
            {
                return true;
            }
            else if (EnumCollection.FeaturePermissionCode.InternetRole.ToString() == type)
            {
                return true;
            }
            return false;
        }
    }
}
