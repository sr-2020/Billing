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
            IScoringManager scoring;
            IBillingManager billing;
            var modelId = BillingHelper.ParseId(model.CharacterId, "characterId");
            switch (model.StateTo)
            {
                case "wounded":
                    scoring = IoC.IocContainer.Get<IScoringManager>();
                    scoring.OnWounded(modelId);
                    break;
                case "clinically_dead":
                    scoring = IoC.IocContainer.Get<IScoringManager>();
                    scoring.OnClinicalDeath(modelId);
                    break;
                case "biologically_dead":
                    billing = IoC.IocContainer.Get<IBillingManager>();
                    billing.DropCharacter(modelId);
                    break;
                case "healthy":
                    switch (model.StateFrom)
                    {
                        case "clinically_dead":
                            billing = IoC.IocContainer.Get<IBillingManager>();
                            billing.DropInsurance(modelId);
                            break;
                        case "biologically_dead":
                            billing = IoC.IocContainer.Get<IBillingManager>();
                            billing.RestoreCharacter(modelId);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }



    }
}
