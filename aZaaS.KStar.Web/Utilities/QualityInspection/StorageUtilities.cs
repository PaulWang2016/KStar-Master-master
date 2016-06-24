using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities.QualityInspection
{
    public class StorageUtilities
    {
        /// <summary>
        /// 查询表Neoway_BaseData_QualityFactory的所有数据
        /// </summary>
        /// <returns></returns>
         public static List<Neoway_BaseData_QualityFactory> GetFactoryBaseData()
        {
            using (var edm=new BasisEntityContainer())
            {
                return edm.Neoway_BaseData_QualityFactory.ToList();
            }
        }
    }
}