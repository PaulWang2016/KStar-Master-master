using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Models
{
    public class ScheduleItemModel
    {
        public ScheduleItemModel()
        {
        }

        public int Id { get; set; }
        public int RequestId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public string Comment { get; set; }
    }
}