using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.LogRequestProvider
{
    public class LogRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }


        public string RequestUrl
        {
            get;
            set;
        }

        public string RequestType
        {
            get;
            set;
        }
 
        public string Parameters
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }


        public string Details
        {
            get;
            set;
        }


        public string IPAddress
        {
            get;
            set;
        }

        public string RequestUser
        {
            get;
            set;
        }

        public DateTime RequestTime
        {
            get;
            set;
        }
    }
}
