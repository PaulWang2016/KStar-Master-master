using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Depository.Models
{
    public class DepositRequestModel
    {
        public DepositRequestModel()
        {
            ReturnDate = DateTime.Now;
            Comment = string.Empty;
            Services = new List<string>();
            //MemberType = "RespectfulMember";
        }

        public int Id { get; set; }
        public int FormId { get; set; }

        public string Customer { get; set; }
        public string Phone { get; set; }
        public string ItemType { get; set; }
        public string ReturnType { get; set; }
        public string Proof { get; set; }
        public DateTime ReturnDate { get; set; }
        public IList<string> Services { get; set; }
        public string MemberType { get; set; }
        public string Comment { get; set; }
    }
}