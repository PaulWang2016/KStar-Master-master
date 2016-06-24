using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace aZaaS.KStar.Wcf
{
 
    public class LoginResult
    {
        public string UserID { get; set; }
        public string TenantID { get;  set; }
        
        private string mErrorMsg = "";
        public string ErrorMsg
        {
            get
            {
                return this.mErrorMsg;
            }
             set
            {
                this.mErrorMsg = value;
            }
        }
    }
}
