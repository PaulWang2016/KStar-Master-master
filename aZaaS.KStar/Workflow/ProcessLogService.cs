using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;

namespace aZaaS.KStar
{
    public class ProcessLogService
    {
        private readonly ProcessLogFacade processLogFacade;

        public ProcessLogService()
        {
            this.processLogFacade = new ProcessLogFacade();
        }


        ///<summary>
        ///新增流程操作日志
        ///</summary>
        ///<param name="log"></param>
        ///<returns></returns>
        public int AddProcessLog( ProcessLog log)
        {

            return this.processLogFacade.AddProcessLog(new ServiceContext(), log);
        }

        ///<summary>
        ///根据D获取流程操作日志
        ///</summary>
        ///<param name="id"></param>
        ///<returns></returns>
        public ProcessLog GetProcessLogByID( int id)
        {

            return this.processLogFacade.GetProcessLogByID(new ServiceContext(), id);
        }

        ///<summary>
        ///根据SN获取流程操作日志
        ///</summary>
        ///<param name="sn"></param>
        ///<returns></returns>
        public ProcessLog GetProcessLogBySN( string sn)
        {

            return this.processLogFacade.GetProcessLogBySN(new ServiceContext(), sn);
        }

        ///<summary>
        ///根据流程实例ID和关卡实例ID获取流程审批日志
        ///</summary>
        ///<param name="procInstID"></param>
        ///<param name="actInstID"></param>
        ///<returns></returns>
        public ProcessLog GetProcessLogByProcAndActID( int procInstID, int actInstID)
        {

            return this.processLogFacade.GetProcessLogByProcAndActID(new ServiceContext(), procInstID, actInstID);
        }

        ///<summary>
        ///根据流程实例ID获取流程审批日志
        ///</summary>
        ///<param name="procInstID"></param>
        ///<returns></returns>
        public List<ProcessLog> GetProcessLogByProcInstID( int procInstID)
        {

            return this.processLogFacade.GetProcessLogByProcInstID(new ServiceContext(), procInstID);
        }

        ///<summary>
        ///根据流程ID和操作类型获取流程日志
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="opType"></param>
        ///<returns></returns>
        public List<ProcessLog> GetProcessLogByOpType( int procInstID, OpType opType)
        {

            return this.processLogFacade.GetProcessLogByOpType(new ServiceContext(), procInstID, opType);
        }

        /// <summary>
        /// 根据用户账号获取流程日志
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public List<ProcessLog> GetProcessLogByUserAccount(string userAccount)
        {
            return this.processLogFacade.GetProcessLogByUserAccount(new ServiceContext(), userAccount);
        }

        ///<summary>
        ///根据criteria获取流程日志
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="processName"></param>
        ///<returns></returns>
        public List<ProcessLog> GetProcessLogs( PageCriteria criteria)
        {

            return this.processLogFacade.GetProcessLogs(new ServiceContext(), criteria);
        }

        ///<summary>
        ///根据Id删除流程日志
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="id"></param>
        public bool DeleteProcessLogByID( int id)
        {

            return this.processLogFacade.DeleteProcessLogByID(new ServiceContext(), id);
        }

        ///<summary>
        ///根据流程实例Id删除流程审批日志
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        public bool DeleteProcessLogByProcInstID( int procInstID)
        {

            return this.processLogFacade.DeleteProcessLogByProcInstID(new ServiceContext(), procInstID);
        }
    }
}
