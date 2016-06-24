
namespace aZaaS.KStar.Utilities
{
   public enum ADUserFlag
    {
        /// 
        /// 登录脚本标志。如果通过 ADSI LDAP 进行读或写操作时，该标志失效。如果通过 ADSI WINNT，该标志为只读。
        /// 
        SCRIPT = 0X0001,
        /// 
        /// 用户帐号禁用标志
        /// 
        ACCOUNTDISABLE = 0X0002,
        /// 
        /// 主文件夹标志
        /// 
        HOMEDIR_REQUIRED = 0X0008,
        /// 
        /// 过期标志
        /// 
        LOCKOUT = 0X0010,
        /// 
        /// 用户密码不是必须的
        /// 
        PASSWD_NOTREQD = 0X0020,
        /// 
        /// 密码不能更改标志
        /// 
        PASSWD_CANT_CHANGE = 0X0040,
        /// 
        /// 使用可逆的加密保存密码
        /// 
        ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
        /// 
        /// 本地帐号标志
        /// 
        TEMP_DUPLICATE_ACCOUNT = 0X0100,
        /// 
        /// 普通用户的默认帐号类型
        /// 
        NORMAL_ACCOUNT = 0X0200,
        /// 
        /// 跨域的信任帐号标志
        /// 
        INTERDOMAIN_TRUST_ACCOUNT = 0X0800,
        /// 
        /// 工作站信任帐号标志
        /// 
        WORKSTATION_TRUST_ACCOUNT = 0x1000,
        /// 
        /// 服务器信任帐号标志
        /// 
        SERVER_TRUST_ACCOUNT = 0X2000,
        /// 
        /// 密码永不过期标志
        /// 
        DONT_EXPIRE_PASSWD = 0X10000,
        /// 
        /// MNS 帐号标志
        /// 
        MNS_LOGON_ACCOUNT = 0X20000,
        /// 
        /// 交互式登录必须使用智能卡
        /// 
        SMARTCARD_REQUIRED = 0X40000,
        /// 
        /// 当设置该标志时，服务帐号（用户或计算机帐号）将通过 Kerberos 委托信任
        /// 
        TRUSTED_FOR_DELEGATION = 0X80000,
        /// 
        /// 当设置该标志时，即使服务帐号是通过 Kerberos 委托信任的，敏感帐号不能被委托
        /// 
        NOT_DELEGATED = 0X100000,
        /// 
        /// 此帐号需要 DES 加密类型
        /// 
        USE_DES_KEY_ONLY = 0X200000,
        /// 
        /// 不要进行 Kerberos 预身份验证
        /// 
        DONT_REQUIRE_PREAUTH = 0X4000000,
        /// 
        /// 用户密码过期标志
        /// 
        PASSWORD_EXPIRED = 0X800000,
        /// 
        /// 用户帐号可委托标志
        /// 
        TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0X1000000
    }
}
