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
using Settings;

namespace BillingAPI.Controllers
{
    [Route("test/[controller]")]
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
            var manager = IocContainer.Get<IBillingManager>();
            manager.ProcessRentas(modelId);
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

    }
}
