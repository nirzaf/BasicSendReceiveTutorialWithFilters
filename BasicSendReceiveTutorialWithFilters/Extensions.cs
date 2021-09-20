using System.Collections.Generic;
using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json; //using Microsoft.Azure.ServiceBus;

namespace BasicSendReceiveTutorialWithFilters
{
    public static class Extensions
    {
        public static T As<T>(this ServiceBusReceivedMessage message) where T : class
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body.ToArray()));
        }

        public static ServiceBusMessage AsMessage(this object obj)
        {
            return new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
        }

        public static bool Any(this IList<ServiceBusReceivedMessage> collection)
        {
            return collection != null && collection.Count > 0;
        }
    }
}