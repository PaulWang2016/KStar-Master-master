using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework.Extensions;

namespace aZaaS.KStar.UserManagement
{
    public static class UserDTOExtension
    {
        public static int JobRank(this UserDTO user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //UserExFieldDTO field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobRank"));

            //return field.Null() ? 0 : (int)(field.ValueNumber ?? 0);
            return 0;
        }

        public static string JobTitle(this UserDTO user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //UserExFieldDTO field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobTitle"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public static string JobClass(this UserDTO user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //UserExFieldDTO field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobClass"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public static string Department(this UserDTO user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //UserExFieldDTO field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("Department"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public static bool IsOnPosition(this UserDTO user,string positionName)
        {
            user.NullThrowArgumentEx("user is not assigned.");
            positionName.NullOrEmptyThrowArgumentEx("position name is not assigned.");

            return user.Positions.Any(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
        }

    }
}
