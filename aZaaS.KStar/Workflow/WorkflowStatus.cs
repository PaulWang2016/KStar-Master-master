using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar
{
   public static  class WorkflowStatus
    {
        public static string Map(int status, int procInstId)
        {
            //GroupName	StatusID	Status
            //Process   	0	Error     
            //Process   	1	Running   
            //Process   	2	Active    
            //Process   	3	Completed 
            //Process   	4	Stopped   
            //Process   	5	Deleted   

            //TODO 使用RM集中管理，并且要支持多语言
            if (status == 3)
            {
                status = GetFlowStatus(status, procInstId);
            }

            switch (status)
            {
                case 0:
                    return "错误";
                case 1:
                    return "运行中";
                case 2:
                    return "进行中";
                case 3:
                    return "已完成";
                case 4:
                    return "已停止";
                case 5:
                    return "作废";
                default:
                    break;
            }

            return "Unkown";
        }

        private static int GetFlowStatus(int status, int procInstId)
        {
            var flowStatus = status;

            using (KStarFramekWorkDbContext context = new KStarFramekWorkDbContext())
            {
                var item = context.ProcessFormCancel.FirstOrDefault(r => r.ProcInstId == procInstId);
                if (item != null)
                {
                    flowStatus = item.Status;
                }
            }

            return flowStatus;
        }
    }
}
