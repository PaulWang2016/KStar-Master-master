using aZaaS.KStar.Wcf.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDelegateService" in both code and config file together.
    [ServiceContract]
    public interface IDelegateService
    {
        /// <summary>
        /// 查询代理列表
        /// </summary>
        /// <param name="delegateInfo">代理属性过滤</param>        
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">当前页，第一页=0</param>
        /// <param name="isOverdue">是否过期</param>
        /// <returns></returns>
        [OperationContract]
        List<DelegateInfo> GetDelegations(DelegateInfo delegateInfo, ref  PagerInfo pager, bool isOverdue);
        /// <summary>
        /// 删除代理
        /// </summary>
        /// <param name="id">代理id</param>
        /// <returns></returns>
        [OperationContract]
        bool DeleteDelegation(int id);
        /// <summary>
        /// 禁用/启用代理
        /// </summary>
        /// <param name="id">代理id</param>
        /// <param name="status">启用=true，禁用=false</param>
        /// <returns></returns>
        [OperationContract]
        bool SetDelegationStatus(int id, bool status, string updatedBy);
        /// <summary>
        /// 创建新代理
        /// </summary>
        /// <param name="delegateInfo">代理信息</param>
        [OperationContract]
        bool CreateDelegation(string tenantId, string userName, DelegateInfo delegateInfo, List<string> delegatetoList, List<string> processList);
    }
}
