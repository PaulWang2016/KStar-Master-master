using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Models;

namespace aZaaS.KStar.Form.ViewModels
{
    public class KStarFormModel 
    {

        public KStarFormModel()
        {
            //FormSubject = string.Empty;
            //ProcessFolio = string.Empty;
            //SubmitComment = string.Empty; 
        }

        public static KStarFormModel Instance(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                throw new ArgumentNullException("jsonData");

            var content = string.Empty;
            KStarFormModel formModel = null;

            jsonData = JsonHelper.Remove(jsonData, "ContentData", ref content);

            formModel = JsonHelper.ConvertToModel<KStarFormModel>(jsonData);
            formModel.ContentData = content;

            return formModel;
        }

        public string Json()
        {
            return JsonHelper.SerializeObject(this);
        }

        public int FormId { get; set; }
        public string FormSubject { get; set; }
        public string ActivityName { get; set; }
        public int ActivityId { get; set; }
        public int ProcInstId { get; set; }
        public string ProcessFolio { get; set; }
        public int Priority { get; set; }
        public DateTime SubmitDate { get; set; }
        public string SubmitterAccount { get; set; }
        public string SubmitterDisplayName { get; set; }
        public string ApplicantAccount { get; set; }
        public string ApplicantDisplayName { get; set; }
        public string ApplicantTelNo { get; set; }
        public string ApplicantEmail { get; set; }
	    public string ApplicantPositionID { get; set;} 
	    public string ApplicantPositionName { get; set;}
	    public string ApplicantOrgNodeID { get; set;}
	    public string ApplicantOrgNodeName { get; set;}
        public string SubmitComment { get; set; }
        public bool IsDraft { get; set; }
        public string DraftUrl { get; set; }
        
        [JsonIgnore]
        public string ContentData { get; set; }

        public ToolbarActionModel Toolbar { get; set; }
        public IList<AttachmentModel> Attachments { get; set; }
        public IList<ProcessLogModel> ProcessLogs { get; set; }
    }
}
