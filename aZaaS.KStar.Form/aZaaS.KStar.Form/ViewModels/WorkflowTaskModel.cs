using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace aZaaS.KStar.Form.ViewModels
{
    public class WorkflowTaskModel
    {
        public WorkflowTaskModel()
        {
            IsEnableSign = false;
            SignActionName = string.Empty;
            Actions = new List<string>();
            DataFields = new Dictionary<string, object>();
            ActivityDataFields = new Dictionary<string, object>();
        }

        public string Folio { get; set; }
        public string SerialNo { get; set; }
        public int ProcInstId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Data { get; set; }
        public string Destination { get; set; }

        public bool IsEnableSign { get; set; }
        public string SignActionName { get; set; }

        [JsonIgnore]
        public IList<string> Actions { get; set; }
        [JsonIgnore]
        public Dictionary<string,object> DataFields { get; set; }
        [JsonIgnore]
        public Dictionary<string,object> ActivityDataFields { get; set; }

        //TODO: DataFields etc...

    }
}
