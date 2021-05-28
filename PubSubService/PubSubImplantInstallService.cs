using Billing;
using PubSubService.Model;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubImplantInstallService : IPubSubService
    {

    }


    public class PubSubImplantInstallService : PubSubService<ImplantInstallModel>, IPubSubImplantInstallService
    {
        public PubSubImplantInstallService() : base("billing_implant_install")
        {
            
        }

        public override void Handle(ImplantInstallModel model)
        {
            base.Handle(model);
            var manager = IoC.IocContainer.Get<IScoringManager>();
            manager.OnImplantInstalled(BillingHelper.ParseId(model.CharacterId, "characterId"), model.ImplantLifestyle, model.AutodocLifestyle);
        }
    }
}
