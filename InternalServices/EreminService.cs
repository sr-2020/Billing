using Core;
using Core.Primitives;
using InternalServices.EreminModel;
using Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalServices
{
    public class EreminService
    {
        string _URL = Environment.GetEnvironmentVariable("MODELS_MANAGER_URL") ?? "https://gateway.evarun.ru/api/v1/models-manager";
        //string _URL = Environment.GetEnvironmentVariable("MODELS_MANAGER_URL");

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
                every = model?.workModel?.discounts?.everything ?? 1;
                if (every < 1)
                {
                    every = Math.Max(every, 0.7m);
                }
                else
                {
                    every = Math.Min(every, 1.5m);
                }
                if (discountType == DiscountType.Samurai)
                    every = every * ( model?.workModel?.discounts?.weaponsArmor ?? 1);
                var discount1 = model?.workModel?.passiveAbilities?.Any(p => p.id == "discount-all-1") ?? false;
                var discount2 = model?.workModel?.passiveAbilities?.Any(p => p.id == "discount-all-2") ?? false;
                var discount3 = model?.workModel?.passiveAbilities?.Any(p => p.id == "discount-all-3") ?? false;
                var discount4 = model?.workModel?.passiveAbilities?.Any(p => p.id == "discount-all-4") ?? false;
                var discount5 = model?.workModel?.passiveAbilities?.Any(p => p.id == "discount-all-5") ?? false;
                decimal totalDisc = 0;
                if (discount1)
                    totalDisc += 0.1m;
                if(discount2)
                    totalDisc += 0.1m;
                if (discount3)
                    totalDisc += 0.1m;
                if (discount4)
                    totalDisc += 0.1m;
                if (discount5)
                    totalDisc = totalDisc * 2;
                every = every * (1 - totalDisc);
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
