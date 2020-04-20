using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NCrontab;
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
            var user = Environment.GetEnvironmentVariable(_configuration.GetConnectionString(dataBase + "User"));
            var password = Environment.GetEnvironmentVariable(_configuration.GetConnectionString(dataBase + "Password"));
            var host = Environment.GetEnvironmentVariable(_configuration.GetConnectionString("host"));
            var db = Environment.GetEnvironmentVariable(_configuration.GetConnectionString(dataBase));
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("Environment user is empty");
            if (string.IsNullOrEmpty(db))
                throw new ArgumentException("DataBase connection string is empty");
            return $"Server = {host}; Database = {db}; persist security info = True;User Id = {user}; Password = {password}";
        }

        public static DateTimeOffset? SetMoscowTimeSpan(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
                return null;
            var result = DateTimeOffset.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentUICulture, System.Globalization.DateTimeStyles.AssumeLocal);
            return new DateTimeOffset(result.Ticks, TimeSpan.FromHours(3));
        }

        public static CrontabSchedule CronParse(string cron)
        {
            if (string.IsNullOrWhiteSpace(cron))
                return null;
            return CrontabSchedule.Parse(cron);
        }
    }
}
