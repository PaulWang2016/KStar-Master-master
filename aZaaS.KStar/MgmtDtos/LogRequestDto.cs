using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace aZaaS.KStar.MgmtDtos
{
    public class LogRequestDto
    {
        /// <summary>
        /// 无参构造方法
        /// </summary>
        public LogRequestDto() { }
        /// <summary>
        /// 指定字段的构造方法
        /// </summary>
        public LogRequestDto(string name, string requestUrl, string requestType, string parameters, string message, string details, string iPAddress, string requestUser, DateTime requestTime)
        {
            this.Name = name;
            this.RequestType = requestType;
            this.RequestUrl = requestUrl;
            this.Parameters = parameters;
            this.Message = message;
            this.Details = details;
            this.IPAddress = iPAddress;
            this.RequestUser = requestUser;
            this.RequestTime = requestTime;
        }

        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string requestUrl;
        public string RequestUrl
        {
            get { return requestUrl; }
            set { requestUrl = value; }
        }
        private string requestType;
        public string RequestType
        {
            get { return requestType; }
            set { requestType = value; }
        }

        private string parameters;
        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private string details;
        public string Details
        {
            get { return details; }
            set { details = value; }
        }

        private string iPAddress;
        public string IPAddress
        {
            get { return iPAddress; }
            set { iPAddress = value; }
        }

        private string requestUser;
        public string RequestUser
        {
            get { return requestUser; }
            set { requestUser = value; }
        }

        private DateTime requestTime;
        public DateTime RequestTime
        {
            get { return requestTime; }
            set { requestTime = value; }
        }
    }
}