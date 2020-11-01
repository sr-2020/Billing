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
        IPubSubAbilityService _service;
        public PubSubSubscriber(IPubSubAbilityService service)
        {
            _service = service;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _service.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _service.Stop();
            return Task.CompletedTask;
        }
    }
}
