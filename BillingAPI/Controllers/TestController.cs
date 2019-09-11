using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET api/version
        [HttpGet]
        public ActionResult<string> Get()
        {
            //RecurringJob.AddOrUpdate(
            //    () =>
            //    Debug.Write($"{DateTime.Now}: test completed"), 
                
            //    "*/1 * * * *");
            return "Hello world!";
        }

    }
}
