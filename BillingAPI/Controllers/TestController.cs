using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Jobs;
using Microsoft.AspNetCore.Mvc;

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
            return GetType().Assembly.GetName().Version.ToString();
        }

    }
}
