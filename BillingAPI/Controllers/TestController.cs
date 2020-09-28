using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class TestController : ControllerBase
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
        [HttpGet("getcharacter")]
        public ActionResult Test(int character)
        {
            var test = EreminService.GetCharacter(character);
            return new JsonResult(test);
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
        [HttpGet("processrentas")]
        public ActionResult ProcessRentas(int modelId = 0)
        {
            Console.WriteLine("processrentas started");
            var manager = IocContainer.Get<IBillingManager>();
            manager.ProcessRentas(modelId);
            Console.WriteLine("processrentas finished");
            return new JsonResult("success");
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
            if(rnd == 1)
                manager.OnTest(1677);
            else
            {
                manager.OnTest(1681);
            }

            return new JsonResult("success");
        }

    }
}
