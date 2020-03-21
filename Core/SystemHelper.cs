﻿using Microsoft.EntityFrameworkCore;
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
