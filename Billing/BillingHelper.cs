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
        public static decimal GetFullScoring(Scoring scoring)
        {
            return scoring.CurerentRelative + scoring.CurrentFix;
        }

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

        public static Lifestyles GetLifestyleFromJoin(string fieldValue)
        {
            switch (fieldValue)
            {
                case "Иридий":
                    return Lifestyles.Iridium;
                case "Дерево":
                    return Lifestyles.Wood;
                case "Бронза":
                    return Lifestyles.Bronze;
                case "Серебро":
                    return Lifestyles.Silver;

                default:
                    break;
            }
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

        public static string GetPassportName(Passport passport, bool anon = false)
        {
            if (passport == null)
                return string.Empty;
            if (anon)
                return "anonymous";
            return $"{ passport.PersonName} ({ passport.Sin})";
        }

        public static decimal GetSpecialisationPrice(CorporationSpecialisation specialisation, Nomenklatura nomenklatura)
        {
            decimal ratio = 5;
            if(specialisation != null)
            {
                ratio = specialisation.Ratio;
            }
            return nomenklatura.BasePrice * ratio;
        }

        //public static decimal GetFinalPrice(Sku sku, decimal discount, decimal scoring)
        //{
        //    var price = sku.Price;
        //    if (price == 0)
        //        price = sku.SkuBasePrice ?? sku.Nomenklatura.BasePrice;
        //    return GetFinalPrice(price, discount, scoring);
        //}


        public static decimal GetShopPrice(Sku sku)
        {
            var price = sku.Price;
            if (price == 0)
                price = sku.SkuBasePrice ?? sku.Nomenklatura.BasePrice;
            return price;
        }

        public static decimal GetFinalPrice(Price price) 
        {
            return GetFinalPrice(price.ShopPrice, price.Discount, price.CurrentScoring);
        }

        public static decimal GetFinalPrice(Renta renta) 
        {
            return GetFinalPrice(renta.ShopPrice, renta.Discount, renta.CurrentScoring);
        }

        public static decimal CalculateComission(Renta renta)
        {
            return renta.BasePrice * (renta.ShopComission / 100);
        }

        public static bool HasQrWrite(string code)
        {
            return !string.IsNullOrEmpty(code);
        }

        public static string GetGmDescription(Passport passport, Sku sku, bool anon)
        {
            return $"Рента по товару {sku.Name} оформлена на {GetPassportName(passport, anon)}";
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

        public static decimal GetShopComission(int shopls)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            switch (GetLifestyle(shopls))
            {
                case Lifestyles.Wood:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopwood);
                case Lifestyles.Bronze:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopbronze);
                case Lifestyles.Silver:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopsilver);
                case Lifestyles.Gold:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopgold);
                case Lifestyles.Platinum:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopplatinum);
                case Lifestyles.Iridium:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopiridium);
                default:
                    return manager.GetDecimalValue(SystemSettingsEnum.shopwood);
            }
        }

        private static decimal GetFinalPrice(decimal price, decimal discount, decimal scoring)
        {
            return Round((price * discount) / scoring);
        }
    }
}
