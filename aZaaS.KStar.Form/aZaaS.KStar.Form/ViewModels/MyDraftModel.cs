using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class MyDraftModel
    {
        public int FormId { get; set; }

        public string FormSubject { get; set; }

        public string ProcessName { get; set; }

        public string DraftUrl { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
