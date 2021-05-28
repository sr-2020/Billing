using Billing.Dto;
using Billing.Dto.Shop;
using Core;
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
        public static int ParseId(string id, string field)
        {
            if (!int.TryParse(id, out int intid))
            {
                throw new BillingException($"Ошибка парсинга {field} {id}");
            }
            return intid;
        }

        public static bool LifestyleIsDefined(string name)
        {
            return Enum.IsDefined(typeof(Lifestyles), name);
        }

        public static Lifestyles GetLifestyle(string name)
        {
            return (Lifestyles)Enum.Parse(typeof(Lifestyles), name);
        }

        public static DiscountType GetDiscountType(int discountType)
        {
            if (Enum.IsDefined(typeof(DiscountType), discountType))
                return (DiscountType)discountType;
            else
                return DiscountType.Gesheftmaher;
        }

        public static decimal Round(decimal value)
        {
            return Math.Round(value, 2);// Math.Floor(value * 2) / 2;
        }

        public static LifeStyleAppDto GetLifeStyleDto()
        {
            var manager = IocContainer.Get<ISettingsManager>();
            var dto = manager.GetValue(SystemSettingsEnum.ls_dto);
            LifeStyleAppDto deserialized;
            try
            {
                deserialized = Serialization.Serializer.Deserialize<LifeStyleAppDto>(dto);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Ошибка десериализации ls_dto: {dto}");
                return new LifeStyleAppDto();
            }
            return deserialized;
        }

        public static decimal GetForecast(Wallet wallet)
        {
            return wallet.Balance + (wallet.IncomeOutcome * 3);
        }

        public static Lifestyles GetLifestyle(int lifestyle)
        {
            if (Enum.IsDefined(typeof(Lifestyles), lifestyle))
                return (Lifestyles)lifestyle;
            else
                return Lifestyles.Wood;
        }

        public static List<NamedEntity> GetLifestyles()
        {
            var ls = new List<NamedEntity>();
            foreach (Lifestyles lifestyle in Enum.GetValues(typeof(Lifestyles)))
            {
                ls.Add(GetLifestyleDto(lifestyle));
            }
            return ls;
        }

        public static NamedEntity GetLifestyleDto(Lifestyles lifestyle)
        {
            return new NamedEntity { Id = (int)lifestyle, Name = lifestyle.ToString() };
        }

        public static decimal GetFinalPrice(decimal basePrice, decimal discount, decimal scoring)
        {
            return BillingHelper.Round((basePrice * discount) / scoring);
        }

        public static decimal CalculateComission(decimal basePrice, decimal comission)
        {
            return basePrice * (comission / 100);
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
