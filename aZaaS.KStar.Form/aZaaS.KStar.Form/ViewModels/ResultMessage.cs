using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class ResultMessage
    {
        public ResultMessage(MessageType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        public static ResultMessage Create(MessageType type, string message)
        {
            return new ResultMessage(type, message);
        }

        public MessageType Type { get; set; }
        public string Message { get; set; }
    }

    public enum MessageType
    {
        Error,
        Information,
        Warning
    }
}
