using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.SuperAdmin
{
    public class SuperADEntity
    {
        [Key]
        public Guid UserID { get; set; }
    }
}
