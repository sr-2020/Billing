using PubSubService.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubHealthService : IPubSubService
    {
    
    }


    public class PubSubHealthService : PubSubService<HealthModel>, IPubSubHealthService
    {
        public PubSubHealthService() : base("billing_health_state")
        {

        }

        public override void Handle(HealthModel model)
        {
            base.Handle(model);

        }

    }
}
