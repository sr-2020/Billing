using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Billing;
using BillingAPI.Filters;
using Core;
using Hangfire;
using InternalServices;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scoringspace;
using Settings;

namespace BillingAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : EvarunApiController
    {
        /// <summary>
        /// Получить общую версию проекта
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public ActionResult<string> Get()
        {
            return IocContainer.Get<ISettingsManager>().GetValue(Core.Primitives.SystemSettingsEnum.eversion);
        }
        [HttpGet("test")]
        public ActionResult Test()
        {
            //Console.WriteLine("test started");
            //Task.Run(() =>
            //{
            //    Console.WriteLine("task started");
            //    var manager = IocContainer.Get<IJobManager>();
            //    var result = RunAction(() => manager.(), $"period");
            //    Thread.Sleep(1000);
            //    Console.WriteLine("task ended");
            //});
            //Console.WriteLine("test ended");
            return new JsonResult("sadasd");
        }
        [HttpGet("testid")]
        public ActionResult TestId(int character)
        {
            return new JsonResult(character);
        }


        /// <summary>
        /// Пересчитать ренты
        /// </summary>
        /// <returns></returns>
        [HttpGet("addinsurances")]
        public ActionResult AddInsurances()
        {
            var manager = IocContainer.Get<IInsuranceManager>();
            manager.AddInsurances();
            return new JsonResult("success");
        }

        /// <summary>
        /// Пересчитать ренты
        /// </summary>
        /// <returns></returns>
        [HttpGet("testscoring")]
        public ActionResult TestScoring()
        {
            var manager = IocContainer.Get<ScoringManager>();
            var random = new Random();
            var rnd = random.Next(1, 3);
            if (rnd == 1)
                manager.OnTest(1677);
            else
            {
                manager.OnTest(1681);
            }

            return new JsonResult("success");
        }

    }
}
