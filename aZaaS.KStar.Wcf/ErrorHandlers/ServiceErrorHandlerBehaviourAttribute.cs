using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace aZaaS.KStar.Wcf
{
    public class ServiceErrorHandlerBehaviourAttribute : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var handler = new ServiceErrorHandler();

            foreach (ChannelDispatcherBase dispatcherBase in  serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                    channelDispatcher.ErrorHandlers.Add(handler);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
        }
    }
}