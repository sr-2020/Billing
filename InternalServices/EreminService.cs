using Core.Primitives;
using InternalServices.EreminModel;
using Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalServices
{
    public class EreminService
    {
        const string URL = "http://models-manager.evarun.ru";

        public static CharacterModel GetCharacter(int characterId)
        {
            var client = new HttpClient();
            var url = $"{URL}/character/model/{characterId}";
            var response = client.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var model = Serializer.Deserialize<CharacterModel>(response.Content.ReadAsStringAsync().Result);
                return model;
            }
            throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public static bool GetAnonimous(int characterId)
        {
            var model = GetCharacter(characterId);
            if (model != null)
            {
                return model.workModel.billing.anonymous;
            }
            return false;
        }

        public static decimal GetDiscount(int characterId, DiscountType discountType, CorporationEnum corporation)
        {
            var model = GetCharacter(characterId);
            decimal every = 1;
            if (model != null)
            {
                var gesheft = model.workModel.discounts.everything;
                decimal samurai = 1;
                if (discountType == DiscountType.Samurai)
                    samurai = model.workModel.discounts.weaponsArmor;
                every = gesheft < samurai ? gesheft : samurai;
            }
            decimal corpDisc = 1;
            switch (corporation)
            {
                case CorporationEnum.ares:
                    corpDisc = model.workModel.discounts.ares;
                    break;
                case CorporationEnum.aztechnology:
                    corpDisc = model.workModel.discounts.aztechnology;
                    break;
                case CorporationEnum.saederKrupp:
                    corpDisc = model.workModel.discounts.saederKrupp;
                    break;
                case CorporationEnum.spinradGlobal:
                    corpDisc = model.workModel.discounts.spinradGlobal;
                    break;
                case CorporationEnum.neonet1:
                    corpDisc = model.workModel.discounts.neonet1;
                    break;
                case CorporationEnum.evo:
                    corpDisc = model.workModel.discounts.evo;
                    break;
                case CorporationEnum.horizon:
                    corpDisc = model.workModel.discounts.horizon;
                    break;
                case CorporationEnum.wuxing:
                    corpDisc = model.workModel.discounts.wuxing;
                    break;
                case CorporationEnum.russia:
                    corpDisc = model.workModel.discounts.russia;
                    break;
                case CorporationEnum.renraku:
                    corpDisc = model.workModel.discounts.renraku;
                    break;
                case CorporationEnum.mutsuhama:
                    corpDisc = model.workModel.discounts.mutsuhama;
                    break;
                case CorporationEnum.shiavase:
                    corpDisc = model.workModel.discounts.shiavase;
                    break;
                case CorporationEnum.unknown:
                default:
                    break;
            }
            return every * corpDisc;
        }

        public static bool WriteQR(string qr, string id, string name, string description, int numberOfUses, decimal basePrice, decimal rentPrice, string gmDescription, int rentaId, Lifestyles lifestyle)
        {
            var client = new HttpClient();
            var url = $"{URL}/qr/model/{qr}";
            var data = new
            {
                id,
                name,
                description,
                numberOfUses,
                basePrice,
                rentPrice,
                gmDescription,
                dealId = rentaId.ToString(),
                lifestyle = lifestyle.ToString()
            };
            var body = new
            {
                eventType = "createMerchandise",
                data
            };
            var json = Serializer.ToJSON(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            return response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created;
        }

        private static decimal ParseToDecimal(string value)
        {
            decimal dec = 0;
            decimal.TryParse(value, out dec);
            return dec;
        }


    }
}
