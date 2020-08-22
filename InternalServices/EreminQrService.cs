using InternalServices.EreminModel;
using Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace InternalServices
{
    public class EreminQrService
    {
        const string URL1 = @"https://qr.aerem.in";
        const string URL2 = @"https://decodeit.ru/image.php?type=qr&value=";

        public static string GetQRUrl(long payload)
        {
            var content = GetQrContent(payload);
            return GetQRUrl(content);
        }

        public static string GetQrContent (long payload)
        {
            var client = new HttpClient();
            var url = $"{URL1}/encode?type=200&kin=0&validUntil=0&payload={payload}";
            var response = client.GetAsync(url).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var model = Serializer.Deserialize<QRModel>(response.Content.ReadAsStringAsync().Result);
                return model.Content;
            }
            throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public static string GetQRUrl(string content)
        {
            return $"{URL2}{content}";
        }

    }
}
