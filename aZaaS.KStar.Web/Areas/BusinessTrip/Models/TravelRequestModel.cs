using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Models
{
    public class TravelRequestModel
    {
        public TravelRequestModel()
        {
            this.Schedules = new List<ScheduleItemModel>();
            IsUsingCar = true;
            StartDate = DateTime.Now;
            BackDate = DateTime.Now;
        }

        public int Id { get; set; }
        public int FormId { get; set; }

        public string Applicant { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Entourage { get; set; }
        public string TravelReason { get; set; }
        public bool IsUsingCar { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime BackDate { get; set; }

        public IList<ScheduleItemModel> Schedules { get; set; }
        public int TotalDays { get; set; }

    }
}