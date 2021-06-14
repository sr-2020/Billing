using Billing;
using PubSubService.Model;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{

    public interface IPubSubImplantUninstallService : IPubSubService
    {

    }


    public class PubSubImplantUninstallService : PubSubService<ImplantInstallModel>, IPubSubImplantInstallService
    {
        public PubSubImplantUninstallService() : base("billing_implant_uninstall")
        {

        }

        public override void Handle(ImplantInstallModel model)
        {
            base.Handle(model);
            IScoringManager manager;
            switch (model.AbilityId)
            {

                case "autodoc":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnImplantUninstalled(BillingHelper.ParseId(model.CharacterId, "characterId"), model.AutodocLifestyle);
                    break;
                case "repoman-black":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnImplantDeletedBlack(BillingHelper.ParseId(model.CharacterId, "characterId"));
                    break;
                case "repoman-active":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnImplantDeletedActive(BillingHelper.ParseId(model.CharacterId, "characterId"));
                    break;
                default:
                    break;
            }

        }
    }

}
