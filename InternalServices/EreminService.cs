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

        public static decimal GetDiscount(int characterId, DiscountType discountType)
        {
            var model = GetCharacter(characterId);
            if (model != null)
            {
                var gesheft = ParseToDecimal(model.workModel.discounts.all);
                decimal samurai = 0;
                if (discountType == DiscountType.Samurai)
                    samurai = ParseToDecimal(model.workModel.discounts.samurai);
                return gesheft > samurai ? gesheft : samurai;
            }
            return 0;
        }

        public static bool WriteQR(string qr, string id, string name, string description, int numberOfUses, object model)
        {
            var client = new HttpClient();
            var url = $"{URL}/qr/model/{qr}";
            var data = new
            {
                id,
                name,
                description,
                numberOfUses,
                additionalData = model
            };
            var body = new
            {
                eventType = "createMerchandise",
                data
            };
            var json = Serializer.ToJSON(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        private static decimal ParseToDecimal(string value)
        {
            decimal dec = 0;
            decimal.TryParse(value, out dec);
            return dec;
        }


    }
}
