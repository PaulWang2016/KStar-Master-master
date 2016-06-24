using System.Runtime.Serialization;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class LoginResult : BaseResult
    {
        public ResultContent Content { get; set; }
    }

    public class ResultContent
    {
        public string UserToken { get; set; }
        public string Mask { get; set; }
        public UserInfo UserInfo { get; set; }
    }

    public class UserInfo
    {
        public string UserName { get; set; }
    }
}
