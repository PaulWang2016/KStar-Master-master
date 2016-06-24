using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.WorkflowData
{
    public class ChangeLogEntity : AbstractEntity
    {
        public string FieldName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedName { get; set; }
        public string OrderBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("RequestMainInfo")]
        public Guid FK_Form_ID { get; set; }

        public RequestMainInfoEntity RequestMainInfo { get; set; }
    }
}