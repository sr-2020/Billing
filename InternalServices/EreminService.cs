using Core;
using Core.Primitives;
using InternalServices.EreminModel;
using Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalServices
{
    public class EreminService
    {
        string _URL = Environment.GetEnvironmentVariable("MODELS_MANAGER_URL") ?? "https://models-manager.evarun.ru";

        public CharacterModel GetCharacter(int characterId)
        {
            var client = new HttpClient();
            var url = $"{_URL}/character/model/{characterId}";
            var response = client.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var model = Serializer.Deserialize<CharacterModel>(response.Content.ReadAsStringAsync().Result);
                return model;
            }
            throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public bool GetAnonimous(int characterId)
        {
            var model = GetCharacter(characterId);
            if (model != null)
            {
                return model.workModel.billing.anonymous ?? false;
            }
            return false;
        }

        public decimal GetDiscount(int characterId, DiscountType discountType)
        {
            var model = GetCharacter(characterId);
            decimal every = 1;
            if (model != null)
            {
                var gesheft = model?.workModel?.discounts?.everything ?? 1;
                decimal samurai = 1;
                if (discountType == DiscountType.Samurai)
                    samurai = model?.workModel?.discounts?.weaponsArmor ?? 1;
                every = gesheft < samurai ? gesheft : samurai;
            }
            return every;
        }

        public async Task ConsumeFood(int rentaId, Lifestyles lifestyle, int modelId)
        {
            var eventType = "consumeFood";
            var id = "food";
            var data = new
            {
                id,
                dealId = rentaId.ToString(),
                lifestyle = lifestyle.ToString(),

            };
            var url = $"{_URL}/character/model/{modelId}";
            await CreateEvent(data, eventType, url);
        }

        private async Task CreateEvent(dynamic data, string eventType, string url)
        {
            var client = new HttpClient();
            var body = new
            {
                eventType,
                data
            };
            var json = Serializer.ToJSON(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(url, content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                    return;
                var message = await response.Content.ReadAsStringAsync();
                throw new BillingException(message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                throw;
            }
        }

        public async Task CleanQR(string qr)
        {
            var eventType = "clear";

            var data = new
            {

            };
            var url = $"{_URL}/qr/model/{qr}";
            await CreateEvent(data, eventType, url);
        }

        public async Task UpdateQR(string qr, decimal basePrice, decimal rentPrice, string gmDescription, int rentaId, Lifestyles lifestyle)
        {
            var eventType = "updateMerchandise";
            var data = new
            {
                basePrice,
                rentPrice,
                gmDescription,
                dealId = rentaId.ToString(),
                lifestyle = lifestyle.ToString()
            };
            var url = $"{_URL}/qr/model/{qr}";
            await CreateEvent(data, eventType, url);
        }

        public async Task WriteQR(string qr, string id, string name, string description, int numberOfUses, decimal basePrice, decimal rentPrice, string gmDescription, int rentaId, Lifestyles lifestyle)
        {
            var eventType = "createMerchandise";
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
            var url = $"{_URL}/qr/model/{qr}";
            await CreateEvent(data, eventType, url);
        }
    }
}
