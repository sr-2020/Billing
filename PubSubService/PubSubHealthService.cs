using Billing;
using PubSubService.Model;
using Scoringspace;
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
            IScoringManager manager;
            var modelId = BillingHelper.ParseId(model.CharacterId, "characterId");
            switch (model.StateTo)
            {
                case "wounded":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnWounded(modelId);
                    break;
                case "clinically_dead":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnClinicalDeath(modelId);
                    var billing = IoC.IocContainer.Get<IBillingManager>();
                    billing.DropInsurance(modelId);
                    break;
                default:
                    break;
            }
        }



    }
}
