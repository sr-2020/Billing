using Billing;
using Core;
using IoC;
using IoC.Init;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;

namespace NillingTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            SetVariable("DBHOST", "instance.evarun.ru");
            SetVariable("POSTGRESQLHANGFIRE_DBNAME", "Hangfire");
            SetVariable("POSTGRESQLHANGFIRE_USER", "hangfire");
            SetVariable("POSTGRESQLHANGFIRE_PASSWORD", "8XrdkzCQuFaSREeZlfnQo1");
            SetVariable("POSTGRESQLBILLING_DBNAME", "rc_sr2020");
            SetVariable("POSTGRESQLBILLING_USER", "backend");
            SetVariable("POSTGRESQLBILLING_PASSWORD", "kz1x21YB1zYJx");
            IocInitializer.Init();
            var configuration = GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);
            SystemHelper.Init(configuration);
        }

        [Test]
        public void CreateOrUpdateWalletTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var update = 10312;
                var wallet = billing.CreateOrUpdatePhysicalWallet(update, 1000);
                Assert.NotNull(wallet);
                Assert.NotNull(wallet?.Scoring);
                Assert.NotNull(wallet?.Wallet);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetTransfersTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var update = 10312;
                var transfers = billing.GetTransfers(update);
                Assert.NotNull(transfers);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void GetBalanceTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var update = 10312;
                var balance = billing.GetBalance(update);
                Assert.NotNull(balance);

            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        [Test]
        public void GetRentasTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var update = 10312;
                var rentas = billing.GetRentas(update);
                Assert.NotNull(rentas);

            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }


        private void SetVariable(string variableName, string value)
        {
            Environment.SetEnvironmentVariable(variableName, value);
        }

        private IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }


    }
}