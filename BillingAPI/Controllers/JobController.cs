using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("start")]
        public string Start()
        {
            try
            {
                RecurringJob.AddOrUpdate("main", (JobManager jm) =>
                jm.DoLongJob(),
                "0-59 * * * * ", TimeZoneInfo.Local);
            }
            catch (Exception e)
            {

                return e.ToString();
            }

            return "success";
        }
        [HttpGet("stop")]
        public string Stop()
        {
            try
            {
                RecurringJob.RemoveIfExists("main");
            }
            catch (Exception e)
            {

                return e.ToString();
            }

            return "success";
        }

    }
}