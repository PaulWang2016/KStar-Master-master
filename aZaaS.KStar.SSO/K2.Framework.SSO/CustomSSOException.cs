using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace K2.Framework.SSO
{
    [Serializable]
    public class CustomSSOException : System.ApplicationException
    {
        private int _errorCode = 0;   //错误代码

        public int ErrorCode
        {
            get
            { return this._errorCode; }
            set
            { this._errorCode = value; }
        }


        public CustomSSOException()
            : base()
        {
        }

        public CustomSSOException(string message)
            : base(message)
        {
        }

        public CustomSSOException(string message, int errorCode)
            : base(message)
        {
            this._errorCode = errorCode;
        }



        public CustomSSOException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public CustomSSOException(string message, Exception inner, int errorCode)
            : base(message, inner)
        {
            this._errorCode = errorCode;
        }


        protected CustomSSOException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

    }
}
