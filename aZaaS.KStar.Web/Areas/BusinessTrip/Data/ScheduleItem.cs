using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Data
{
    public partial class ScheduleItem
    {
        public int Id { get; set; }
        public Nullable<int> RequestId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public string Comment { get; set; }
        public virtual TravelRequest TravelRequest { get; set; }
    }
}
