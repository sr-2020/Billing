using Core.Model;
using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    public class BillingHelper
    {

        public static DiscountType GetDiscountType(int discountType)
        {
            if (Enum.IsDefined(typeof(DiscountType), discountType))
                return (DiscountType)discountType;
            else
                return DiscountType.Gesheftmaher;
        }

        public static decimal RoundDown(decimal value)
        {
            return Math.Floor(value * 2) / 2;
        }

        public static Lifestyles GetLifeStyleByBalance(decimal balance)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            foreach (Lifestyles lifestyle in Enum.GetValues(typeof(Lifestyles)))
            {
                if (balance < manager.GetIntValue(lifestyle.ToString().ToLower()))
                    return lifestyle;
            }
            return Lifestyles.Iridium;
        }

        public static Lifestyles GetLifestyle(int lifestyle)
        {
            if (Enum.IsDefined(typeof(Lifestyles), lifestyle))
                return (Lifestyles)lifestyle;
            else
                return Lifestyles.Wood;
        }

        public static decimal GetFinalPrice(decimal basePrice, decimal discount, decimal scoring)
        {
            return (basePrice - (basePrice * (discount / 100))) / scoring;
        }

        //public static int GetComission(int shopLifestyle)
        //{
        //    var manager = IocContainer.Get<ISettingsManager>();
        //    return manager.GetIntValue($"shop{((Lifestyles)shopLifestyle).ToString().ToLower()}");
        //}

        public static decimal CalculateComission(decimal basePrice, decimal comission)
        {
            return basePrice * comission;
        }

        public static bool HasQrWrite(string code)
        {
            return !string.IsNullOrEmpty(code);
        }

        public static bool IsAdmin(int character)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            try
            {
                var list = manager.GetValue(SystemSettingsEnum.admin_id).Split(';').ToList();
                if (list.Contains(character.ToString()))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {

                return false;
            }
        }

    }
}
