using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

namespace Serialization
{
    public static class Serializer
    {
        public static string ToJSON(object obj, JsonSerializerSettings settings)
        {
            var result = JsonConvert.SerializeObject(
                obj,
                Formatting.Indented,
                settings
            );
            return result;
        }

        public static string ToJSON(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return ToJSON(obj, settings);
        }

        public static T DeserializeAnonymousType<T>(string json, T model)
        {
            return JsonConvert.DeserializeAnonymousType(json, model);
        }

        public static T Deserialize<T>(string obj, string dateFormat = null)
        {
            if (string.IsNullOrEmpty(dateFormat))
            {
                return JsonConvert.DeserializeObject<T>(obj);
            }

            return JsonConvert.DeserializeObject<T>(obj, new IsoDateTimeConverter { DateTimeFormat = dateFormat });
        }

        public static T DeserializeFromFile<T>(string filePath, string dateFormat = null)
        {
            return Deserialize<T>(File.ReadAllText(filePath), dateFormat);
        }

    }
}
