using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    public class RoleInfo
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescr { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string TenantID { get; set; }

        private bool _isChecked = false;
        /// <summary>
        /// Only for UI Edit
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != this._isChecked)
                {
                    this._isChecked = value;
                }
            }
        }
    }
}
