using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Web;

using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;

namespace aZaaS.KStar.Wcf
{
    public class ServiceErrorHandler : IErrorHandler
    {
        const string SERVICE_NAME = "KStar.WcfService";

        public bool HandleError(Exception error)
        {
            try
            {
                var logger = LogFactory.GetLogger();
                logger.Write(LogUtil.CreateLog(SERVICE_NAME, error.Source, error.Message, error));
            }
            catch { }

            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var faultEx = new FaultException(error.Message);
            var msgFault = faultEx.CreateMessageFault();

            fault = Message.CreateMessage(version,msgFault, faultEx.Action);
        }
    }
}