using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.WorkflowConfiguration
{
    public class Configuration_ProcInstDesc : Configuration_ActDesc
    {

        public int ProcInstID { set; get; }

        public byte Status { set; get; }


        public int ProcID { set; get; }
         
        public string FullName { set; get; }

        public int ProcSetID { set; get; }
        /// <summary>
        /// 自循环备注
        /// </summary>
        public string LoopRemark { set; get; }
    }

    public class Configuration_ActDesc
    {
        public int ActID { set; get; }
        public string Name { set; get; }

        public string SourceName { set; get; }

        public string LineName { set; get; }

        public string UserNames { set; get; }
    }
}
