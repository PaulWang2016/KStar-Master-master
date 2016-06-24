using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities.JSRSaleContract
{
    public class SaleContractStag
    {
        /// <summary>
        /// 获取合同号和序号
        /// </summary>
        /// <param name="customerBrand"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool GetContractNoStr(string country, string contractBelong, out string contractNo, out string no, string customerBrand = null, string model = null)
        {

            var codeBelong = string.Empty;
            switch (contractBelong)
            {
                case "网销":
                    codeBelong = "W";
                    break;
                default:
                    codeBelong = "J";
                    break;
            }
            using (var edm = new BasisEntityContainer())
            {
                int contractCount = 1;
                var code = edm.NeoWay_BaseData_CustomerBrand.FirstOrDefault(c => c.Country == country).ISOCode;
                var year = DateTime.Today.Year.ToString().Substring(2);
                var month = DateTime.Today.Month.ToString().PadLeft(2, '0');
                var entity = edm.NeoWay_BaseData_JSRSaleContract.Where(c=>c.Country==country).OrderByDescending(c => c.Id).FirstOrDefault();
                if (entity != null)
                {
                    var contract = entity.ContractNo; ;
                    if (month == contract.Substring(8, 2))
                    {
                        contractCount = int.Parse(contract.Substring(10, 3)) + 1;
                    }
                    
                }
                contractNo = "000" + code + year + month + contractCount.ToString().PadLeft(3, '0') + codeBelong;
                var count = edm.NeoWay_BaseData_JSRSaleContract.Count(c => c.CustomerBrand == customerBrand && c.Model == model) + 1;
                no = "E" + count.ToString().PadLeft(3, '0');
                return true;
            }
        }
        /// <summary>
        /// 获取合同变更列表
        /// </summary>
        /// <param name="contractNo"></param>
        /// <returns></returns>
        public static List<NeoWay_BaseData_JSRSaleContractChgHis> GetContractChangedHis(string contractNo,int type)
        {
            using (var edm = new BasisEntityContainer())
            {
                return edm.NeoWay_BaseData_JSRSaleContractChgHis.Where(c => c.ContractNo == contractNo && c.Type==type).OrderBy(c => c.ChangeCount).ToList();
            }
        }
        public static FormAttachment GetAttachmentInfo(Guid fileGuid, string formId)
        {
            using (var edm = new BasisEntityContainer())
            {
                return edm.FormAttachments.FirstOrDefault(c => c.FileGuid == fileGuid);

            }
        }
        public static int InsertFormAttachment(FormAttachment model)
        {
            using (var edm = new BasisEntityContainer())
            {
                edm.FormAttachments.Add(model);
                return edm.SaveChanges();

            }
        }
        public static int GetChangedCount(string contractNo, int type)
        {
            using (var edm = new BasisEntityContainer())
            {
                return edm.NeoWay_BaseData_JSRSaleContractChgHis.Count(c => c.ContractNo == contractNo && c.Type == type) + 1;
            }
        }
        public static int InsertChangedHis(NeoWay_BaseData_JSRSaleContractChgHis model)
        {
            using (var edm = new BasisEntityContainer())
            {
                edm.NeoWay_BaseData_JSRSaleContractChgHis.Add(model);
                return edm.SaveChanges();
            }
        }
        public static int InsertContractInfo(NeoWay_BaseData_JSRSaleContract model)
        {
            using (var edm = new BasisEntityContainer())
            {
                edm.NeoWay_BaseData_JSRSaleContract.Add(model);
                return edm.SaveChanges();
            }
        }
        public static int UpdateContractInfo(int id, string sparesPayment, string formid, string sparesContractData)
        {
            using (var edm = new BasisEntityContainer())
            {
                var model = edm.NeoWay_BaseData_JSRSaleContract.FirstOrDefault(c => c.Id == id);
                model.SparesPayment = sparesPayment;
                model.SparesFormId = formid; ;
                model.SparesData = sparesContractData;
                return edm.SaveChanges();
            }
        }
        public static NeoWay_BaseData_JSRSaleContract GetContractInfo(string contractNo)
        {
            using (var edm = new BasisEntityContainer())
            {
                return edm.NeoWay_BaseData_JSRSaleContract.FirstOrDefault(c => c.ContractNo == contractNo);

            }
        }
        public static int UpdateContractStatus(string contractNo, int status,bool? isSpares=null)
        {
            using (var edm = new BasisEntityContainer())
            {
                var model = edm.NeoWay_BaseData_JSRSaleContract.FirstOrDefault(c => c.ContractNo == contractNo);
                if (model != null)
                {
                    model.Status = status;
                    if (isSpares != null) model.IsSpares = isSpares;

                }
                return edm.SaveChanges();

            }
        }
        public static NeoWay_BaseData_JSRSaleContract GetContractStatusFreezing(string customerBrand, string model)
        {
            using (var edm = new BasisEntityContainer())
            {
                return edm.NeoWay_BaseData_JSRSaleContract.FirstOrDefault(c => c.Status == 0&&c.CustomerBrand==customerBrand&&c.Model==model);///0表示冻结

            }
        }
    }
}