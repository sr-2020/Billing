using Microsoft.Extensions.Configuration;
using System;

namespace Core
{
    public static class UnitOfWork
    {
        public static IConfiguration Configuration { get; set; }

        public static void Init(IConfiguration configuration = null)
        {
            Configuration = configuration;
        }

        public static string GetConnectionString(string dataBase)
        {
            if (Configuration == null)
                throw new ArgumentNullException("Configuration is null");
            var user = Environment.GetEnvironmentVariable("POSTGRESQL_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD");
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("Environment user is empty");
            var db = Configuration.GetConnectionString(dataBase);
            if (string.IsNullOrEmpty(db))
                throw new ArgumentException("DataBase connection string is empty");
            return $"{Configuration.GetConnectionString(dataBase)} User Id = {user}; Password = {password}";
        }
    }
}
