using Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalServices
{
    public class EreminPushAdapter
    {
        const string URL = "https://push.evarun.ru";

        public async static Task SendNotification(int characterId, string title, string body)
        {
            try
            {
                var url = $"{URL}/send_notification/{characterId}";
                var client = new HttpClient();
                var message = new
                {
                    title,
                    body
                };
                var json = Serializer.ToJSON(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //await client.PostAsync(url, content);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ошибка notification");
                Console.Error.WriteLine($"{e.Message}");
            }

        }
    }
}
