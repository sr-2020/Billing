using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using InternalServices;
using IoC;
using Jobs;
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
        [HttpGet("testaction")]
        public ActionResult Test(int id)
        {
            var test = EreminService.GetCharacter(id);
            return new JsonResult("ok");
        }

    }
}
