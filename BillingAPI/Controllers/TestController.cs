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
            var manager = new ExcelManager();
            var memStream = manager.LoadTransfers();
            byte[] fileBytes = memStream.ToArray();
            string fileName = "transfers.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        [HttpGet("testid")]
        public ActionResult TestId(int character)
        {
            return new JsonResult(character);
        }


    }
}
