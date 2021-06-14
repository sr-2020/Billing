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
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        public void HandleAbility(AbilityModel model)
        {
            ActiveAbility activeAbilityEnum;
            ModelPrimitives.Abilities.TryGetValue(model.Id, out activeAbilityEnum);
            switch (activeAbilityEnum)
            {
                case ActiveAbility.HowMuch:
                    break;
                case ActiveAbility.WhoNeed:
                    break;
                case ActiveAbility.PayAndCry:
                    break;
                case ActiveAbility.LetHim:
                    IocContainer.Get<IAbilityManager>().LetHimPay(BillingHelper.ParseId(model?.CharacterId, "characterId"), BillingHelper.ParseId(model?.TargetCharacterId, "TargetCharacterId") , BillingHelper.ParseId(model?.QrCode?.Data?.DealId, "DealId"), model?.QrCode?.ModelId);
                    break;
                case ActiveAbility.Letme:
                    IocContainer.Get<IAbilityManager>().LetMePay(BillingHelper.ParseId(model?.CharacterId, "characterId"), BillingHelper.ParseId(model?.QrCode?.Data?.DealId, "DealId"), model?.QrCode?.ModelId);
                    break;
                case ActiveAbility.Rerent:
                    IocContainer.Get<IAbilityManager>().Rerent(BillingHelper.ParseId(model?.CharacterId, "characterId"), BillingHelper.ParseId(model?.QrCode?.Data?.DealId, "DealId"), model?.QrCode?.ModelId);
                    break;
                case ActiveAbility.Marauder:
                    IocContainer.Get<IAbilityManager>().Marauder(BillingHelper.ParseId(model?.CharacterId, "characterId"), BillingHelper.ParseId(model?.TargetCharacterId, "TargetCharacterId"));
                    break;
                default:
                    break;
            }
        }
    }
}
