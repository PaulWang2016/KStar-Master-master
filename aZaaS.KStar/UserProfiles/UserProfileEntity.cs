using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aZaaS.KStar.UserProfiles
{
    public class UserProfileEntity
    {
        public virtual Guid UserId { get; set; }

        public virtual string Key { get; set; }

        public virtual string Value { get; set; }
    }

}
