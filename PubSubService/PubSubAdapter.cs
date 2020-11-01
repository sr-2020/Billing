using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PubSubService
{
    public class PubSubAdapter
    {
        public void PullMessages(string subscriptionId, Action<string> action)
        {
            var projectId = "imposing-elixir-249711";
            var subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberServiceApiClient subscriber = SubscriberServiceApiClient.Create();
            var response = subscriber.Pull(subscriptionName, returnImmediately: true, maxMessages: 10);
            foreach (ReceivedMessage received in response.ReceivedMessages)
            {
                PubsubMessage msg = received.Message;
                action.Invoke(msg.Data.ToStringUtf8());
            }
            if(response.ReceivedMessages?.Count > 0)
            {
                subscriber.Acknowledge(subscriptionName, response.ReceivedMessages.Select(m => m.AckId));
            }
        }
    }
}
