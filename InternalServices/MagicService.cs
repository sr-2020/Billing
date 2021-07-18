using Core;
using Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InternalServices
{
    public class MagicService
    {
        public const string InternalUrl = "https://maps-n-magic2.evarun.ru";
        public const string ExternalUrl = "http://maps-n-magic2";

        public async Task PutSpiritInJar(int qr, string code)
        {
            var client = new HttpClient();
            var body = new
            {
                spiritType = code,
                qrId = qr
            };
            var json = Serializer.ToJSON(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var url = $"{InternalUrl}/innerApi2/putSpiritInJar";
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

    }
}
