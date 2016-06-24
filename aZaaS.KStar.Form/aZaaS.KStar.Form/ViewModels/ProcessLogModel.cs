using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class ProcessLogModel
    {
        public int SysId { get; set; }
        public int ProcInstID { get; set; }    
        public string ProcessName { get; set; }    
        public DateTime CreatedDate { get; set; }      
        public string TaskOwner { get; set; }       
        public string TaskOwnerName { get; set; }     
        public string ActionTaker { get; set; }        
        public string ActionTakerName { get; set; }       
        public string ActivityName { get; set; }  
        public string ActionName { get; set; }
        public string Comment { get; set; }

        public string PostSysID { get; set; }

        public string PostName { get; set; }
    }
}
