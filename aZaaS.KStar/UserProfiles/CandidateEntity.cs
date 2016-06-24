using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.UserProfiles
{
    public class CandidateEntity
    {
        [Key, Column(Order = 0)]
        public virtual Guid SysId { get; set; }

        [Key, Column(Order = 1)]
        public virtual Guid PersonId { get; set; }

        public virtual string Type { get; set; }
    }
}
