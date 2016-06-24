using System.Runtime.Serialization;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class BaseResult
    {
        /// <summary>
        /// 结果类型： S - 成功, E - 异常
        /// </summary>
        public string Result { get; set; }
        public string Message { get; set; }
        public string MessageDetails { get; set; }
    }
}
