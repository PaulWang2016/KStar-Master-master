using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities.SIM
{
    public class SimMgr
    {
        /// <summary>
        /// sim借用流程 借用成功之后更新NW_SIMManagement表
        /// </summary>
        /// <param name="longNumber">长号</param>
        /// <param name="borrower">借用人</param>
        /// <param name="borrowerUserName">借用人工号</param>
        /// <param name="borrowerDate">借用日期</param>
        /// <param name="dept">借用人部门</param>
        /// <returns></returns>
        public static int UpdateBorrowerSimMgr(string longNumber, string borrower, string borrowerUserName, string borrowerDate, string dept)
        {
            using (var edm = new BasisEntityContainer())
            {
                var model = edm.NW_SIMManagement.FirstOrDefault(c => c.LongNumber == longNumber);
                if (model != null)
                {
                    model.Borrower = borrower;
                    model.BorrowerUserName = borrowerUserName;
                    model.BorrowDate = borrowerDate;
                    model.BorrowDept = dept;
                    model.SIMStatus = "Using";
                    model.Validate = StatusEnum.Available.ToString();
                }
                return edm.SaveChanges();
            }
        }
        public static int UpdateFormData(string longNumber, string borrower, string borrowerUserName, string borrowerDate, string dept)
        {
            using (var edm = new BasisEntityContainer())
            {
                var model = edm.NW_FormData.FirstOrDefault(c => c.JsonData.Contains(longNumber));
                if(model!=null)
                {
                    var FormModel = aZaaS.KStar.Form.ViewModels.KStarFormModel.Instance(model.JsonData);
                    var Model = JsonHelper.ConvertToModel<aZaaS.KStar.Web.Areas.JSRSIMManagement.Models.JSRSIMManagementModel>(FormModel.ContentData);
                    Model.FormId = (int)model.FormID;
                    Model.Borrower = borrower;
                    Model.BorrowerUserName = borrowerUserName;
                    Model.BorrowDept = dept;
                    Model.SIMStatus = "Using";
                    Model.BorrowDate = borrowerDate;
                    var jsonData = JsonHelper.SerializeObject(Model);
                    var Data = JsonHelper.SerializeObject(FormModel);
                    model.JsonData = Data.TrimEnd('}') + ",\"ContentData\":" + jsonData + "}"; 

                }
                return edm.SaveChanges();
            }
        }
        /// <summary>
        /// 更新SIM卡状态
        /// </summary>
        /// <param name="longNumber"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        public static int UpdateStatus(string longNumber,string available)
        {
            using (var edm = new BasisEntityContainer())
            {
                var model = edm.NW_SIMManagement.FirstOrDefault(c => c.LongNumber == longNumber);
                if(model!=null)
                {
                    model.Validate = available;
                    model.C_FormId = string.Empty;
                }
                return edm.SaveChanges();
            }
        }

    }
}