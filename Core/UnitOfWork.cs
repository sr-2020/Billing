using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Core
{
    public interface IUnitOfWork
    {
        BillingContext BillingContext { get; }
    }
    public class UnitOfWork : IUnitOfWork
    {
        public static IConfiguration Configuration { get; set; }
        private BillingContext _billingContext;
        public BillingContext BillingContext => _billingContext ?? (_billingContext = new BillingContext());

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
            var db = Configuration.GetConnectionString(dataBase);
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("Environment user is empty");
            if (string.IsNullOrEmpty(db))
                throw new ArgumentException("DataBase connection string is empty");
            return $"{db} User Id = {user}; Password = {password}";
        }
    }
}
