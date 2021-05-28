using Billing;
using PubSubService.Model;
using Scoringspace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PubSubService
{
    public interface IPubSubFoodService : IPubSubService
    {

    }
    public class PubSubFoodService : PubSubService<FoodModel>, IPubSubFoodService
    {
        public PubSubFoodService() : base("billing_food_consumption")
        {

        }

        public override void Handle(FoodModel model)
        {
            base.Handle(model);
            var manager = IoC.IocContainer.Get<IScoringManager>();
            manager.OnFoodConsume(BillingHelper.ParseId(model.CharacterId, "characterId"), model.LifeStyle);
        }

    }
}
