using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using IoC;
using Jobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        [HttpGet("start/{id}")]
        public string Start(string id)
        {
            try
            {
                var jm = IocContainer.Get<IJobManager>();
                var job = jm.GetJob(id);
                foreach (var date in job.GetJobSchedules())
                {
                    BackgroundJob.Schedule(() => job.DoJob(), new DateTimeOffset(date));
                }
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