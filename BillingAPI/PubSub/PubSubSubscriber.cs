using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Hosting;
using PubSubService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BillingAPI
{
    public class PubSubSubscriber : IHostedService
    {
        List<IPubSubService> _services;

        public PubSubSubscriber(IPubSubAbilityService ability, IPubSubFoodService food, IPubSubHealthService health)
        {
            _services = new List<IPubSubService>();
            _services.Add(ability);
            _services.Add(food);
            _services.Add(health);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                service.Run();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                service.Stop();
            }
            return Task.CompletedTask;
        }
    }
}
