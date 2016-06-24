using System;
using System.Collections.Generic;
using System.Text;

namespace aZaaS.KSTAR.MobileServices.Helpers
{
    public class ADUserProperty
    {
        public class General
        {
            public const string CommonName = "cn"; //名
            public const string FirstName = "givenName"; //名
            public const string LastName = "sn"; // 姓
            public const string Initials = "initials"; //英文缩写
            public const string DisplayName = "displayName"; //显示名称
            public const string Description = "description"; //描述
            public const string Office = "physicalDeliveryOfficeName"; //办公室
            public const string TelephoneNumber = "telephoneNumber"; //电话号码
            public const string TelephoneOther = "otherTelephone"; //电话号码：其它 otherTelephone 多个以英文分号分隔
            public const string Email = "mail"; //电子邮件
            public const string WebPage = "wwwHomePage"; //wWWHomePage
            public const string WebPageOther = "url"; //其它 url 多个以英文分号分隔
        }

        public class Address
        {
            public const string Street = "streetAddress"; //街道
            public const string P_O_Box = "postOfficeBox"; //邮政信箱
            public const string City = "l"; //市/县
            public const string StateOrProvince = "st"; //省/自治区
            public const string ZipOrPostalCode = "postalCode"; //邮政编码
            public const string CountryOrRegion = "c"; //c, co, and countryCode 如：中国CN，英国GB
        }

        public class Account
        {
            public const string UserLogonName = "userPrincipalName"; //用户登录名,形如：pccai1983@hotmail.com 
            public const string UserLogonNamePreWin2000 = "sAMAccountname"; //用户登录名（以前版本）,形如：S1 
            public const string LogonHours = "logonHours"; //登录时间
            public const string LogOnTo = "logonWorkstation"; //登录到,多个以英文逗号分隔
            public const string AccountIsLockedOut = "userAccountControl"; //用户帐户控制,(启用：512，禁用：514， 密码永不过期：66048)
            public const string UserMustChangePasswordAtNextLogon = "pwdLastSet"; //
            public const string UserCanNotChangePassword = "N/A";
            public const string OtherAccountOptions = "userAccountControl";
            public const string AccountExpires = "accountExpires"; //帐户过期
            public const string WhenCreated = "whenCreated";
            public const string WhenChanged = "whenChanged";

            public const string ObjectGuid = "objectGUID";
            public const string ObjectSID = "objectSid";
            public const string DistinguishedName = "distinguishedName";

        }

        public class Organization
        {
            public const string Title = "title"; //职务
            public const string Department = "department"; //部门
            public const string Company = "company"; //公司
            public const string ManagerName = "manager";
            public const string DirectReports = "directReports";
        }

        public class Profile
        {
            public const string ProfilePath = "profilePath"; //配置文件路径
            public const string LogonScript = "scriptPath"; //登录脚本
            public const string HomeFolder_LocalPath = "homeDirectory"; //主文件夹：本地路径
            public const string HomeFolder_Connect = "homeDrive"; //连接
            public const string HomeFolder_To = "homeDirectory"; //到
        }

        public class Telephones
        {
            public const string Home = "telephoneNumber"; //家庭电话
            public const string Home_Other = "otherTelephone";
            public const string Pager = "pager"; //寻呼机
            public const string Pager_Other = "pagerOther";
            public const string Mobile = "mobile"; //移动电话,若多个以英文分号分隔
            public const string Mobile_Other = "otherMobile";
            public const string Fax = "facsimileTelephoneNumber"; //传真
            public const string Fax_Other = "otherFacsimileTelephoneNumber";
            public const string IPPhone = "ipPhone"; //IP电话
            public const string IPPhone_Other = "otherIpPhone";
            public const string Notes = "info"; //注释
        }

        public class Members
        {
            public const string MemberOf = "memberOf"; //隶属于,用户组的DN不需使用引号， 多个用分号分隔
            public const string SetPrimaryGroup = "primaryGroupID";
        }

    }
}
