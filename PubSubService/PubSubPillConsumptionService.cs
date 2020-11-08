using Billing;
using PubSubService.Model;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubPillConsumptionService : IPubSubService
    {

    }
    public class PubSubPillConsumptionService : PubSubService<PillConsumptionModel>, IPubSubPillConsumptionService
    {
        public PubSubPillConsumptionService() : base("billing_pill_consumption")
        {

        }

        public override void Handle(PillConsumptionModel model)
        {
            base.Handle(model);

            var manager = IoC.IocContainer.Get<IScoringManager>();
            manager.OnPillConsumed(model.CharacterId, model.LifeStyle);

        }
    }
}
