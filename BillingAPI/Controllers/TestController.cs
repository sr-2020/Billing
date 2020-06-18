using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Filters;
using Hangfire;
using InternalServices;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settings;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
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
    }
}
