using Billing;
using IoC;
using PubSubService.Model;
using Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubAbilityService : IPubSubService
    {
        void HandleAbility(AbilityModel model);
    }


    public class PubSubAbilityService : PubSubService<AbilityModel>, IPubSubAbilityService
    {
        public PubSubAbilityService() : base("billing_ability_used_subscription")
        {

        }
        public override void Handle(AbilityModel model)
        {
            base.Handle(model);
            if (ModelPrimitives.Abilities.ContainsKey(model.Id))
            {
                try
                {
                    HandleAbility(model);
                }
                catch (Exception e)
                {
                    e.ToString();
                }

            }
        }

        public void HandleAbility(AbilityModel model)
        {
            ActiveAbility activeAbilityEnum;
            ModelPrimitives.Abilities.TryGetValue(model.Id, out activeAbilityEnum);
            IBillingManager billing;
            switch (activeAbilityEnum)
            {
                case ActiveAbility.HowMuch:
                    break;
                case ActiveAbility.WhoNeed:
                    break;
                case ActiveAbility.PayAndCry:
                    break;
                case ActiveAbility.LetHim:
                    billing = IocContainer.Get<IBillingManager>();
                    billing.LetHimPay(model?.CharacterId, model?.TargetCharacterId, model?.QrCode?.Data?.DealId);
                    break;
                case ActiveAbility.Letme:
                    billing = IocContainer.Get<IBillingManager>();
                    billing.LetMePay(model?.CharacterId, model?.QrCode?.Data?.DealId);
                    break;
                default:
                    break;
            }
        }

    }
}
