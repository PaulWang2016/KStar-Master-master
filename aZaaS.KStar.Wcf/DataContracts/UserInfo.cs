using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    public class UserInfo
    {
        public string ProfileID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DisplayName { get; set; }

        
        /// <summary>
        /// 工号
        /// </summary>
        public string WorkID { get; set; }
        
        public string Email { get; set; }
        public string MobilePhone { get; set; }

        /// <summary>
        /// 直属主管ID
        /// </summary>
        public string ManagerID { get; set; }

        public UserInfo Manager { get; set; }

        
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompID { get; set; }
        public string CompName { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptID { get; set; }
        public string DeptName { get; set; }
       

        public string PostID { get; set; }
        public string PostName { get; set; }
        

        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZoneID { get; set; }
        /// <summary>
        /// 行事历
        /// </summary>
        public string CalendarID { get; set; }
       


        public bool IsDefault { get; set; }
        public string OrgVersionID { get; set; }
        public string VersionName { get; set; }
        public string OrgID { get; set; }
        public string OrgName { get; set; }

        public string TenantID { get; set; }

    }

   
}
