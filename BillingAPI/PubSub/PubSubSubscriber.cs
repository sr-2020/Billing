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
        Dictionary<string, IPubSubService> _services;

        public PubSubSubscriber(IPubSubAbilityService ability, IPubSubFoodService food, IPubSubHealthService health, IPubSubDampshockService dump, IPubSubImplantInstallService implant, IPubSubPillConsumptionService pill)
        {
            _services = new Dictionary<string, IPubSubService>();
            _services.Add(ability.SubscriptionId, ability);
            _services.Add(food.SubscriptionId, food);
            _services.Add(health.SubscriptionId, health);
            _services.Add(dump.SubscriptionId, dump);
            _services.Add(implant.SubscriptionId, implant);
            _services.Add(pill.SubscriptionId, pill);
        }

        public string GetStatus(string id)
        {
            IPubSubService service;
            if (_services.TryGetValue(id, out service))
            {
                return service.IsRunning ? "running" : "stoped";
            }
            return "unknown service";
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                service.Value.Run();
                Console.WriteLine($"{service.Key} started");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                Console.WriteLine($"{service.Key} stoped");
                service.Value.Stop();
            }
            return Task.CompletedTask;
        }
    }
}
