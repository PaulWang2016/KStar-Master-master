 
namespace aZaaS.KStar.ProcessForm
{
    using System;
    using System.Collections.Generic;
    
    public partial class view_ProcinstList
    {
        public int ProcInstID { get; set; }
        public System.DateTime StartDate { get; set; }
        public byte Status { get; set; }
        public string Originator { get; set; }
        public string Folio { get; set; }
        public int ProcID { get; set; }
        public System.DateTime FinishDate { get; set; }
        public string ActName { get; set; }
        public Nullable<System.DateTime> TaskStartDate { get; set; }
        public string Destination { get; set; }
        public Nullable<int> ActID { get; set; }
        public string SN { get; set; }
        public string StartName { get; set; }

        public string FullName { get; set; }
    }
}
