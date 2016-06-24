
namespace aZaaS.KStar.Utilities
{
    /// <summary>
    /// AD用户属性
    /// </summary>
    public class ADUserProperty
    {
        /// <summary>
        /// 一般信息
        /// </summary>
        public class General
        {
            /// <summary>
            /// 通用名
            /// </summary>
            public const string CommonName = "cn";
            /// <summary>
            /// 名
            /// </summary>
            public const string FirstName = "givenName";
            /// <summary>
            /// 姓
            /// </summary>
            public const string LastName = "sn";
            /// <summary>
            /// 英文缩写
            /// </summary>
            public const string Initials = "initials";
            /// <summary>
            /// 显示名称
            /// </summary>
            public const string DisplayName = "displayName";
            /// <summary>
            /// 描述
            /// </summary>
            public const string Description = "description";
            /// <summary>
            /// 办公室
            /// </summary>
            public const string Office = "physicalDeliveryOfficeName";
            /// <summary>
            /// 电话号码
            /// </summary>
            public const string TelephoneNumber = "telephoneNumber";
            /// <summary>
            /// 电话号码：其它 otherTelephone 多个以英文分号分隔
            /// </summary>
            public const string TelephoneOther = "otherTelephone";
            /// <summary>
            /// 电子邮件
            /// </summary>
            public const string Email = "mail";
            /// <summary>
            /// WWW HomePage
            /// </summary>
            public const string WebPage = "wwwHomePage";
            /// <summary>
            /// 其它 url 多个以英文分号分隔
            /// </summary>
            public const string WebPageOther = "url";
        }

        /// <summary>
        /// 地址信息
        /// </summary>
        public class Address
        {
            /// <summary>
            /// 街道
            /// </summary>
            public const string Street = "streetAddress";
            /// <summary>
            /// 邮政信箱
            /// </summary>
            public const string P_O_Box = "postOfficeBox";
            /// <summary>
            /// 市/县
            /// </summary>
            public const string City = "l";
            /// <summary>
            /// 省/自治区
            /// </summary>
            public const string StateOrProvince = "st";
            /// <summary>
            /// 邮政编码
            /// </summary>
            public const string ZipOrPostalCode = "postalCode";
            /// <summary>
            /// c, co, and countryCode 如：中国CN，英国GB
            /// </summary>
            public const string CountryOrRegion = "c";
        }

        /// <summary>
        /// 帐户信息
        /// </summary>
        public class Account
        {
            /// <summary>
            /// 用户登录名,形如：pccai1983@hotmail.com 
            /// </summary>
            public const string UserLogonName = "userPrincipalName";
            /// <summary>
            /// 用户登录名（以前版本）,形如：S1 
            /// </summary>
            public const string UserLogonNamePreWin2000 = "sAMAccountname";
            /// <summary>
            /// 登录时间
            /// </summary>
            public const string LogonHours = "logonHours";
            /// <summary>
            /// 登录到,多个以英文逗号分隔
            /// </summary>
            public const string LogOnTo = "logonWorkstation";
            /// <summary>
            /// 用户帐户控制,(启用：512，禁用：514， 密码永不过期：66048)
            /// </summary>
            public const string AccountIsLockedOut = "userAccountControl";

            public const string LockOutTime = "lockOutTime";
            /// <summary>
            /// 
            /// </summary>
            public const string UserMustChangePasswordAtNextLogon = "pwdLastSet";
            /// <summary>
            /// 
            /// </summary>
            public const string UserCanNotChangePassword = "N/A";
            /// <summary>
            /// 
            /// </summary>
            public const string OtherAccountOptions = "userAccountControl";
            /// <summary>
            /// 帐户过期
            /// </summary>
            public const string AccountExpires = "accountExpires";
            /// <summary>
            /// 
            /// </summary>
            public const string WhenCreated = "whenCreated";
            /// <summary>
            /// 
            /// </summary>
            public const string WhenChanged = "whenChanged";

            /// <summary>
            /// 
            /// </summary>
            public const string ObjectGuid = "objectGUID";
            /// <summary>
            /// 
            /// </summary>
            public const string ObjectSID = "objectSid";
            /// <summary>
            /// 
            /// </summary>
            public const string DistinguishedName = "distinguishedName";

        }

        /// <summary>
        /// 组织信息
        /// </summary>
        public class Organization
        {
            /// <summary>
            /// 职务
            /// </summary>
            public const string Title = "title";
            /// <summary>
            /// 部门
            /// </summary>
            public const string Department = "department";
            /// <summary>
            /// 公司
            /// </summary>
            public const string Company = "company";
            /// <summary>
            /// 
            /// </summary>
            public const string ManagerName = "manager";
            /// <summary>
            /// 
            /// </summary>
            public const string DirectReports = "directReports";
        }

        /// <summary>
        /// 用户配置信息
        /// </summary>
        public class Profile
        {
            /// <summary>
            /// 配置文件路径
            /// </summary>
            public const string ProfilePath = "profilePath";
            /// <summary>
            /// 登录脚本
            /// </summary>
            public const string LogonScript = "scriptPath";
            /// <summary>
            /// 主文件夹：本地路径
            /// </summary>
            public const string HomeFolder_LocalPath = "homeDirectory";
            /// <summary>
            /// 连接
            /// </summary>
            public const string HomeFolder_Connect = "homeDrive";
            /// <summary>
            /// 到
            /// </summary>
            public const string HomeFolder_To = "homeDirectory";
        }

        /// <summary>
        /// 联系电话信息
        /// </summary>
        public class Telephones
        {
            /// <summary>
            /// 家庭电话
            /// </summary>
            public const string Home = "telephoneNumber";
            /// <summary>
            /// 
            /// </summary>
            public const string Home_Other = "otherTelephone";
            /// <summary>
            /// 寻呼机
            /// </summary>
            public const string Pager = "pager";
            /// <summary>
            /// 
            /// </summary>
            public const string Pager_Other = "pagerOther";
            /// <summary>
            /// 移动电话,若多个以英文分号分隔
            /// </summary>
            public const string Mobile = "mobile";
            /// <summary>
            /// 
            /// </summary>
            public const string Mobile_Other = "otherMobile";
            /// <summary>
            /// 传真
            /// </summary>
            public const string Fax = "facsimileTelephoneNumber";
            /// <summary>
            /// 
            /// </summary>
            public const string Fax_Other = "otherFacsimileTelephoneNumber";
            /// <summary>
            /// IP电话
            /// </summary>
            public const string IPPhone = "ipPhone";
            /// <summary>
            /// 
            /// </summary>
            public const string IPPhone_Other = "otherIpPhone";
            /// <summary>
            /// 注释
            /// </summary>
            public const string Notes = "info";
        }

        /// <summary>
        /// 成员隶属信息
        /// </summary>
        public class Members
        {
            /// <summary>
            /// 隶属于,用户组的DN不需使用引号， 多个用分号分隔
            /// </summary>
            public const string MemberOf = "memberOf";
            /// <summary>
            /// 
            /// </summary>
            public const string SetPrimaryGroup = "primaryGroupID";
        }

    }
}
