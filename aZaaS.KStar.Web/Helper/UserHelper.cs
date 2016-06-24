using aZaaS.KStar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Configuration;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.MgmtServices;

namespace System.Web.Mvc
{
    public static class UserHelper
    {
        //private readonly static UserBO userBO = new UserBO();

        public static string CurrentUser(this HttpContextBase context)
        {
            string userName = context.User.Identity.Name;

            return userName;
        }

        public static Guid CurrentRequesterID(this HttpContextBase context)
        {
            string userName = context.User.Identity.Name;
            UserBO userBO = new UserBO();
            //UserWithRelationshipsDto user = userBO.ReadUser(userName);
            var user = userBO.ReadUserBase(userName);
            if (user == null)
            {
                return Guid.Empty;
            }

            return user.SysID;
        }

        public static string CurrentRequester(this HttpContextBase context)
        {
            string userName = context.User.Identity.Name;
            UserBO userBO = new UserBO();
            //UserWithRelationshipsDto user = userBO.ReadUser(userName);

            var user = userBO.ReadUserBase(userName);
            string name = null;
            if (user != null)
            {
                //if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                //{
                //    name = user.FirstName + " " + user.LastName;
                //}
                name = CustomHelper.UserNameFormat(user.LastName, user.FirstName, user.UserName);
            }

            return name;
        }

        public static string CurrentRequesterEmail(this HttpContextBase context)
        {
            string userName = context.User.Identity.Name;
            UserBO userBO = new UserBO();
            UserWithRelationshipsDto user = userBO.ReadUser(userName);
            string email = null;
            if (user != null)
            {
                email = user.Email;
            }

            return email;
        }

        public static string CurrentAuthType(this HttpContextBase context)
        {
            return (context.Session["__AuthType"] == null ? "" : context.Session["__AuthType"].ToString());
        }

        public static IHtmlString GetKStarUser(this HtmlHelper htmlHelper)
        {
            UserDto loginUser = null;
            var userService = new UserService();
            var userExFields = new List<string>();

            if (htmlHelper.ViewContext.Controller is BaseMvcController)
                loginUser = (htmlHelper.ViewContext.Controller as BaseMvcController).GetLoginUser();

            if (loginUser != null)
                loginUser.ExtendItems.ToList().ForEach(field => userExFields.Add(string.Format("{0}:'{1}'", field.Name, field.Value)));

            var formatedJSUser = loginUser == null ? "{}" : JsonHelper.SerializeObject(loginUser);
            var formatedJSUserExFields = !userExFields.Any() ? "{}" : string.Format("{{{0}}}", string.Join(",", userExFields));

            return htmlHelper.Raw(string.Format(@"{2} KStar.User={0}; KStar.User.ExField = {1};", formatedJSUser, formatedJSUserExFields,"if(typeof KStar === 'undefined'){ KStar={}; }"));
        }
    }
}