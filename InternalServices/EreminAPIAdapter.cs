using Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace InternalServices
{
    public class EreminAPIAdapter
    {
        const string URL = "http://push.evarun.ru";

        public async static void SendNotification(int characterId, string title, string body)
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
            await client.PostAsync(url, content);
        }
    }
}
