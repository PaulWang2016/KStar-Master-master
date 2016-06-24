using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Form.Models;

namespace aZaaS.KStar.CustomRole
{
    public class CustomRoleContext : MarshalByRefObject
    {
        public int ProcInstID { get; set; }

        public string Requester { get; set; }

        public Guid Key { get; set; }

        public string ProcessFullName { get; set; }

        public string ActivityName { get; set; }

        public int FormId { get; set; }

        public ProcessFormHeader ProcessFormHeader { get; set; }

        public ProcessFormContent ProcessFormContent { get; set; }

        public Dictionary<string, string> FormInfo { get; set; }

    }
}
