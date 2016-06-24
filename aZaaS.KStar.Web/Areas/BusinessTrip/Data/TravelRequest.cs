using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Data
{
    public partial class TravelRequest
    {
        public TravelRequest()
        {
            this.ScheduleItems = new List<ScheduleItem>();
        }

        public int Id { get; set; }
        public Nullable<int> FormId { get; set; }
        public string Applicant { get; set; }
        public string ApplicantName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> BackDate { get; set; }
        public Nullable<System.Guid> DepartmentId { get; set; }
        public string DepartmetName { get; set; }
        public string Phone { get; set; }
        public string Entourage { get; set; }
        public Nullable<bool> IsUsingCar { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public string TravelReason { get; set; }
        public virtual ICollection<ScheduleItem> ScheduleItems { get; set; }
    }
}
