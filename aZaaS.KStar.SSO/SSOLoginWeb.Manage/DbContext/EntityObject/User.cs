
namespace SSOLoginWeb.Manage.DbContext.EntityObject
{
    using System;
    using System.Collections.Generic;
    
    public  class User
    { 
        public System.Guid SysId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}
