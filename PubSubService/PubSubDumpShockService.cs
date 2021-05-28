using Billing;
using PubSubService.Model;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubDampshockService : IPubSubService
    {

    }
    public class PubSubDampshockService : PubSubService<BaseLocationModel>, IPubSubDampshockService
    {
        public PubSubDampshockService() : base("billing_dumpshock")
        {
            
        }

        public override void Handle(BaseLocationModel model)
        {
            base.Handle(model);
            var manager = IoC.IocContainer.Get<IScoringManager>();
            manager.OnDumpshock(BillingHelper.ParseId(model.CharacterId, "characterId"));
        }

    }
}
