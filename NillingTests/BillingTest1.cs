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
            SetVariable("POSTGRESQLBILLING_DBNAME", "sr2020");
            SetVariable("POSTGRESQLBILLING_USER", "backend");
            SetVariable("POSTGRESQLBILLING_PASSWORD", "kz1x21YB1zYJx");
            IocInitializer.Init();
            var configuration = GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);
            SystemHelper.Init(configuration);
        }
        public int TestId = 44043;
        [Test]
        public void InitCharacterTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var test = TestId;
                var wallet = billing.InitCharacter(test, "Случай", "meta-norm");
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
        public void MakeTransferSINSINTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var from = TestId;
                var to = 55817;
                var transfer = billing.MakeTransferSINSIN(from, to, 1, "тест");
                Assert.NotNull(transfer);

            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void CreateOrUpdateWalletTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var test = TestId;
                var wallet = billing.CreateOrUpdatePhysicalWallet(test, "Случай", 1, 100000);
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
                var test = TestId;
                var transfers = billing.GetTransfers(test);
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
                var test = TestId;
                var balance = billing.GetBalance(test);
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
                var test = TestId;
                var rentas = billing.GetRentas(test);
                Assert.NotNull(rentas);

            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ProcessCycleTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {

                Assert.NotNull(null);

            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ProcessPeriodTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var test = TestId;
                billing.ProcessPeriod(test);
                
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void ConfirmRentaTest()
        {
            var billing = IocContainer.Get<IBillingManager>();
            try
            {
                var sku = 199;
                var shop = 3;
                var character = TestId;
                var price = billing.GetPrice(character, shop, sku);
                Assert.AreNotEqual(0, price.PriceId);
                var renta = billing.ConfirmRenta(10312, price.PriceId);
                Assert.AreNotEqual(0, renta.RentaId);
                var transfers = billing.GetTransfersByRenta(renta.RentaId);
                Assert.NotNull(transfers);
                Assert.AreEqual(3, transfers.Count);
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