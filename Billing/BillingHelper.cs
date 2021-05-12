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
        public const string BlockErrorMessage = "В данный момент ведется пересчет рентных платежей, попробуйте повторить чуть позже";
        public static int GetModelId(string model)
        {
            int modelId;
            if (!int.TryParse(model, out modelId))
                throw new BillingException("modelId is invalid");
            return modelId;
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

        public static void BillingBlocked(int modelId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var sin = manager.GetSINByModelId(modelId);
            if (sin?.Blocked ?? true)
            {
                throw new BillingException(BlockErrorMessage);
            }
        }

        public static void BillingBlocked()
        {
            ISettingsManager _settings = IocContainer.Get<ISettingsManager>();
            var block = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (block)
            {
                throw new BillingException(BlockErrorMessage);
            }
        }

    }
}
