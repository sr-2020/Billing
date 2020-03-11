using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class LifeStyleHelper
    {

        public static readonly Dictionary<Lifestyles, int> MaxValues = new Dictionary<Lifestyles, int>
        {
            { Lifestyles.Wood, 100 },
            { Lifestyles.Bronze, 200 },
            { Lifestyles.Silver, 300 },
            { Lifestyles.Gold, 400 },
            { Lifestyles.Platinum, 500 },
            { Lifestyles.Iridium, 600 },
        };

        public static Lifestyles GetLifeStyle(decimal balance)
        {
            var manager = IocContainer.Get<ISettingsManager>();

            foreach (Lifestyles lifestyle in Enum.GetValues(typeof(Lifestyles)))
            {
                if (balance < manager.GetIntValue(lifestyle.ToString().ToLower()))
                    return lifestyle;
            }
            return Lifestyles.Iridium;
        }

    }
}
