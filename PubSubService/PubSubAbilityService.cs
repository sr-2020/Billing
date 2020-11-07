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
                HandleAbility(model);
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
                    break;
                case ActiveAbility.Letme:
                    break;
                default:
                    break;
            }
        }

    }
}
