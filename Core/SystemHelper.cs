using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Core
{
    public class SystemHelper
    {
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        static IConfiguration _configuration;

        public static string GetConnectionString(string dataBase)
        {
            if (_configuration == null)
                throw new ArgumentNullException("Configuration is null");
            var user = Environment.GetEnvironmentVariable("POSTGRESQL_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD");
            var db = _configuration.GetConnectionString(dataBase);
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("Environment user is empty");
            if (string.IsNullOrEmpty(db))
                throw new ArgumentException("DataBase connection string is empty");
            return $"{db} User Id = {user}; Password = {password}";
        }

        public static DateTime ConvertDateTimeToLocal(DateTime? dateTime = null)
        {
            if (!dateTime.HasValue)
                dateTime = DateTime.Now;
            return TimeZoneInfo.ConvertTime(dateTime.Value, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"), TimeZoneInfo.Local);
        }

        public static DateTime ConvertDateTimeToMoscow(DateTime? dateTime = null)
        {
            if (!dateTime.HasValue)
                dateTime = DateTime.Now;
            return TimeZoneInfo.ConvertTime(dateTime.Value, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow"));
        }
    }
}
