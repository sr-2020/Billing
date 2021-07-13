using Core;
using IoC.Init;
using Jobs;
using Microsoft.Extensions.Configuration;
using System;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            Do();


            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        static void Do()
        {
            var life = new JobLifeService();
            var beat = life.DoBeat(Core.Primitives.BeatTypes.Characters, "test", true);
        }

        static void Init()
        {
            TestHelper.SetVariable("DBHOST", "instance.evarun.ru");
            TestHelper.SetVariable("POSTGRESQLHANGFIRE_DBNAME", "Hangfire");
            TestHelper.SetVariable("POSTGRESQLHANGFIRE_USER", "hangfire");
            TestHelper.SetVariable("POSTGRESQLHANGFIRE_PASSWORD", "8XrdkzCQuFaSREeZlfnQo1");
            TestHelper.SetVariable("POSTGRESQLBILLING_DBNAME", "rc_sr2020");
            TestHelper.SetVariable("POSTGRESQLBILLING_USER", "backend");
            TestHelper.SetVariable("POSTGRESQLBILLING_PASSWORD", "kz1x21YB1zYJx");
            IocInitializer.Init();
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();
            SystemHelper.Init(configuration);

        }

        public class TestHelper
        {
            public static void SetVariable(string variableName, string value)
            {
                Environment.SetEnvironmentVariable(variableName, value);
            }
            public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
            {
                return new ConfigurationBuilder()
                    .SetBasePath(outputPath)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
        }
    }
}
