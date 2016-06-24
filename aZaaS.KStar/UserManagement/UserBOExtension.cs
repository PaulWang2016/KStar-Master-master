using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework.Extensions;

namespace aZaaS.KStar.UserManagement
{
    public static class UserBOExtension
    {
        public static UserDTO GetUserOwnerEqualsJobTitle(this UserBO userBO, string userName, string jobTitle)
        {
            //var satisfiedOwnerList = new List<UserDTO>();

            //userName.NullOrEmptyThrowArgumentEx("user name is not assigned.");
            //jobTitle.NullOrEmptyThrowArgumentEx("job title is not assigned.");

            //UserDTO user = userBO.ReadUser(userName);
            //user.NullThrowArgumentEx("can not found the specified user.");

            //RecursiveOwnersEqualsJobTitle(ref satisfiedOwnerList, user, jobTitle);

            //return satisfiedOwnerList.FirstOrDefault();
            return null;
        }
        
        public static UserDTO GetUserOwnerEqualsJobClass(this UserBO userBO, string userName, string jobClass)
        {
            //var satisfiedOwnerList = new List<UserDTO>();

            //userName.NullOrEmptyThrowArgumentEx("user name is not assigned.");
            //jobClass.NullOrEmptyThrowArgumentEx("job class is not assigned.");

            //UserDTO user = userBO.ReadUser(userName);
            //user.NullThrowArgumentEx("can not found the specified user.");

            //RecursiveOwnersEqualsClass(ref satisfiedOwnerList, user, jobClass);

            //return satisfiedOwnerList.FirstOrDefault();
            return null;
        }

        public static UserDTO GetUserOwnerContainsJobClass(this UserBO userBO, string userName, string jobClass)
        {
            //var satisfiedOwnerList = new List<UserDTO>();

            //userName.NullOrEmptyThrowArgumentEx("user name is not assigned.");
            //jobClass.NullOrEmptyThrowArgumentEx("job class is not assigned.");

            //UserDTO user = userBO.ReadUser(userName);
            //user.NullThrowArgumentEx("can not found the specified user.");

            //RecursiveOwnersContainsClass(ref satisfiedOwnerList, user, jobClass);

            //return satisfiedOwnerList.FirstOrDefault();
            return null;
        }


        private static void RecursiveOwnersEqualsJobTitle(ref List<UserDTO> expectedOwnerList, UserDTO currentUser, string requiredJobTitle)
        {
            //if (currentUser.NotNull() && currentUser.ReportTo.NotNullOrEmpty())
            //{
            //    foreach (UserDTO currentOwner in currentUser.ReportTo)
            //    {
            //        string currentOwnerJobTitle = currentOwner.JobTitle();
            //        if (currentOwnerJobTitle.Equals(requiredJobTitle, StringComparison.OrdinalIgnoreCase))
            //        {
            //            expectedOwnerList.Add(currentOwner);
            //            break;//We would stop searching once we got the satisfied owner. 
            //        }
            //        else
            //        {
            //            RecursiveOwnersEqualsJobTitle(ref expectedOwnerList, currentOwner, requiredJobTitle);
            //        }
            //    }
            //}
        }

        private static void RecursiveOwnersEqualsClass(ref List<UserDTO> expectedOwnerList, UserDTO currentUser, string requiredJobClass)
        {
            //if (currentUser.NotNull() && currentUser.ReportTo.NotNullOrEmpty())
            //{
            //    foreach (UserDTO currentOwner in currentUser.ReportTo)
            //    {
            //        string currentOwnerJobClass = currentOwner.JobClass();
            //        if (currentOwnerJobClass.Equals(requiredJobClass, StringComparison.OrdinalIgnoreCase))
            //        {
            //            expectedOwnerList.Add(currentOwner);
            //            break;//We would stop searching once we got the satisfied owner. 
            //        }
            //        else
            //        {
            //            RecursiveOwnersEqualsClass(ref expectedOwnerList, currentOwner, requiredJobClass);
            //        }
            //    }
            //}
        }

        private static void RecursiveOwnersContainsClass(ref List<UserDTO> expectedOwnerList, UserDTO currentUser, string requiredJobClass)
        {
            //if (currentUser.NotNull() && currentUser.ReportTo.NotNullOrEmpty())
            //{
            //    foreach (UserDTO currentOwner in currentUser.ReportTo)
            //    {
            //        string currentOwnerJobClass = currentOwner.JobClass();
            //        if (requiredJobClass.Contains(currentOwnerJobClass))
            //        {
            //            expectedOwnerList.Add(currentOwner);
            //            break;//We would stop searching once we got the satisfied owner. 
            //        }
            //        else
            //        {
            //            RecursiveOwnersContainsClass(ref expectedOwnerList, currentOwner, requiredJobClass);
            //        }
            //    }
            //}
        }
    }

}
