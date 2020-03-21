using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class BillingHelper
    {

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

        public static decimal GetFinalPrice(decimal basePrice, decimal discount, decimal scoring)
        {
            return (basePrice - (basePrice * (discount / 100))) * scoring;
        }

        public static int GetComission(int shopLifestyle)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            return manager.GetIntValue($"shop{((Lifestyles)shopLifestyle).ToString().ToLower()}");
        }

    }
}
