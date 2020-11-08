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
            switch (model.StateTo)
            {
                case "wounded":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnWounded(model.CharacterId);
                    break;
                case "clinically_dead":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnClinicalDeath(model.CharacterId);
                    break;
                default:
                    break;
            }
        }

    }
}
